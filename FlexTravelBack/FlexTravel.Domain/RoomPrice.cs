namespace FlexTravel.Domain
{
    public class RoomPrice
    {
        public int RoomPriceId { get; set; }
        public int RoomId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }

        public virtual Room Room { get; set; }
    }
}
