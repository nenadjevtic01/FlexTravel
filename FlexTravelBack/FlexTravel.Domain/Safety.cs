namespace FlexTravel.Domain
{
    public class Safety
    {
        public int SafetyId { get; set; }
        public int CountryId { get; set; }
        public int SafetyLevel { get; set; }
        public string Advices { get; set; }

        public virtual Country Country { get; set; }
    }
}
