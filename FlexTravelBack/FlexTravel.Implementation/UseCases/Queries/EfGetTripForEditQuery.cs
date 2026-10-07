using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetTripForEditQuery : EfUseCase, IGetTripForEditQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetTripForEditQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetTripForEdit;
        public string UseCaseName => "Get trip for edit";
        public string UseCaseDescription => "Trip data for the edit form";

        public TripForEditDto Execute(int id)
        {
            var agencyId = _agency.Id(UseCaseName);

            var trip = Context.Trip
                .Where(x => x.TripId == id && x.AgencyId == agencyId)
                .Select(x => new
                {
                    x.TripId,
                    x.TripName,
                    x.TripDescription,
                    x.DestinationId,
                    Destination = x.Destination.DestinationName + ", " + x.Destination.Country.CountryName,
                    x.TransportId,
                    x.StartDate,
                    x.EndDate,
                    x.Capacity,
                    x.ChildDiscountPercent,
                    x.EarlyBookingPercent,
                    x.EarlyBookingUntil,
                    x.LastMinutePercent,
                    x.LastMinuteDays,
                    AttractionIds = x.Attractions.Select(a => a.AttractionId).ToList(),
                    x.IsActive,
                    x.CreatedAt,
                    x.UpdatedAt,
                    CreatedBy = x.CreatedBy == null ? null : x.CreatedBy.FirstName + " " + x.CreatedBy.LastName,
                    UpdatedBy = x.UpdatedBy == null ? null : x.UpdatedBy.FirstName + " " + x.UpdatedBy.LastName,
                    AccommodationIds = x.Accommodations.Select(a => a.AccommodationId).ToList(),
                    Activities = x.Itinerary.ItineraryActivities
                        .Select(a => new ItineraryActivity { Day = a.Day, Title = a.Title, Activity = a.Activity, Meals = a.Meals, Overnight = a.Overnight })
                        .ToList(),
                    Extras = x.Extras
                        .Where(e => e.IsActive)
                        .OrderBy(e => e.TripExtraId)
                        .Select(e => new TripExtra { TripExtraId = e.TripExtraId, Kind = e.Kind, Name = e.Name, Description = e.Description, Price = e.Price, PerPerson = e.PerPerson })
                        .ToList(),
                    Prices = x.Prices.OrderByDescending(p => p.ValidFrom).Select(p => new { p.Price, p.ValidFrom }).ToList(),
                    Active = x.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Select(r => new { r.AccommodationId, Travelers = r.NumberOfAdults + r.NumberOfChildren })
                        .ToList()
                })
                .FirstOrDefault();

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", id);
            }

            return new TripForEditDto
            {
                Id = trip.TripId,
                Name = trip.TripName,
                Description = trip.TripDescription,
                DestinationId = trip.DestinationId,
                Destination = trip.Destination,
                TransportId = trip.TransportId,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Capacity = trip.Capacity,
                PricePerPerson = BookingPricing.PriceAt(trip.Prices.Select(p => (p.ValidFrom, p.Price)), DateTime.Now),
                ChildDiscountPercent = trip.ChildDiscountPercent,
                EarlyBookingPercent = trip.EarlyBookingPercent,
                EarlyBookingUntil = trip.EarlyBookingUntil,
                LastMinutePercent = trip.LastMinutePercent,
                LastMinuteDays = trip.LastMinuteDays,
                AccommodationIds = trip.AccommodationIds,
                AttractionIds = trip.AttractionIds,

                Itinerary = ItineraryRules.ToForm(trip.Activities),
                Extras = trip.Extras.Select(e => new TripExtraInputDto
                {
                    Id = e.TripExtraId,
                    Kind = e.Kind.ToString(),
                    Name = e.Name,
                    Description = e.Description,
                    Price = e.Price,
                    PerPerson = e.PerPerson
                }).ToList(),
                IsActive = trip.IsActive,
                SeatsBooked = trip.Active.Sum(r => r.Travelers),
                ActiveReservations = trip.Active.Count,
                LockedAccommodationIds = trip.Active.Select(r => r.AccommodationId).Distinct().ToList(),
                PriceHistory = trip.Prices.Select(p => new TripPriceHistoryDto { Price = p.Price, ValidFrom = p.ValidFrom }).ToList(),
                CreatedBy = trip.CreatedBy,
                CreatedAt = trip.CreatedAt,
                UpdatedBy = trip.UpdatedBy,
                UpdatedAt = trip.UpdatedAt
            };
        }
    }
}
