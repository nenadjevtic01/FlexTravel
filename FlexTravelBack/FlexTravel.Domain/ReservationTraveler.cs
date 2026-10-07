namespace FlexTravel.Domain
{
    public class ReservationTraveler
    {
        public int ReservationTravelerId { get; set; }
        public int ReservationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsChild { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}
