using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Queries.Administration
{
    public class EfGetAdminStatsQuery : EfUseCase, IGetAdminStatsQuery
    {
        private const int MonthsInChart = 6;

        public EfGetAdminStatsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetAdminStats;
        public string UseCaseName => "Get admin stats";
        public string UseCaseDescription => "System overview for the admin";

        public AdminStatsDto Execute(int request)
        {
            var today = DateTime.Today;
            var reservations = Context.Reservation.Where(r => r.IsActive);
            var holding = reservations.Where(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed);

            var byStatus = reservations
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count(), Value = g.Sum(r => r.TotalPrice) })
                .ToList();

            int Count(ReservationStatus status) => byStatus.FirstOrDefault(x => x.Status == status)?.Count ?? 0;
            decimal Value(ReservationStatus status) => byStatus.FirstOrDefault(x => x.Status == status)?.Value ?? 0;

            var firstMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-(MonthsInChart - 1));
            var monthly = holding
                .Where(r => r.CreatedAt >= firstMonth)
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Value = g.Sum(r => r.TotalPrice) })
                .ToList();

            var bookingsByMonth = Enumerable.Range(0, MonthsInChart)
                .Select(i => firstMonth.AddMonths(i))
                .Select(month =>
                {
                    var data = monthly.FirstOrDefault(m => m.Year == month.Year && m.Month == month.Month);
                    return new MonthlyBookingsDto
                    {
                        Month = month.ToString("yyyy-MM"),
                        Reservations = data?.Count ?? 0,
                        Value = data?.Value ?? 0
                    };
                })
                .ToList();

            return new AdminStatsDto
            {
                Customers = Context.User.OfType<RegularUser>().Count(),
                AgencyEmployees = Context.User.OfType<AgencyEmployee>().Count(),
                Admins = Context.User.OfType<Domain.Admin>().Count(),
                BlockedUsers = Context.User.Count(u => !u.IsActive),
                Agencies = Context.Agency.Count(),
                TripsOnSale = Context.Trip.Count(t => t.IsActive && t.StartDate > today),
                ReservationsPending = Count(ReservationStatus.Pending),
                ReservationsConfirmed = Count(ReservationStatus.Confirmed),
                ReservationsCancelled = Count(ReservationStatus.Cancelled),
                ReservationsRejected = Count(ReservationStatus.Rejected),
                ConfirmedRevenue = Value(ReservationStatus.Confirmed),
                CollectedPayments = Context.Payment.Sum(p => (decimal?)p.Amount) ?? 0,
                PendingValue = Value(ReservationStatus.Pending),
                BookingsByMonth = bookingsByMonth,
                TopDestinations = holding
                    .GroupBy(r => new { r.Trip.Destination.DestinationName, r.Trip.Destination.Country.CountryName })
                    .Select(g => new TopDestinationDto
                    {
                        Destination = g.Key.DestinationName,
                        Country = g.Key.CountryName,
                        Reservations = g.Count(),
                        Travelers = g.Sum(r => r.NumberOfAdults + r.NumberOfChildren)
                    })
                    .OrderByDescending(x => x.Travelers)
                    .ThenBy(x => x.Destination)
                    .Take(5)
                    .ToList(),
                AgencyResults = Context.Agency
                    .Select(a => new AgencyResultDto
                    {
                        Agency = a.AgencyName,
                        Trips = a.Trips.Count(t => t.IsActive && t.StartDate > today),
                        Reservations = a.Trips.SelectMany(t => t.Reservations)
                            .Count(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed)),
                        Revenue = a.Trips.SelectMany(t => t.Reservations)
                            .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed)
                            .Sum(r => (decimal?)r.TotalPrice) ?? 0,
                        Rating = a.Trips.SelectMany(t => t.Reviews).Where(r => r.IsActive).Average(r => (double?)r.TripRating),
                        Reviews = a.Trips.SelectMany(t => t.Reviews).Count(r => r.IsActive)
                    })
                    .OrderByDescending(x => x.Revenue)
                    .ToList()
                    .Select(x =>
                    {
                        x.Rating = Booking.Ratings.Round(x.Rating);
                        return x;
                    })
                    .ToList()
            };
        }
    }
}
