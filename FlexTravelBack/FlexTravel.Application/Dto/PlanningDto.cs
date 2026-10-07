namespace FlexTravel.Application.Dto
{
    public class TripExtraDto
    {
        public int Id { get; set; }

        public string Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool PerPerson { get; set; }
    }

    public class TripExtraInputDto
    {
        public int? Id { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool PerPerson { get; set; }
    }

    public class BookingExtraDto
    {
        public int ExtraId { get; set; }

        public int Quantity { get; set; }
    }

    public class PriceQuoteExtraDto
    {
        public int ExtraId { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class ItineraryDayInputDto
    {
        public string Title { get; set; }
        public string Activity { get; set; }
        public bool Breakfast { get; set; }
        public bool Lunch { get; set; }
        public bool Dinner { get; set; }
        public string Overnight { get; set; }
    }

    public class RatingBreakdownDto
    {
        public double? Average { get; set; }
        public int Count { get; set; }

        public int[] Stars { get; set; } = new int[5];
        public IEnumerable<RatingCategoryDto> Categories { get; set; } = new List<RatingCategoryDto>();
    }

    public class RatingCategoryDto
    {
        public string Name { get; set; }
        public double? Average { get; set; }
        public int Count { get; set; }
    }

    public class PriceAlertDto
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string Destination { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal? LowestPrice { get; set; }
        public int? LowestPriceTripId { get; set; }
        public bool IsMet => LowestPrice.HasValue && LowestPrice.Value <= MaxPrice;
        public DateTime? LastNotifiedAt { get; set; }
    }

    public class SetPriceAlertDto
    {
        public int DestinationId { get; set; }
        public decimal? MaxPrice { get; set; }
    }

    public class ExchangeRateDto
    {
        public string Currency { get; set; }
        public decimal RatePerEuro { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class SetExchangeRateDto
    {
        public string Currency { get; set; }
        public decimal RatePerEuro { get; set; }
    }

    public class WeatherRequestDto
    {
        public int DestinationId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public class WeatherDto
    {
        public bool Available { get; set; }

        public string Kind { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public double? AverageHigh { get; set; }
        public double? AverageLow { get; set; }
        public int RainyDays { get; set; }
        public IEnumerable<WeatherDayDto> Days { get; set; } = new List<WeatherDayDto>();
        public string Source { get; set; } = "Open-Meteo";
    }

    public class WeatherDayDto
    {
        public DateTime Date { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public double? Precipitation { get; set; }

        public int? Code { get; set; }
        public string Summary { get; set; }
    }

    public class AgencyCalendarRequestDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class AgencyCalendarDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IEnumerable<CalendarDepartureDto> Departures { get; set; } = new List<CalendarDepartureDto>();
        public IEnumerable<CalendarDeadlineDto> Deadlines { get; set; } = new List<CalendarDeadlineDto>();
    }

    public class CalendarDepartureDto
    {
        public int TripId { get; set; }
        public string Name { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int SeatsBooked { get; set; }
        public int Pending { get; set; }
        public bool IsActive { get; set; }
    }

    public class CalendarDeadlineDto
    {
        public int ReservationId { get; set; }
        public string Code { get; set; }
        public string Customer { get; set; }
        public string TripName { get; set; }

        public string Kind { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class AgencyReportDto
    {
        public int Year { get; set; }
        public IEnumerable<int> Years { get; set; } = new List<int>();
        public AgencyReportTotalsDto Totals { get; set; }
        public IEnumerable<ReportMonthDto> Months { get; set; } = new List<ReportMonthDto>();
        public IEnumerable<ReportDepartureDto> Departures { get; set; } = new List<ReportDepartureDto>();
        public IEnumerable<ReportDestinationDto> Destinations { get; set; } = new List<ReportDestinationDto>();
        public IEnumerable<ReportExtraDto> Extras { get; set; } = new List<ReportExtraDto>();
    }

    public class AgencyReportTotalsDto
    {
        public int Reservations { get; set; }
        public int Confirmed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
        public int Rejected { get; set; }
        public int Expired { get; set; }
        public int Travelers { get; set; }

        public decimal Sales { get; set; }
        public decimal Collected { get; set; }
        public decimal AverageReservation { get; set; }
        public decimal ExtrasSales { get; set; }

        public double ConfirmationRate { get; set; }
        public double CancellationRate { get; set; }
        public double AverageOccupancy { get; set; }
    }

    public class ReportMonthDto
    {
        public int Month { get; set; }
        public int Reservations { get; set; }
        public int Travelers { get; set; }
        public decimal Sales { get; set; }
        public decimal Collected { get; set; }
    }

    public class ReportDepartureDto
    {
        public int TripId { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public int Capacity { get; set; }
        public int SeatsSold { get; set; }
        public double Occupancy { get; set; }
        public decimal Sales { get; set; }
    }

    public class ReportDestinationDto
    {
        public string Destination { get; set; }
        public string Country { get; set; }
        public int Reservations { get; set; }
        public int Travelers { get; set; }
        public decimal Sales { get; set; }
    }

    public class ReportExtraDto
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public int Sold { get; set; }
        public decimal Sales { get; set; }
    }

    public class AgencyPageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public RatingBreakdownDto Rating { get; set; }
        public int TravelersServed { get; set; }
        public int DestinationCount { get; set; }
        public IEnumerable<TripListDto> Trips { get; set; } = new List<TripListDto>();
    }

    public class UpdateAgencyProfileDto
    {
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
    }

    public class ScheduledJobRunDto
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string StartedBy { get; set; }
        public int ExpiredReservations { get; set; }
        public int PaymentReminders { get; set; }
        public int ReviewInvitations { get; set; }
        public int PriceAlerts { get; set; }
        public int ClosedWaitlists { get; set; }
        public string Error { get; set; }
    }

    public class ScheduledJobsOverviewDto
    {
        public int IntervalMinutes { get; set; }
        public DateTime? NextRunAt { get; set; }
        public IEnumerable<ScheduledJobRunDto> Runs { get; set; } = new List<ScheduledJobRunDto>();
    }
}
