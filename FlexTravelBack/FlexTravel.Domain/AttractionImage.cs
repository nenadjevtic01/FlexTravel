namespace FlexTravel.Domain
{
    public class AttractionImage
    {
        public int AttractionImageId { get; set; }
        public int AttractionId { get; set; }
        public string Src { get; set; }
        public string Alt { get; set; }

        public virtual Attraction Attraction { get; set; }

    }
}
