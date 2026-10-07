namespace FlexTravel.Application.Dto
{
    public class DestinationSearchDto : PagedSearch
    {
        public string Keyword { get; set; }
        public int? ContinentId { get; set; }
        public int? CountryId { get; set; }

        public bool IncludeHidden { get; set; }
    }

    public class DestinationListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public string Image { get; set; }
        public int AccommodationsCount { get; set; }
        public int SafetyLevel { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; }
    }

    public class DestinationDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Climate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public IEnumerable<ImageDto> Images { get; set; } = new List<ImageDto>();
        public CountryDto Country { get; set; }
        public IEnumerable<AccommodationDto> Accommodations { get; set; } = new List<AccommodationDto>();
        public IEnumerable<AttractionDto> Attractions { get; set; } = new List<AttractionDto>();

        public RatingDto Rating { get; set; }
    }

    public class ImageDto
    {
        public string Src { get; set; }
        public string Alt { get; set; }
    }

    public class AccommodationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Stars { get; set; }
        public RatingDto Rating { get; set; }
        public IEnumerable<RoomDto> Rooms { get; set; } = new List<RoomDto>();
    }

    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public int NumberOfRooms { get; set; }
        public bool AC { get; set; }
        public bool Balcony { get; set; }
    }
}
