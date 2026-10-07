namespace FlexTravel.Domain
{
    public class DestinationImage
    {
        public int DestinationImageId { get; set; }
        public int DestinationId { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }
        public virtual Destination Destination { get; set; }
    }
}
