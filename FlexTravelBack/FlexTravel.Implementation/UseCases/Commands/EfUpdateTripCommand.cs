using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfUpdateTripCommand : EfUseCase, IUpdateTripCommand
    {
        private readonly CurrentAgency _agency;
        private readonly CreateTripValidator _validator;
        private readonly IUser _user;
        private readonly WaitlistService _waitlist;
        private readonly NotificationService _notifications;
        private readonly PriceAlertService _priceAlerts;

        public EfUpdateTripCommand(FlexTravelContext context, CurrentAgency agency, CreateTripValidator validator, IUser user,
            WaitlistService waitlist, NotificationService notifications, PriceAlertService priceAlerts)
            : base(context)
        {
            _agency = agency;
            _validator = validator;
            _user = user;
            _waitlist = waitlist;
            _notifications = notifications;
            _priceAlerts = priceAlerts;
        }

        public int UseCaseId => UseCaseIds.UpdateTrip;
        public string UseCaseName => "Update trip";
        public string UseCaseDescription => "Agency edits its trip";

        public void Execute(UpdateTripDto request)
        {
            var agencyId = _agency.Id(UseCaseName);

            var trip = Context.Trip
                .Include(x => x.Prices)
                .Include(x => x.Accommodations)
                .Include(x => x.Attractions)
                .Include(x => x.Itinerary).ThenInclude(i => i.ItineraryActivities)
                .Include(x => x.Extras)
                .FirstOrDefault(x => x.TripId == request.Id && x.AgencyId == agencyId);

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", request.Id);
            }

            if (!trip.IsActive)
            {
                throw new ConflictException("Withdrawn trips cannot be edited.");
            }

            if (trip.StartDate.Date <= DateTime.Today)
            {
                throw new ConflictException("Trips that have already departed cannot be edited.");
            }

            request.DestinationId = trip.DestinationId;
            request.AttractionIds ??= new List<int>();
            request.Itinerary ??= new List<ItineraryDayInputDto>();
            request.Extras ??= new List<TripExtraInputDto>();
            _validator.ValidateAndThrow(request);

            var unknownExtra = request.Extras.FirstOrDefault(e => e.Id.HasValue && trip.Extras.All(x => x.TripExtraId != e.Id.Value));
            if (unknownExtra != null)
            {
                throw new ConflictException($"The extra \"{unknownExtra.Name}\" does not belong to this trip.");
            }

            var active = Context.Reservation
                .Where(r => r.TripId == trip.TripId && r.IsActive
                    && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                .Select(r => new { r.AccommodationId, Travelers = r.NumberOfAdults + r.NumberOfChildren })
                .ToList();

            if (active.Any())
            {
                if (request.StartDate.Date != trip.StartDate.Date || request.EndDate.Date != trip.EndDate.Date)
                {
                    throw new ConflictException(
                        $"The dates cannot change while the trip has {active.Count} active reservation(s).");
                }

                var seatsBooked = active.Sum(r => r.Travelers);
                if (request.Capacity < seatsBooked)
                {
                    throw new ConflictException($"{seatsBooked} seats are already booked, so the capacity cannot be lower.");
                }

                var removedBookedHotel = active.Select(r => r.AccommodationId).Distinct()
                    .FirstOrDefault(id => !request.AccommodationIds.Contains(id));
                if (removedBookedHotel != 0)
                {
                    var hotelName = Context.Accommodation.Where(a => a.AccommodationId == removedBookedHotel)
                        .Select(a => a.AccommodationName).First();
                    throw new ConflictException($"{hotelName} has reservations and cannot be removed from the trip.");
                }
            }

            var capacityGrew = request.Capacity > trip.Capacity;

            trip.TripName = request.Name.Trim();
            trip.TripDescription = request.Description.Trim();
            trip.TransportId = request.TransportId;
            trip.StartDate = request.StartDate.Date;
            trip.EndDate = request.EndDate.Date;
            trip.Capacity = request.Capacity;
            trip.ChildDiscountPercent = request.ChildDiscountPercent;
            trip.EarlyBookingPercent = request.EarlyBookingPercent;
            trip.EarlyBookingUntil = request.EarlyBookingPercent > 0 ? request.EarlyBookingUntil?.Date : null;
            trip.LastMinutePercent = request.LastMinutePercent;
            trip.LastMinuteDays = request.LastMinutePercent > 0 ? request.LastMinuteDays : 0;
            trip.UpdatedByUserId = _user.Id;

            var currentPrice = BookingPricing.TripPriceAt(trip.Prices, DateTime.Now);
            if (request.PricePerPerson != currentPrice)
            {
                trip.Prices.Add(new TripPrice { Price = request.PricePerPerson, ValidFrom = DateTime.Now });
            }

            var wanted = request.AccommodationIds.Distinct().ToList();
            foreach (var removed in trip.Accommodations.Where(a => !wanted.Contains(a.AccommodationId)).ToList())
            {
                trip.Accommodations.Remove(removed);
            }
            foreach (var added in wanted.Where(id => trip.Accommodations.All(a => a.AccommodationId != id)))
            {
                trip.Accommodations.Add(new TripAccommodation { AccommodationId = added });
            }

            var wantedAttractions = request.AttractionIds.Distinct().ToList();
            foreach (var removed in trip.Attractions.Where(a => !wantedAttractions.Contains(a.AttractionId)).ToList())
            {
                trip.Attractions.Remove(removed);
            }
            foreach (var added in wantedAttractions.Where(id => trip.Attractions.All(a => a.AttractionId != id)))
            {
                trip.Attractions.Add(new TripAttraction { AttractionId = added });
            }

            Context.RemoveRange(trip.Itinerary.ItineraryActivities);
            trip.Itinerary.ItineraryName = trip.TripName;
            foreach (var activity in ItineraryRules.ToActivities(request.Itinerary))
            {
                trip.Itinerary.ItineraryActivities.Add(activity);
            }

            ExtraRules.Apply(trip, request.Extras);

            Context.SaveChanges();

            if (capacityGrew)
            {
                _waitlist.SeatsReleased(trip.TripId);
            }

            _priceAlerts.Check(trip.DestinationId);
            _notifications.Push();
        }
    }
}
