namespace FlexTravel.Application.Dto
{
    public class UseCaseLogSearchDto : PagedSearch
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string UseCaseName { get; set; }
        public string Username { get; set; }
    }
}
