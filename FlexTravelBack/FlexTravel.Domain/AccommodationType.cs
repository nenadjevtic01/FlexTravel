namespace FlexTravel.Domain
{
    public class AccommodationType
    {
        public int AccommodationTypeId { get; set; }
        public string AccommodationTypeName { get; set; }

        public virtual ICollection<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
    }
}
