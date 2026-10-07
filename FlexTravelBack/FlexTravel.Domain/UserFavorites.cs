namespace FlexTravel.Domain
{
    public class UserFavorites
    {
        public int UserId { get; set; }
        public int DestinationId { get; set; }

        public virtual RegularUser User { get; set; }
        public virtual Destination Destination { get; set; }
    }
}
