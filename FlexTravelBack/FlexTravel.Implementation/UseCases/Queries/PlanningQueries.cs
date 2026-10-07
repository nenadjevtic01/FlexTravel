using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.Jobs;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.Application.Weather;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Jobs;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetWeatherQuery : EfUseCase, IGetWeatherQuery, ISkipAuditLog
    {
        private const int MaxDays = 21;
        private readonly IWeatherProvider _weather;

        public EfGetWeatherQuery(FlexTravelContext context, IWeatherProvider weather) : base(context)
        {
            _weather = weather;
        }

        public int UseCaseId => UseCaseIds.GetWeather;
        public string UseCaseName => "Get weather";
        public string UseCaseDescription => "Weather forecast or last year's weather for a destination";

        public WeatherDto Execute(WeatherRequestDto request)
        {
            var destination = Context.Destination
                .Where(d => d.DestinationId == request.DestinationId && d.IsActive)
                .Select(d => new { d.Latitude, d.Longitude })
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Destination", request.DestinationId);

            if (destination.Latitude == null || destination.Longitude == null)
            {
                return new WeatherDto { Available = false };
            }

            var today = DateTime.Today;
            var from = (request.From ?? today).Date;
            var to = (request.To ?? from.AddDays(6)).Date;
            if (to < from)
            {
                to = from;
            }

            if ((to - from).Days >= MaxDays)
            {
                to = from.AddDays(MaxDays - 1);
            }

            var lat = destination.Latitude.Value;
            var lng = destination.Longitude.Value;
            var forecastEnd = today.AddDays(_weather.ForecastDays - 1);

            string kind;
            IReadOnlyList<WeatherDayDto> days;
            if (from <= forecastEnd && to >= today)
            {
                kind = "Forecast";
                from = from < today ? today : from;
                to = to > forecastEnd ? forecastEnd : to;
                days = _weather.Forecast(lat, lng, from, to);
            }
            else if (from > forecastEnd)
            {
                kind = "LastYear";
                days = _weather.History(lat, lng, from.AddYears(-1), to.AddYears(-1));
            }
            else
            {
                kind = "Past";
                days = _weather.History(lat, lng, from, to);
            }

            if (days == null || days.Count == 0)
            {
                return new WeatherDto { Available = false, Kind = kind, From = from, To = to };
            }

            foreach (var day in days)
            {
                day.Summary = WeatherCodes.Describe(day.Code);
            }

            var highs = days.Where(d => d.High.HasValue).Select(d => d.High.Value).ToList();
            var lows = days.Where(d => d.Low.HasValue).Select(d => d.Low.Value).ToList();

            return new WeatherDto
            {
                Available = true,
                Kind = kind,
                From = from,
                To = to,
                AverageHigh = highs.Count == 0 ? null : Math.Round(highs.Average(), 1),
                AverageLow = lows.Count == 0 ? null : Math.Round(lows.Average(), 1),
                RainyDays = days.Count(d => d.Precipitation >= 1),
                Days = days
            };
        }
    }

    public static class WeatherCodes
    {
        public static string Describe(int? code) => code switch
        {
            null => "Unknown",
            0 => "Clear sky",
            1 => "Mainly clear",
            2 => "Partly cloudy",
            3 => "Overcast",
            45 or 48 => "Fog",
            51 or 53 or 55 or 56 or 57 => "Drizzle",
            61 or 63 or 66 => "Rain",
            65 or 67 => "Heavy rain",
            71 or 73 or 75 or 77 => "Snow",
            80 or 81 or 82 => "Rain showers",
            85 or 86 => "Snow showers",
            95 or 96 or 99 => "Thunderstorm",
            _ => "Mixed"
        };
    }

    public class EfGetMyPriceAlertsQuery : EfUseCase, IGetMyPriceAlertsQuery
    {
        private readonly IUser _user;
        private readonly PriceAlertService _priceAlerts;

        public EfGetMyPriceAlertsQuery(FlexTravelContext context, IUser user, PriceAlertService priceAlerts) : base(context)
        {
            _user = user;
            _priceAlerts = priceAlerts;
        }

        public int UseCaseId => UseCaseIds.GetMyPriceAlerts;
        public string UseCaseName => "Get my price alerts";
        public string UseCaseDescription => "Price alerts of the customer";

        public IEnumerable<PriceAlertDto> Execute(int request)
        {
            var alerts = Context.PriceAlert
                .Where(a => a.UserId == _user.Id)
                .OrderBy(a => a.Destination.DestinationName)
                .Select(a => new PriceAlertDto
                {
                    Id = a.PriceAlertId,
                    DestinationId = a.DestinationId,
                    Destination = a.Destination.DestinationName,
                    Country = a.Destination.Country.CountryName,
                    Image = a.Destination.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                    MaxPrice = a.MaxPrice,
                    CreatedAt = a.CreatedAt,
                    LastNotifiedAt = a.LastNotifiedAt
                })
                .ToList();

            foreach (var alert in alerts)
            {
                var cheapest = _priceAlerts.Cheapest(alert.DestinationId);
                alert.LowestPrice = cheapest?.PriceFrom;
                alert.LowestPriceTripId = cheapest?.Id;
            }

            return alerts;
        }
    }

    public class EfGetExchangeRatesQuery : EfUseCase, IGetExchangeRatesQuery, ISkipAuditLog
    {
        public EfGetExchangeRatesQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetExchangeRates;
        public string UseCaseName => "Get exchange rates";
        public string UseCaseDescription => "Rates for showing prices in other currencies";

        public IEnumerable<ExchangeRateDto> Execute(int request) => Context.ExchangeRate
            .OrderBy(r => r.Currency)
            .Select(r => new ExchangeRateDto
            {
                Currency = r.Currency,
                RatePerEuro = r.RatePerEuro,
                UpdatedAt = r.UpdatedAt,
                UpdatedBy = r.UpdatedBy == null ? null : r.UpdatedBy.FirstName + " " + r.UpdatedBy.LastName
            })
            .ToList();
    }

    public class EfGetAgencyCalendarQuery : EfUseCase, IGetAgencyCalendarQuery
    {
        private const int MaxDays = 62;
        private readonly CurrentAgency _agency;

        public EfGetAgencyCalendarQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetAgencyCalendar;
        public string UseCaseName => "Get agency calendar";
        public string UseCaseDescription => "Departures and payment deadlines of the agency by date";

        public AgencyCalendarDto Execute(AgencyCalendarRequestDto request)
        {
            var agencyId = _agency.Id(UseCaseName);
            var from = request.From == default ? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) : request.From.Date;
            var to = request.To == default || request.To.Date < from ? from.AddMonths(1).AddDays(-1) : request.To.Date;
            if ((to - from).Days >= MaxDays)
            {
                to = from.AddDays(MaxDays - 1);
            }

            var today = DateTime.Today;

            var departures = Context.Trip
                .Where(t => t.AgencyId == agencyId && t.StartDate <= to && t.EndDate >= from)
                .OrderBy(t => t.StartDate)
                .Select(t => new CalendarDepartureDto
                {
                    TripId = t.TripId,
                    Name = t.TripName,
                    Destination = t.Destination.DestinationName,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    Capacity = t.Capacity,
                    SeatsBooked = t.Reservations
                        .Where(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                        .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0,
                    Pending = t.Reservations.Count(r => r.IsActive && r.Status == ReservationStatus.Pending),
                    IsActive = t.IsActive
                })
                .ToList();

            var reservations = Context.Reservation
                .Where(r => r.IsActive && r.Trip.AgencyId == agencyId && r.Trip.StartDate >= from
                    && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed))
                .Select(r => new
                {
                    r.ReservationId,
                    r.Code,
                    r.Status,
                    r.CreatedAt,
                    r.TotalPrice,
                    r.CancellationFee,
                    Customer = r.User.FirstName + " " + r.User.LastName,
                    r.Trip.TripName,
                    r.Trip.StartDate,
                    Paid = r.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                    ConfirmedAt = r.StatusChanges
                        .Where(c => c.Status == ReservationStatus.Confirmed)
                        .Min(c => (DateTime?)c.ChangedAt)
                })
                .ToList();

            var deadlines = new List<CalendarDeadlineDto>();
            foreach (var r in reservations)
            {
                void Add(string kind, DateTime? date, decimal amount)
                {
                    if (date.HasValue && date.Value.Date >= from && date.Value.Date <= to)
                    {
                        deadlines.Add(new CalendarDeadlineDto
                        {
                            ReservationId = r.ReservationId,
                            Code = r.Code,
                            Customer = r.Customer,
                            TripName = r.TripName,
                            Kind = kind,
                            Date = date.Value.Date,
                            Amount = amount,
                            IsOverdue = date.Value.Date < today
                        });
                    }
                }

                if (r.Status == ReservationStatus.Pending)
                {
                    Add("Decision", PendingRules.DecideBy(r.CreatedAt, r.StartDate), r.TotalPrice);
                    continue;
                }

                var summary = PaymentRules.Summary(r.Status, r.TotalPrice, r.CancellationFee, r.StartDate, r.ConfirmedAt, r.Paid, today);
                if (r.Paid < summary.Deposit && summary.DepositDueDate != summary.BalanceDueDate)
                {
                    Add("Deposit", summary.DepositDueDate, summary.Deposit - r.Paid);
                }

                if (summary.Balance > 0)
                {
                    Add("Balance", summary.BalanceDueDate, summary.Balance);
                }
            }

            return new AgencyCalendarDto
            {
                From = from,
                To = to,
                Departures = departures,
                Deadlines = deadlines.OrderBy(d => d.Date).ThenBy(d => d.Kind).ToList()
            };
        }
    }

    public class EfGetAgencyReportQuery : EfUseCase, IGetAgencyReportQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetAgencyReportQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetAgencyReport;
        public string UseCaseName => "Get agency report";
        public string UseCaseDescription => "Sales, occupancy and destinations of the agency for a year";

        public AgencyReportDto Execute(int year)
        {
            var agencyId = _agency.Id(UseCaseName);
            year = year <= 0 ? DateTime.Today.Year : year;
            var start = new DateTime(year, 1, 1);
            var end = start.AddYears(1);

            var reservations = Context.Reservation
                .Where(r => r.IsActive && r.Trip.AgencyId == agencyId && r.CreatedAt >= start && r.CreatedAt < end)
                .Select(r => new
                {
                    r.CreatedAt,
                    r.Status,
                    r.TotalPrice,
                    r.ExtrasTotal,
                    r.CancellationFee,
                    Travelers = r.NumberOfAdults + r.NumberOfChildren,
                    WasConfirmed = r.StatusChanges.Any(c => c.Status == ReservationStatus.Confirmed),
                    Destination = r.Trip.Destination.DestinationName,
                    Country = r.Trip.Destination.Country.CountryName
                })
                .ToList();

            var payments = Context.Payment
                .Where(p => p.Reservation.Trip.AgencyId == agencyId && p.PaidAt >= start && p.PaidAt < end)
                .Select(p => new { p.PaidAt, p.Amount })
                .ToList();

            decimal SaleOf(ReservationStatus status, decimal total, decimal? fee) =>
                status == ReservationStatus.Confirmed ? total : status == ReservationStatus.Cancelled ? fee ?? 0 : 0;

            var confirmed = reservations.Where(r => r.Status == ReservationStatus.Confirmed).ToList();
            var decided = reservations.Count(r => r.Status != ReservationStatus.Pending);

            var departures = Context.Trip
                .Where(t => t.AgencyId == agencyId && t.StartDate >= start && t.StartDate < end)
                .OrderBy(t => t.StartDate)
                .Select(t => new
                {
                    t.TripId,
                    t.TripName,
                    t.StartDate,
                    t.Capacity,
                    Sold = t.Reservations
                        .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed)
                        .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0,
                    Sales = t.Reservations
                        .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed)
                        .Sum(r => (decimal?)r.TotalPrice) ?? 0
                })
                .ToList()
                .Select(t => new ReportDepartureDto
                {
                    TripId = t.TripId,
                    Name = t.TripName,
                    StartDate = t.StartDate,
                    Capacity = t.Capacity,
                    SeatsSold = t.Sold,
                    Occupancy = t.Capacity == 0 ? 0 : Math.Round(100.0 * t.Sold / t.Capacity, 1),
                    Sales = t.Sales
                })
                .ToList();

            var extras = Context.ReservationExtra
                .Where(e => e.Reservation.IsActive && e.Reservation.Status == ReservationStatus.Confirmed
                    && e.Reservation.Trip.AgencyId == agencyId
                    && e.Reservation.CreatedAt >= start && e.Reservation.CreatedAt < end)
                .Select(e => new { e.Name, e.Kind, e.Quantity, e.Total })
                .ToList()
                .GroupBy(e => new { e.Name, e.Kind })
                .Select(g => new ReportExtraDto
                {
                    Name = g.Key.Name,
                    Kind = g.Key.Kind.ToString(),
                    Sold = g.Sum(x => x.Quantity),
                    Sales = g.Sum(x => x.Total)
                })
                .OrderByDescending(e => e.Sales)
                .ToList();

            var years = Context.Reservation
                .Where(r => r.Trip.AgencyId == agencyId)
                .Select(r => r.CreatedAt.Year)
                .Distinct()
                .ToList()
                .Append(DateTime.Today.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            return new AgencyReportDto
            {
                Year = year,
                Years = years,
                Totals = new AgencyReportTotalsDto
                {
                    Reservations = reservations.Count,
                    Confirmed = confirmed.Count,
                    Pending = reservations.Count(r => r.Status == ReservationStatus.Pending),
                    Cancelled = reservations.Count(r => r.Status == ReservationStatus.Cancelled),
                    Rejected = reservations.Count(r => r.Status == ReservationStatus.Rejected),
                    Expired = reservations.Count(r => r.Status == ReservationStatus.Expired),
                    Travelers = confirmed.Sum(r => r.Travelers),
                    Sales = reservations.Sum(r => SaleOf(r.Status, r.TotalPrice, r.CancellationFee)),
                    Collected = payments.Sum(p => p.Amount),
                    AverageReservation = confirmed.Count == 0 ? 0 : Math.Round(confirmed.Average(r => r.TotalPrice), 2),
                    ExtrasSales = confirmed.Sum(r => r.ExtrasTotal),
                    ConfirmationRate = decided == 0 ? 0 : Math.Round(100.0 * reservations.Count(r => r.WasConfirmed) / decided, 1),
                    CancellationRate = reservations.Count == 0 ? 0
                        : Math.Round(100.0 * reservations.Count(r => r.Status == ReservationStatus.Cancelled) / reservations.Count, 1),
                    AverageOccupancy = departures.Count == 0 ? 0 : Math.Round(departures.Average(d => d.Occupancy), 1)
                },
                Months = Enumerable.Range(1, 12).Select(month => new ReportMonthDto
                {
                    Month = month,
                    Reservations = reservations.Count(r => r.CreatedAt.Month == month),
                    Travelers = confirmed.Where(r => r.CreatedAt.Month == month).Sum(r => r.Travelers),
                    Sales = reservations.Where(r => r.CreatedAt.Month == month).Sum(r => SaleOf(r.Status, r.TotalPrice, r.CancellationFee)),
                    Collected = payments.Where(p => p.PaidAt.Month == month).Sum(p => p.Amount)
                }).ToList(),
                Departures = departures,
                Destinations = confirmed
                    .GroupBy(r => new { r.Destination, r.Country })
                    .Select(g => new ReportDestinationDto
                    {
                        Destination = g.Key.Destination,
                        Country = g.Key.Country,
                        Reservations = g.Count(),
                        Travelers = g.Sum(r => r.Travelers),
                        Sales = g.Sum(r => r.TotalPrice)
                    })
                    .OrderByDescending(d => d.Sales)
                    .Take(8)
                    .ToList(),
                Extras = extras
            };
        }
    }

    public class EfGetAgencyPageQuery : EfUseCase, IGetAgencyPageQuery
    {
        private readonly ISearchTripsQuery _search;

        public EfGetAgencyPageQuery(FlexTravelContext context, ISearchTripsQuery search) : base(context)
        {
            _search = search;
        }

        public int UseCaseId => UseCaseIds.GetAgencyPage;
        public string UseCaseName => "Get agency page";
        public string UseCaseDescription => "Public page of an agency with its trips and reviews";

        public AgencyPageDto Execute(int id)
        {
            var agency = Context.Agency
                .Where(a => a.AgencyId == id)
                .Select(a => new AgencyPageDto
                {
                    Id = a.AgencyId,
                    Name = a.AgencyName,
                    Description = a.Description,
                    Address = a.Address,
                    Phone = a.Phone,
                    Email = a.Email,
                    Website = a.Website,
                    Logo = a.Logo,
                    DestinationCount = a.Trips.Select(t => t.DestinationId).Distinct().Count()
                })
                .FirstOrDefault()
                ?? throw new EntityNotFoundException("Agency", id);

            var today = DateTime.Today;
            agency.TravelersServed = Context.Reservation
                .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed && r.Trip.AgencyId == id && r.Trip.EndDate < today)
                .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0;
            agency.Rating = Ratings.Breakdown(Context.Review.Where(r => r.Trip.AgencyId == id));
            agency.Trips = _search.Execute(new TripSearchDto { AgencyId = id, PerPage = 48 }).Items;

            return agency;
        }
    }

    public class EfGetScheduledJobRunsQuery : EfUseCase, IGetScheduledJobRunsQuery, ISkipAuditLog
    {
        private readonly IJobSchedule _schedule;

        public EfGetScheduledJobRunsQuery(FlexTravelContext context, IJobSchedule schedule) : base(context)
        {
            _schedule = schedule;
        }

        public int UseCaseId => UseCaseIds.GetScheduledJobRuns;
        public string UseCaseName => "Get scheduled job runs";
        public string UseCaseDescription => "Latest runs of the background jobs";

        public ScheduledJobsOverviewDto Execute(int request) => new ScheduledJobsOverviewDto
        {
            IntervalMinutes = _schedule.IntervalMinutes,
            NextRunAt = _schedule.NextRunAt,
            Runs = Context.ScheduledJobRun
                .OrderByDescending(r => r.StartedAt)
                .Take(15)
                .ToList()
                .Select(ScheduledJobs.ToDto)
                .ToList()
        };
    }
}
