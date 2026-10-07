namespace FlexTravel.Domain
{
    public class Climate
    {
        public int ClimateId { get; set; }
        public int DestinationId { get; set; }
        public string ClimateInfo { get; set; }

        public virtual Destination Destination { get; set; }
    }
}