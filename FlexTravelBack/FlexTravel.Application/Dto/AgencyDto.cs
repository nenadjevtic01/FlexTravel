namespace FlexTravel.Application.Dto
{
    public class AgencyTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int SeatsBooked { get; set; }
        public int PendingReservations { get; set; }

        public int Waiting { get; set; }
        public decimal PricePerPerson { get; set; }
        public string Offer { get; set; }
        public int OfferPercent { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AgencyOverviewDto
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public RatingDto Rating { get; set; }
        public int UnansweredReviews { get; set; }
        public IEnumerable<AgencyTripDto> Trips { get; set; } = new List<AgencyTripDto>();
        public IEnumerable<AgencyTeamMemberDto> Team { get; set; } = new List<AgencyTeamMemberDto>();
    }

    public class AgencyTeamMemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public bool IsActive { get; set; }
        public int TripsCreated { get; set; }
        public int ReservationsHandled { get; set; }
    }

    public class UpdateTripDto : CreateTripDto
    {
        public int Id { get; set; }
    }

    public class TripForEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DestinationId { get; set; }
        public string Destination { get; set; }
        public int TransportId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }
        public int EarlyBookingPercent { get; set; }
        public DateTime? EarlyBookingUntil { get; set; }
        public int LastMinutePercent { get; set; }
        public int LastMinuteDays { get; set; }
        public List<int> AccommodationIds { get; set; } = new List<int>();
        public List<int> AttractionIds { get; set; } = new List<int>();
        public List<ItineraryDayInputDto> Itinerary { get; set; } = new List<ItineraryDayInputDto>();
        public List<TripExtraInputDto> Extras { get; set; } = new List<TripExtraInputDto>();
        public bool IsActive { get; set; }

        public int SeatsBooked { get; set; }
        public int ActiveReservations { get; set; }
        public List<int> LockedAccommodationIds { get; set; } = new List<int>();

        public IEnumerable<TripPriceHistoryDto> PriceHistory { get; set; } = new List<TripPriceHistoryDto>();
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TripPriceHistoryDto
    {
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
    }

    public class TripDestinationLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int Trips { get; set; }
    }

    public class CreateTripDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int DestinationId { get; set; }
        public int TransportId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }

        public int EarlyBookingPercent { get; set; }
        public DateTime? EarlyBookingUntil { get; set; }
        public int LastMinutePercent { get; set; }
        public int LastMinuteDays { get; set; }

        public List<int> AccommodationIds { get; set; } = new List<int>();

        public List<int> AttractionIds { get; set; } = new List<int>();

        public List<ItineraryDayInputDto> Itinerary { get; set; } = new List<ItineraryDayInputDto>();

        public List<TripExtraInputDto> Extras { get; set; } = new List<TripExtraInputDto>();
    }

    public class TripFormOptionsDto
    {
        public IEnumerable<LookupDto> Transports { get; set; } = new List<LookupDto>();
        public IEnumerable<TripFormDestinationDto> Destinations { get; set; } = new List<TripFormDestinationDto>();
    }

    public class LookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TripFormDestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public IEnumerable<TripFormAccommodationDto> Accommodations { get; set; } = new List<TripFormAccommodationDto>();
        public IEnumerable<AttractionDto> Attractions { get; set; } = new List<AttractionDto>();
    }

    public class TripFormAccommodationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Stars { get; set; }
        public int Rooms { get; set; }
    }
}
