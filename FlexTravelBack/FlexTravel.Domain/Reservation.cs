namespace FlexTravel.Domain
{
    public class Reservation : Entity
    {
        public int ReservationId { get; set; }
        public string Code { get; set; }
        public int UserId { get; set; }
        public int TripId { get; set; }
        public int AccommodationId { get; set; }
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public DateTime? StatusChangedAt { get; set; }
        public string Note { get; set; }

        public decimal PricePerPerson { get; set; }
        public int ChildDiscountPercent { get; set; }

        public decimal Subtotal { get; set; }
        public TripOffer Offer { get; set; }
        public int OfferPercent { get; set; }
        public int? PromoCodeId { get; set; }
        public int PromoPercent { get; set; }
        public decimal DiscountTotal { get; set; }

        public decimal ExtrasTotal { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal? CancellationFee { get; set; }

        public DateTime? DepositReminderSentAt { get; set; }
        public DateTime? BalanceReminderSentAt { get; set; }
        public DateTime? ReviewInvitationSentAt { get; set; }

        public virtual RegularUser User { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        public virtual Review Review { get; set; }

        public virtual ICollection<ReservationRoom> Rooms { get; set; } = new List<ReservationRoom>();
        public virtual ICollection<ReservationStatusChange> StatusChanges { get; set; } = new List<ReservationStatusChange>();
        public virtual ICollection<ReservationTraveler> Travelers { get; set; } = new List<ReservationTraveler>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<ReservationMessage> Messages { get; set; } = new List<ReservationMessage>();
        public virtual ICollection<ReservationExtra> Extras { get; set; } = new List<ReservationExtra>();
    }
}
