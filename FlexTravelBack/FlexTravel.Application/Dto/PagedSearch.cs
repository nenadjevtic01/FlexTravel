namespace FlexTravel.Application.Dto
{
    public abstract class PagedSearch
    {
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 12;
    }

    public class PagedResponse<T>
    {
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int PagesCount => ItemsPerPage == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / ItemsPerPage);
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
