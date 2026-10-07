namespace FlexTravel.Domain
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CapitalCity { get; set; }
        public long? Population { get; set; }
        public int? TotalArea { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public int ContinentId { get; set; }

        public virtual Safety Safety { get; set; }
        public virtual Visa VisaInfo { get; set; }
        public virtual Continent Continent { get; set; }

        public virtual ICollection<Destination> Destination { get; set; } = new List<Destination>();
    }
}
