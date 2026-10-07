namespace FlexTravel.Domain
{
    public class Transport
    {
        public int TransportId { get; set; }
        public string TransportName { get; set; }

        public virtual ICollection<Trip> TripList { get; set; } = new List<Trip>();
    }
}
