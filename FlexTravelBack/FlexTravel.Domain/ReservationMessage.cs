namespace FlexTravel.Domain
{
    public class ReservationMessage
    {
        public int ReservationMessageId { get; set; }
        public int ReservationId { get; set; }
        public int SenderUserId { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public DateTime? ReadAt { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual User Sender { get; set; }
    }
}
