namespace FlexTravel.Domain
{
    public class Trip : Entity
    {
        public int TripId { get; set; }
        public int TransportId { get; set; }
        public int ItineraryId { get; set; }
        public int DestinationId { get; set; }
        public int AgencyId { get; set; }
        public int TripSeriesId { get; set; }
        public string TripName { get; set; }
        public string TripDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int ChildDiscountPercent { get; set; }

        public int EarlyBookingPercent { get; set; }
        public DateTime? EarlyBookingUntil { get; set; }

        public int LastMinutePercent { get; set; }
        public int LastMinuteDays { get; set; }

        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }

        public virtual Transport Transport { get; set; }
        public virtual Itinerary Itinerary { get; set; }
        public virtual Destination Destination { get; set; }
        public virtual Agency Agency { get; set; }
        public virtual TripSeries Series { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual User UpdatedBy { get; set; }

        public virtual ICollection<TripPrice> Prices { get; set; } = new List<TripPrice>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<TripAccommodation> Accommodations { get; set; } = new List<TripAccommodation>();
        public virtual ICollection<TripAttraction> Attractions { get; set; } = new List<TripAttraction>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<WaitlistEntry> Waitlist { get; set; } = new List<WaitlistEntry>();
        public virtual ICollection<TripExtra> Extras { get; set; } = new List<TripExtra>();
    }
}
