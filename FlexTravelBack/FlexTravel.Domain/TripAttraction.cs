namespace FlexTravel.Domain
{
    public class TripAttraction
    {
        public int TripId { get; set; }
        public int AttractionId { get; set; }

        public virtual Attraction Attraction { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
