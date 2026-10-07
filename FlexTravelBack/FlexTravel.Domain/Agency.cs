namespace FlexTravel.Domain
{
    public class Agency
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public string Description { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }

        public virtual ICollection<AgencyEmployee> Employees { get; set; } = new List<AgencyEmployee>();
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
        public virtual ICollection<TripSeries> TripSeries { get; set; } = new List<TripSeries>();
    }
}
