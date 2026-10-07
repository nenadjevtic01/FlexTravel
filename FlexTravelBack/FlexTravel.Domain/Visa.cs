namespace FlexTravel.Domain
{
    public class Visa
    {
        public int VisaId { get; set; }
        public string VisaType { get; set; }
        public string Requirements { get; set; }
        public string Duration { get; set; }
        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
