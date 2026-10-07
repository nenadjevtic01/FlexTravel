namespace FlexTravel.Domain
{
    public class AttractionCategory
    {
        public int AttractionCategoryId { get; set; }
        public string AttractionCategoryName { get; set; }

        public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    }
}
