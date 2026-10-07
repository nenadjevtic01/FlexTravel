namespace FlexTravel.Domain
{
    public class TripSeries
    {
        public int TripSeriesId { get; set; }
        public int AgencyId { get; set; }
        public string Name { get; set; }

        public virtual Agency Agency { get; set; }
        public virtual ICollection<Trip> Departures { get; set; } = new List<Trip>();
    }
}
