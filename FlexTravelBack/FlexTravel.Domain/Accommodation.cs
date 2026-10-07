namespace FlexTravel.Domain
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; }
        public decimal AccommodationStars { get; set; }
        public int DestinationId { get; set; }
        public int AccommodationTypeId { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual Destination Destination { get; set; }
        public virtual AccommodationType AccommodationType { get; set; }
        public virtual ICollection<TripAccommodation> Trips { get; set; } = new List<TripAccommodation>();
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
