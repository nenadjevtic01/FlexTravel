namespace FlexTravel.Application.Dto
{
    public class BookingRequestDto
    {
        public int TripId { get; set; }
        public int AccommodationId { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public List<BookingRoomDto> Rooms { get; set; } = new List<BookingRoomDto>();
        public string Note { get; set; }
        public string PromoCode { get; set; }
        public List<BookingExtraDto> Extras { get; set; } = new List<BookingExtraDto>();

        public List<TravelerDto> Travelers { get; set; } = new List<TravelerDto>();
    }

    public class TravelerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsChild { get; set; }
    }

    public class UpdateTravelersDto
    {
        public int ReservationId { get; set; }
        public List<TravelerDto> Travelers { get; set; } = new List<TravelerDto>();
    }

    public class BookingRoomDto
    {
        public int RoomId { get; set; }
        public int Count { get; set; }
    }

    public class PriceQuoteDto
    {
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Nights { get; set; }
        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }
        public decimal ChildPrice { get; set; }
        public decimal TravelersTotal { get; set; }
        public IEnumerable<PriceQuoteRoomDto> Rooms { get; set; } = new List<PriceQuoteRoomDto>();
        public decimal RoomsTotal { get; set; }

        public decimal Subtotal { get; set; }
        public string Offer { get; set; }
        public int OfferPercent { get; set; }
        public decimal OfferDiscount { get; set; }
        public string PromoCode { get; set; }
        public int PromoPercent { get; set; }
        public decimal PromoDiscount { get; set; }

        public string PromoCodeError { get; set; }

        public IEnumerable<PriceQuoteExtraDto> Extras { get; set; } = new List<PriceQuoteExtraDto>();
        public decimal ExtrasTotal { get; set; }
        public decimal Total { get; set; }
        public int Beds { get; set; }
        public int SeatsLeft { get; set; }

        public List<string> Problems { get; set; } = new List<string>();
        public bool IsValid => Problems.Count == 0;
    }

    public class PriceQuoteRoomDto
    {
        public int RoomId { get; set; }
        public string RoomType { get; set; }
        public int Count { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal Total { get; set; }
    }

    public class CreatedReservationDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StatusChangedAt { get; set; }
        public int TripId { get; set; }
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Destination { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
        public string Agency { get; set; }
        public string Accommodation { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }
        public decimal Subtotal { get; set; }
        public string Offer { get; set; }
        public int OfferPercent { get; set; }
        public string PromoCode { get; set; }
        public int PromoPercent { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal ExtrasTotal { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public IEnumerable<PriceQuoteRoomDto> Rooms { get; set; } = new List<PriceQuoteRoomDto>();
        public IEnumerable<PriceQuoteExtraDto> Extras { get; set; } = new List<PriceQuoteExtraDto>();
        public bool CanCancel { get; set; }

        public DateTime? DecideBy { get; set; }

        public decimal? CancellationFee { get; set; }
        public PaymentSummaryDto Payment { get; set; }
        public int UnreadMessages { get; set; }

        public bool CanReview { get; set; }
        public ReservationReviewDto Review { get; set; }
        public ReservationCustomerDto Customer { get; set; }

        public string HandledBy { get; set; }
        public DateTime? HandledAt { get; set; }
    }

    public class ReservationDetailDto : ReservationDto
    {
        public int Nights { get; set; }
        public decimal ChildPrice { get; set; }
        public decimal AdultsTotal { get; set; }
        public decimal ChildrenTotal { get; set; }
        public decimal RoomsTotal { get; set; }
        public decimal OfferDiscount { get; set; }
        public decimal PromoDiscount { get; set; }
        public string AgencyAddress { get; set; }
        public string AgencyPhone { get; set; }
        public DateTime FreeCancellationUntil { get; set; }

        public int CancellationFeePercentToday { get; set; }
        public decimal CancellationFeeToday { get; set; }
        public IEnumerable<CancellationTierDto> CancellationPolicy { get; set; } = new List<CancellationTierDto>();
        public bool CanDecide { get; set; }
        public bool CanRecordPayment { get; set; }
        public bool CanEditTravelers { get; set; }
        public IEnumerable<TravelerDto> Travelers { get; set; } = new List<TravelerDto>();
        public IEnumerable<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public IEnumerable<ReservationHistoryDto> History { get; set; } = new List<ReservationHistoryDto>();
    }

    public class ReservationHistoryDto
    {
        public string Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; }
        public string Role { get; set; }
        public string Comment { get; set; }
    }

    public class ReservationCustomerDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class ReservationSearchDto : PagedSearch
    {
        public string Status { get; set; }
        public int? TripId { get; set; }
    }

    public class DecideReservationDto
    {
        public int ReservationId { get; set; }
        public bool Confirm { get; set; }
        public string Reason { get; set; }
    }

    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
    }
}
