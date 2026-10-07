namespace FlexTravel.Domain
{
    public class Review : Entity
    {
        public int ReviewId { get; set; }
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int TripId { get; set; }
        public int AccommodationId { get; set; }

        public int TripRating { get; set; }
        public int HotelRating { get; set; }

        public int? GuideRating { get; set; }
        public int? OrganisationRating { get; set; }
        public int? ValueRating { get; set; }
        public string Comment { get; set; }

        public string AgencyReply { get; set; }
        public DateTime? AgencyReplyAt { get; set; }
        public int? AgencyReplyByUserId { get; set; }

        public string ModerationNote { get; set; }
        public DateTime? ModeratedAt { get; set; }
        public int? ModeratedByUserId { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual RegularUser User { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }
        public virtual User AgencyReplyBy { get; set; }
        public virtual User ModeratedBy { get; set; }
    }
}
