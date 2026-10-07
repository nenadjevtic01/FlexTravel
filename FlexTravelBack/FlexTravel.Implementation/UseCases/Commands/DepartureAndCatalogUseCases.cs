using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.Files;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfAddDepartureCommand : EfUseCase, IAddDepartureCommand
    {
        private readonly CurrentAgency _agency;
        private readonly IUser _user;
        private readonly NotificationService _notifications;
        private readonly PriceAlertService _priceAlerts;

        public EfAddDepartureCommand(FlexTravelContext context, CurrentAgency agency, IUser user, NotificationService notifications,
            PriceAlertService priceAlerts)
            : base(context)
        {
            _agency = agency;
            _user = user;
            _notifications = notifications;
            _priceAlerts = priceAlerts;
        }

        public int UseCaseId => UseCaseIds.AddDeparture;
        public string UseCaseName => "Add departure";
        public string UseCaseDescription => "Agency adds another date of a trip";

        public int Execute(AddDepartureDto request)
        {
            var agencyId = _agency.Id(UseCaseName);

            var source = Context.Trip
                .Include(t => t.Prices)
                .Include(t => t.Accommodations).ThenInclude(a => a.Accommodation)
                .Include(t => t.Attractions)
                .Include(t => t.Itinerary).ThenInclude(i => i.ItineraryActivities)
                .Include(t => t.Extras)
                .FirstOrDefault(t => t.TripId == request.TripId && t.AgencyId == agencyId)
                ?? throw new EntityNotFoundException("Trip", request.TripId);

            var start = request.StartDate.Date;
            if (start <= DateTime.Today)
            {
                throw new ValidationException(new[] { new ValidationFailure("StartDate", "The new date must be in the future.") });
            }

            if (Context.Trip.Any(t => t.TripSeriesId == source.TripSeriesId && t.IsActive && t.StartDate == start))
            {
                throw new ConflictException($"This trip already departs on {start.ToString("d MMM yyyy", CultureInfo.InvariantCulture)}.");
            }

            var hotels = source.Accommodations.Where(a => a.Accommodation.IsActive).Select(a => a.AccommodationId).ToList();
            if (hotels.Count == 0)
            {
                throw new ConflictException("None of the trip's hotels is offered any more. Create a new trip instead.");
            }

            var departure = new Trip
            {
                AgencyId = agencyId,
                TripSeriesId = source.TripSeriesId,
                DestinationId = source.DestinationId,
                TransportId = source.TransportId,
                TripName = source.TripName,
                TripDescription = source.TripDescription,
                StartDate = start,
                EndDate = start.AddDays((source.EndDate.Date - source.StartDate.Date).Days),
                Capacity = source.Capacity,
                ChildDiscountPercent = source.ChildDiscountPercent,
                LastMinutePercent = source.LastMinutePercent,
                LastMinuteDays = source.LastMinuteDays,
                CreatedByUserId = _user.Id,
                Itinerary = new Itinerary { ItineraryName = source.Itinerary.ItineraryName }
            };

            foreach (var activity in source.Itinerary.ItineraryActivities)
            {
                departure.Itinerary.ItineraryActivities.Add(ItineraryRules.Copy(activity));
            }

            foreach (var extra in source.Extras.Where(e => e.IsActive))
            {
                departure.Extras.Add(ExtraRules.Copy(extra));
            }

            departure.Prices.Add(new TripPrice { Price = BookingPricing.TripPriceAt(source.Prices, DateTime.Now), ValidFrom = DateTime.Today });

            foreach (var hotelId in hotels)
            {
                departure.Accommodations.Add(new TripAccommodation { AccommodationId = hotelId });
            }

            foreach (var attraction in source.Attractions)
            {
                departure.Attractions.Add(new TripAttraction { AttractionId = attraction.AttractionId });
            }

            Context.Trip.Add(departure);
            Context.SaveChanges();

            EfCreateTripCommand.NotifyFans(Context, _notifications, departure, agencyId);
            Context.SaveChanges();
            _priceAlerts.Check(departure.DestinationId);
            _notifications.Push();

            return departure.TripId;
        }
    }

    public class EfSetDestinationActiveCommand : EfUseCase, ISetDestinationActiveCommand
    {
        public EfSetDestinationActiveCommand(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.SetDestinationActive;
        public string UseCaseName => "Set destination active";
        public string UseCaseDescription => "Hide or show a destination";

        public void Execute(SetActiveDto request)
        {
            var destination = Context.Destination.FirstOrDefault(d => d.DestinationId == request.Id)
                ?? throw new EntityNotFoundException("Destination", request.Id);

            if (!request.IsActive)
            {
                var onSale = Context.Trip.Count(t => t.DestinationId == request.Id && t.IsActive && t.StartDate > DateTime.Today);
                if (onSale > 0)
                {
                    throw new ConflictException($"{destination.DestinationName} has {onSale} trip(s) on sale. The agencies have to withdraw them first.");
                }
            }

            destination.IsActive = request.IsActive;
            Context.SaveChanges();
        }
    }

    public class EfSetHotelActiveCommand : EfUseCase, ISetHotelActiveCommand
    {
        public EfSetHotelActiveCommand(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.SetHotelActive;
        public string UseCaseName => "Set hotel active";
        public string UseCaseDescription => "Hide or show a hotel";

        public void Execute(SetActiveDto request)
        {
            var hotel = Context.Accommodation.FirstOrDefault(a => a.AccommodationId == request.Id)
                ?? throw new EntityNotFoundException("Hotel", request.Id);

            if (!request.IsActive)
            {
                var onSale = Context.TripAccommodation.Count(t => t.AccommodationId == request.Id
                    && t.Trip.IsActive && t.Trip.StartDate > DateTime.Today);
                if (onSale > 0)
                {
                    throw new ConflictException($"{hotel.AccommodationName} is offered on {onSale} trip(s) on sale, so it cannot be hidden.");
                }
            }

            hotel.IsActive = request.IsActive;
            Context.SaveChanges();
        }
    }

    public class EfCleanUpImagesCommand : EfUseCase, ICleanUpImagesCommand
    {
        private readonly IFileStorage _storage;

        public EfCleanUpImagesCommand(FlexTravelContext context, IFileStorage storage) : base(context)
        {
            _storage = storage;
        }

        public int UseCaseId => UseCaseIds.CleanUpImages;
        public string UseCaseName => "Clean up images";
        public string UseCaseDescription => "Delete uploaded images that nothing uses";

        public int Execute(int request)
        {
            var used = Context.DestinationImage.Select(i => i.Src)
                .Concat(Context.Agency.Where(a => a.Logo != null).Select(a => a.Logo))
                .Concat(Context.User.Where(u => u.Avatar != null).Select(u => u.Avatar))
                .ToHashSet();
            var limit = DateTime.UtcNow.AddHours(-1);
            var unused = _storage.List().Where(f => f.SavedAt < limit && !used.Contains(f.Src)).ToList();

            foreach (var file in unused)
            {
                _storage.Delete(file.Src);
            }

            return unused.Count;
        }
    }
}

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetSimilarTripsQuery : EfUseCase, IGetSimilarTripsQuery
    {
        private const int SimilarShown = 4;
        private readonly ISearchTripsQuery _search;

        public EfGetSimilarTripsQuery(FlexTravelContext context, ISearchTripsQuery search) : base(context)
        {
            _search = search;
        }

        public int UseCaseId => UseCaseIds.GetSimilarTrips;
        public string UseCaseName => "Get similar trips";
        public string UseCaseDescription => "Other departures and similar trips";

        public SimilarTripsDto Execute(int tripId)
        {
            var trip = Context.Trip
                .Where(t => t.TripId == tripId)
                .Select(t => new { t.TripSeriesId, t.DestinationId, t.Destination.CountryId, t.StartDate })
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Trip", tripId);

            var seriesTripIds = Context.Trip
                .Where(t => t.TripSeriesId == trip.TripSeriesId)
                .Select(t => t.TripId)
                .ToHashSet();

            List<TripListDto> Search(TripSearchDto search)
            {
                search.PerPage = 48;
                return _search.Execute(search).Items.ToList();
            }

            var sameDestination = Search(new TripSearchDto { DestinationId = trip.DestinationId });

            var departures = sameDestination
                .Where(t => seriesTripIds.Contains(t.Id) && t.Id != tripId)
                .OrderBy(t => t.StartDate)
                .Select(t => new DepartureDto
                {
                    Id = t.Id,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    SeatsLeft = t.SeatsLeft,
                    PriceFrom = t.PriceFrom,
                    Offer = t.Offer,
                    OfferPercent = t.OfferPercent
                })
                .ToList();

            var similar = sameDestination
                .Concat(Search(new TripSearchDto { CountryId = trip.CountryId }))
                .Concat(Search(new TripSearchDto { DateFrom = trip.StartDate.AddDays(-45), DateTo = trip.StartDate.AddDays(45) }))
                .Where(t => !seriesTripIds.Contains(t.Id))
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .Take(SimilarShown)
                .ToList();

            return new SimilarTripsDto { OtherDepartures = departures, Similar = similar };
        }
    }
}
