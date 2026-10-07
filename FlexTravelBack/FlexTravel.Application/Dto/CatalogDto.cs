namespace FlexTravel.Application.Dto
{
    public class CatalogOptionsDto
    {
        public IEnumerable<LookupDto> Continents { get; set; } = new List<LookupDto>();
        public IEnumerable<LookupDto> AccommodationTypes { get; set; } = new List<LookupDto>();
        public IEnumerable<RoomTypeLookupDto> RoomTypes { get; set; } = new List<RoomTypeLookupDto>();
        public IEnumerable<LookupDto> AttractionCategories { get; set; } = new List<LookupDto>();
    }

    public class RoomTypeLookupDto : LookupDto
    {
        public int Capacity { get; set; }
    }

    public class SaveCountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContinentId { get; set; }
        public string CapitalCity { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public long? Population { get; set; }
        public int? TotalArea { get; set; }

        public string VisaType { get; set; }
        public string VisaDuration { get; set; }
        public string VisaRequirements { get; set; }

        public int? SafetyLevel { get; set; }
        public string SafetyAdvices { get; set; }
    }

    public class SaveDestinationDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Climate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<ImageDto> Images { get; set; } = new List<ImageDto>();

        public List<SaveHotelDto> Hotels { get; set; } = new List<SaveHotelDto>();
        public List<SaveSightDto> Sights { get; set; } = new List<SaveSightDto>();
    }

    public class DestinationForEditDto
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Climate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; }
        public int UpcomingTrips { get; set; }
        public IEnumerable<ImageDto> Images { get; set; } = new List<ImageDto>();
        public IEnumerable<HotelForEditDto> Hotels { get; set; } = new List<HotelForEditDto>();
        public IEnumerable<SightForEditDto> Sights { get; set; } = new List<SightForEditDto>();
    }

    public class HotelForEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public decimal Stars { get; set; }
        public int Trips { get; set; }
        public int UpcomingTrips { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<RoomForEditDto> Rooms { get; set; } = new List<RoomForEditDto>();
    }

    public class RoomForEditDto
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public int NumberOfRooms { get; set; }
        public bool AC { get; set; }
        public bool Balcony { get; set; }
        public decimal PricePerNight { get; set; }

        public int MaxBooked { get; set; }

        public bool HasReservations { get; set; }
    }

    public class SightForEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int Trips { get; set; }
    }

    public class SaveHotelDto
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public decimal Stars { get; set; }
        public List<SaveRoomDto> Rooms { get; set; } = new List<SaveRoomDto>();
    }

    public class SaveRoomDto
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public int NumberOfRooms { get; set; }
        public bool AC { get; set; }
        public bool Balcony { get; set; }
        public decimal PricePerNight { get; set; }
    }

    public class SaveSightDto
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
    }

    public class UploadImageDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
