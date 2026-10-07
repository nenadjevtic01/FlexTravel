using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetAgencyTripsQuery : EfUseCase, IGetAgencyTripsQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetAgencyTripsQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetAgencyTrips;
        public string UseCaseName => "Get agency trips";
        public string UseCaseDescription => "Trips of the employee's agency with occupancy";

        public AgencyOverviewDto Execute(int request)
        {
            var agencyId = _agency.Id(UseCaseName);

            var trips = Context.Trip
                .Where(x => x.AgencyId == agencyId)
                .OrderBy(x => x.StartDate)
                .Select(x => new
                {
                    x.TripId,
                    x.TripName,
                    Destination = x.Destination.DestinationName,
                    x.StartDate,
                    x.EndDate,
                    x.Capacity,
                    x.IsActive,
                    CreatedBy = x.CreatedBy == null ? null : x.CreatedBy.FirstName + " " + x.CreatedBy.LastName,
                    UpdatedBy = x.UpdatedBy == null ? null : x.UpdatedBy.FirstName + " " + x.UpdatedBy.LastName,
                    x.UpdatedAt,
                    SeatsBooked = x.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0,
                    Pending = x.Reservations.Count(r => r.IsActive && r.Status == ReservationStatus.Pending),
                    Waiting = x.Waitlist.Where(w => w.IsActive).Sum(w => (int?)(w.Adults + w.Children)) ?? 0,
                    x.EarlyBookingPercent,
                    x.EarlyBookingUntil,
                    x.LastMinutePercent,
                    x.LastMinuteDays,
                    Prices = x.Prices.Select(p => new { p.ValidFrom, p.Price }).ToList()
                })
                .ToList();

            var team = Context.User
                .OfType<AgencyEmployee>()
                .Where(e => e.AgencyId == agencyId)
                .OrderBy(e => e.FirstName)
                .Select(e => new AgencyTeamMemberDto
                {
                    Id = e.UserId,
                    Name = e.FirstName + " " + e.LastName,
                    Email = e.Email,
                    Avatar = e.Avatar,
                    IsActive = e.IsActive,
                    TripsCreated = Context.Trip.Count(t => t.CreatedByUserId == e.UserId),
                    ReservationsHandled = Context.ReservationStatusChange.Count(c => c.ChangedByUserId == e.UserId
                        && (c.Status == ReservationStatus.Confirmed || c.Status == ReservationStatus.Rejected))
                })
                .ToList();

            var reviews = Context.Review.Where(r => r.Trip.AgencyId == agencyId);

            return new AgencyOverviewDto
            {
                AgencyId = agencyId,
                AgencyName = Context.Agency.Where(a => a.AgencyId == agencyId).Select(a => a.AgencyName).First(),
                Rating = Ratings.Trip(reviews),
                UnansweredReviews = reviews.Count(r => r.IsActive && r.AgencyReply == null),
                Team = team,
                Trips = trips.Select(x =>
                {
                    var (offer, offerPercent) = BookingPricing.OfferAt(x.StartDate, x.EarlyBookingPercent, x.EarlyBookingUntil,
                        x.LastMinutePercent, x.LastMinuteDays, DateTime.Today);
                    return new AgencyTripDto
                    {
                        Id = x.TripId,
                        Name = x.TripName,
                        Destination = x.Destination,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Capacity = x.Capacity,
                        SeatsBooked = x.SeatsBooked,
                        PendingReservations = x.Pending,
                        Waiting = x.Waiting,
                        PricePerPerson = BookingPricing.PriceAt(x.Prices.Select(p => (p.ValidFrom, p.Price)), DateTime.Now),
                        Offer = x.StartDate > DateTime.Today ? BookingPricing.OfferName(offer) : null,
                        OfferPercent = x.StartDate > DateTime.Today ? offerPercent : 0,
                        IsActive = x.IsActive,
                        CreatedBy = x.CreatedBy,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedAt = x.UpdatedAt
                    };
                }).ToList()
            };
        }
    }
}
