namespace FlexTravel.Domain
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaidAt { get; set; }
        public int RecordedByUserId { get; set; }
        public string Note { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual User RecordedBy { get; set; }
    }

    public enum PaymentMethod
    {
        Cash = 1,
        Card = 2,
        BankTransfer = 3
    }
}
