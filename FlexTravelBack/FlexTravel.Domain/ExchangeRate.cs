namespace FlexTravel.Domain
{
    public class ExchangeRate
    {
        public int ExchangeRateId { get; set; }
        public string Currency { get; set; }
        public decimal RatePerEuro { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }

        public virtual User UpdatedBy { get; set; }
    }
}
