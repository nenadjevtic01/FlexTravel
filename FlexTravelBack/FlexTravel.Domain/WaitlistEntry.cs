namespace FlexTravel.Domain
{
    public class WaitlistEntry : Entity
    {
        public int WaitlistEntryId { get; set; }
        public int TripId { get; set; }
        public int UserId { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }

        public DateTime? NotifiedAt { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual RegularUser User { get; set; }
    }
}
