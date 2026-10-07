namespace FlexTravel.Domain
{
    public class TripAccommodation
    {
        public int TripId { get; set; }
        public int AccommodationId { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual Accommodation Accommodation { get; set; }
    }
}
