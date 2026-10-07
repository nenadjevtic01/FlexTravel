namespace FlexTravel.Domain
{
    public class TripExtra
    {
        public int TripExtraId { get; set; }
        public int TripId { get; set; }
        public ExtraKind Kind { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool PerPerson { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Trip Trip { get; set; }
    }

    public enum ExtraKind
    {
        Insurance = 1,
        Transfer = 2,
        Excursion = 3,
        Other = 4
    }

    public class ReservationExtra
    {
        public int ReservationExtraId { get; set; }
        public int ReservationId { get; set; }
        public int TripExtraId { get; set; }
        public ExtraKind Kind { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public virtual Reservation Reservation { get; set; }
        public virtual TripExtra TripExtra { get; set; }
    }
}
