using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using System.Globalization;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfCreateTripCommand : EfUseCase, ICreateTripCommand
    {
        private readonly CurrentAgency _agency;
        private readonly CreateTripValidator _validator;
        private readonly IUser _user;
        private readonly NotificationService _notifications;
        private readonly PriceAlertService _priceAlerts;

        public EfCreateTripCommand(FlexTravelContext context, CurrentAgency agency, CreateTripValidator validator, IUser user,
            NotificationService notifications, PriceAlertService priceAlerts)
            : base(context)
        {
            _agency = agency;
            _validator = validator;
            _user = user;
            _notifications = notifications;
            _priceAlerts = priceAlerts;
        }

        public int UseCaseId => UseCaseIds.CreateTrip;
        public string UseCaseName => "Create trip";
        public string UseCaseDescription => "Agency publishes a new trip";

        public int Execute(CreateTripDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            request.AttractionIds ??= new List<int>();
            request.Itinerary ??= new List<ItineraryDayInputDto>();
            request.Extras ??= new List<TripExtraInputDto>();
            _validator.ValidateAndThrow(request);

            var itinerary = new Itinerary { ItineraryName = request.Name.Trim() };
            foreach (var activity in ItineraryRules.ToActivities(request.Itinerary))
            {
                itinerary.ItineraryActivities.Add(activity);
            }

            var trip = new Trip
            {
                AgencyId = agencyId,
                DestinationId = request.DestinationId,
                TransportId = request.TransportId,
                Itinerary = itinerary,
                TripName = request.Name.Trim(),
                TripDescription = request.Description.Trim(),
                StartDate = request.StartDate.Date,
                EndDate = request.EndDate.Date,
                Capacity = request.Capacity,
                ChildDiscountPercent = request.ChildDiscountPercent,
                EarlyBookingPercent = request.EarlyBookingPercent,
                EarlyBookingUntil = request.EarlyBookingPercent > 0 ? request.EarlyBookingUntil?.Date : null,
                LastMinutePercent = request.LastMinutePercent,
                LastMinuteDays = request.LastMinutePercent > 0 ? request.LastMinuteDays : 0,
                CreatedByUserId = _user.Id,

                Series = new TripSeries { AgencyId = agencyId, Name = request.Name.Trim() }
            };

            trip.Prices.Add(new TripPrice { Price = request.PricePerPerson, ValidFrom = DateTime.Today });

            foreach (var accommodationId in request.AccommodationIds.Distinct())
            {
                trip.Accommodations.Add(new TripAccommodation { AccommodationId = accommodationId });
            }

            foreach (var attractionId in request.AttractionIds.Distinct())
            {
                trip.Attractions.Add(new TripAttraction { AttractionId = attractionId });
            }

            foreach (var extra in request.Extras)
            {
                trip.Extras.Add(ExtraRules.ToEntity(extra));
            }

            Context.Trip.Add(trip);
            Context.SaveChanges();

            NotifyFans(Context, _notifications, trip, agencyId);
            Context.SaveChanges();
            _priceAlerts.Check(trip.DestinationId);
            _notifications.Push();

            return trip.TripId;
        }

        internal static void NotifyFans(FlexTravelContext context, NotificationService notifications, Trip trip, int agencyId)
        {
            var fans = context.UserFavorite
                .Where(f => f.DestinationId == trip.DestinationId && f.User.IsActive)
                .Select(f => f.UserId)
                .ToList();

            if (fans.Count == 0)
            {
                return;
            }

            var destination = context.Destination.Where(d => d.DestinationId == trip.DestinationId).Select(d => d.DestinationName).First();
            var agency = context.Agency.Where(a => a.AgencyId == agencyId).Select(a => a.AgencyName).First();
            var departs = trip.StartDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture);

            foreach (var userId in fans)
            {
                notifications.Send(userId, NotificationType.NewTripToFavorite,
                    $"New trip to {destination}",
                    $"{agency} published {trip.TripName}, departing {departs}.",
                    $"/trips/{trip.TripId}");
            }
        }
    }
}
