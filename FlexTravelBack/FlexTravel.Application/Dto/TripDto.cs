namespace FlexTravel.Application.Dto
{
    public class TripSearchDto : PagedSearch
    {
        public string Keyword { get; set; }
        public int? AgencyId { get; set; }
        public int? DestinationId { get; set; }
        public int? CountryId { get; set; }
        public int? ContinentId { get; set; }
        public int? TransportId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Travelers { get; set; }

        public bool Deals { get; set; }

        public string Sort { get; set; }
    }

    public class TripListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DestinationId { get; set; }
        public string Destination { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
        public string Agency { get; set; }
        public string Transport { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PricePerPerson { get; set; }

        public decimal PriceFrom { get; set; }
        public decimal PriceFromBeforeDiscount { get; set; }
        public string Offer { get; set; }
        public int OfferPercent { get; set; }

        public double? Rating { get; set; }
        public int ReviewCount { get; set; }

        public int OtherDepartures { get; set; }
    }

    public class TripDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }
        public string Transport { get; set; }
        public int FreeCancellationDays { get; set; }
        public DateTime FreeCancellationUntil { get; set; }
        public IEnumerable<CancellationTierDto> CancellationPolicy { get; set; } = new List<CancellationTierDto>();

        public string Offer { get; set; }
        public int OfferPercent { get; set; }
        public int EarlyBookingPercent { get; set; }
        public DateTime? EarlyBookingUntil { get; set; }
        public int LastMinutePercent { get; set; }
        public int LastMinuteDays { get; set; }

        public RatingDto Rating { get; set; }
        public RatingBreakdownDto RatingDetails { get; set; }
        public IEnumerable<AttractionDto> Attractions { get; set; } = new List<AttractionDto>();
        public IEnumerable<TripExtraDto> Extras { get; set; } = new List<TripExtraDto>();
        public TripAgencyDto Agency { get; set; }
        public TripDestinationDto Destination { get; set; }
        public IEnumerable<ItineraryDayDto> Itinerary { get; set; } = new List<ItineraryDayDto>();
        public IEnumerable<TripAccommodationDto> Accommodations { get; set; } = new List<TripAccommodationDto>();
    }

    public class TripAgencyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class TripDestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public IEnumerable<ImageDto> Images { get; set; } = new List<ImageDto>();
    }

    public class ItineraryDayDto
    {
        public int Day { get; set; }
        public string Title { get; set; }
        public string Activity { get; set; }
        public bool Breakfast { get; set; }
        public bool Lunch { get; set; }
        public bool Dinner { get; set; }
        public string Overnight { get; set; }
    }

    public class TripAccommodationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Stars { get; set; }
        public RatingDto Rating { get; set; }
        public IEnumerable<TripRoomDto> Rooms { get; set; } = new List<TripRoomDto>();
    }

    public class TripRoomDto
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public bool AC { get; set; }
        public bool Balcony { get; set; }
        public decimal PricePerNight { get; set; }
        public int Available { get; set; }
    }
}
