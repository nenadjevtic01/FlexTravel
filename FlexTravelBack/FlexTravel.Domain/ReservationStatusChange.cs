namespace FlexTravel.Domain
{
    public class ReservationStatusChange
    {
        public int ReservationStatusChangeId { get; set; }
        public int ReservationId { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }

        public int? ChangedByUserId { get; set; }

        public string Comment { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual User ChangedBy { get; set; }
    }
}
