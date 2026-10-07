namespace FlexTravel.Domain
{
    public class ItineraryActivity
    {
        public int ItineraryActivityId { get; set; }
        public int ItineraryId { get; set; }
        public int Day { get; set; }

        public string Title { get; set; }
        public string Activity { get; set; }
        public Meals Meals { get; set; }

        public string Overnight { get; set; }

        public virtual Itinerary Itinerary { get; set; }
    }

    [Flags]
    public enum Meals
    {
        None = 0,
        Breakfast = 1,
        Lunch = 2,
        Dinner = 4
    }
}
