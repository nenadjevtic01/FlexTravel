namespace FlexTravel.Domain
{
    public class PromoCode : Entity
    {
        public int PromoCodeId { get; set; }
        public int AgencyId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public int? MaxUses { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
