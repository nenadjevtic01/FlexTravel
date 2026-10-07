namespace FlexTravel.Domain
{
    public class TripPrice
    {
        public int TripPriceId { get; set; }
        public int TripId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }

        public virtual Trip Trip { get; set; }
    }
}
