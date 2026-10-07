namespace FlexTravel.Application.Dto
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class NotificationsDto
    {
        public int UnreadCount { get; set; }
        public IEnumerable<NotificationDto> Items { get; set; } = new List<NotificationDto>();
    }

    public class MarkNotificationsReadDto
    {
        public List<int> Ids { get; set; } = new List<int>();
    }

    public class FavoriteDestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Image { get; set; }
        public int UpcomingTrips { get; set; }
        public DateTime? NextDeparture { get; set; }
    }

    public class SetFavoriteDto
    {
        public int DestinationId { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class ReviewSearchDto : PagedSearch
    {
        public int? TripId { get; set; }
        public int? DestinationId { get; set; }
        public int? AgencyId { get; set; }
        public int? AccommodationId { get; set; }

        public bool Unanswered { get; set; }

        public bool IncludeHidden { get; set; }

        public bool HiddenOnly { get; set; }
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int TripId { get; set; }
        public string TripName { get; set; }
        public DateTime TripStartDate { get; set; }
        public int AgencyId { get; set; }
        public string Agency { get; set; }
        public int DestinationId { get; set; }
        public string Destination { get; set; }
        public int HotelId { get; set; }
        public string Hotel { get; set; }

        public string Author { get; set; }
        public string AuthorAvatar { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int TripRating { get; set; }
        public int HotelRating { get; set; }
        public int? GuideRating { get; set; }
        public int? OrganisationRating { get; set; }
        public int? ValueRating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AgencyReply { get; set; }
        public string AgencyReplyBy { get; set; }
        public DateTime? AgencyReplyAt { get; set; }

        public bool IsHidden { get; set; }
        public string ModerationNote { get; set; }
        public string ModeratedBy { get; set; }
        public DateTime? ModeratedAt { get; set; }
    }

    public class ReviewPageDto : PagedResponse<ReviewDto>
    {
        public double? AverageTripRating { get; set; }
        public double? AverageHotelRating { get; set; }
    }

    public class ReservationReviewDto
    {
        public int Id { get; set; }
        public int TripRating { get; set; }
        public int HotelRating { get; set; }
        public int? GuideRating { get; set; }
        public int? OrganisationRating { get; set; }
        public int? ValueRating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AgencyReply { get; set; }
        public bool IsHidden { get; set; }
    }

    public class CreateReviewDto
    {
        public int ReservationId { get; set; }
        public int TripRating { get; set; }
        public int HotelRating { get; set; }

        public int? GuideRating { get; set; }
        public int? OrganisationRating { get; set; }
        public int? ValueRating { get; set; }
        public string Comment { get; set; }
    }

    public class ReplyToReviewDto
    {
        public int ReviewId { get; set; }
        public string Reply { get; set; }
    }

    public class ModerateReviewDto
    {
        public int ReviewId { get; set; }
        public bool Visible { get; set; }
        public string Note { get; set; }
    }

    public class RatingDto
    {
        public double? Average { get; set; }
        public int Count { get; set; }
    }

    public class JoinWaitlistDto
    {
        public int TripId { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
    }

    public class WaitlistEntryDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string TripName { get; set; }
        public string Destination { get; set; }
        public string Image { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public DateTime JoinedAt { get; set; }

        public int Position { get; set; }
        public int SeatsLeft { get; set; }
        public bool CanBookNow { get; set; }
        public DateTime? NotifiedAt { get; set; }
    }

    public class PromoCodeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
        public int Uses { get; set; }
        public decimal DiscountGiven { get; set; }
        public bool IsActive { get; set; }

        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePromoCodeDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? MaxUses { get; set; }
    }

    public class SetPromoCodeActiveDto
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class AttractionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
