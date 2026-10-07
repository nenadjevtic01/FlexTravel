namespace FlexTravel.Domain
{
    public class Continent
    {
        public int ContinentId { get; set; }
        public string ContinentName { get; set; }

        public virtual ICollection<Country> Countries { get; set; } = new List<Country>();
    }
}
