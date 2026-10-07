namespace FlexTravel.Domain
{
    public class PriceAlert
    {
        public int PriceAlertId { get; set; }
        public int UserId { get; set; }
        public int DestinationId { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? LastNotifiedPrice { get; set; }
        public DateTime? LastNotifiedAt { get; set; }

        public virtual RegularUser User { get; set; }
        public virtual Destination Destination { get; set; }
    }
}
