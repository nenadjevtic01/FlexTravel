namespace FlexTravel.Application.Dto
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContinentId { get; set; }
        public string Continent { get; set; }
        public string CapitalCity { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public long? Population { get; set; }
        public int? TotalArea { get; set; }
        public int DestinationsCount { get; set; }
        public VisaDto Visa { get; set; }
        public SafetyDto Safety { get; set; }
    }

    public class VisaDto
    {
        public string Type { get; set; }
        public string Duration { get; set; }
        public string Requirements { get; set; }
    }

    public class SafetyDto
    {
        public int Level { get; set; }
        public string Advices { get; set; }
    }

    public class CountrySearchDto
    {
        public int? ContinentId { get; set; }
    }
}
