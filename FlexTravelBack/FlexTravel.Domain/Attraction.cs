namespace FlexTravel.Domain
{
    public class Attraction
    {
        public int AttractionId { get; set; }
        public int DestinationId { get; set; }
        public string AttractionName { get; set; }
        public string AttractionDescription { get; set; }
        public int AttractionCategoryId { get; set; }

        public virtual AttractionCategory AttractionCategory { get; set; }
        public virtual Destination Destination { get; set; }

        public virtual ICollection<AttractionImage> AttractionImages { get; set; } = new List<AttractionImage>();
        public virtual ICollection<TripAttraction> Trips { get; set; } = new List<TripAttraction>();
    }
}
