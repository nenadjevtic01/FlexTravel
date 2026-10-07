using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetTripFormOptionsQuery : EfUseCase, IGetTripFormOptionsQuery
    {
        public EfGetTripFormOptionsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetTripFormOptions;
        public string UseCaseName => "Get trip form options";
        public string UseCaseDescription => "Transports, destinations and hotels for the trip form";

        public TripFormOptionsDto Execute(int request)
        {
            return new TripFormOptionsDto
            {
                Transports = Context.Transport
                    .OrderBy(x => x.TransportName)
                    .Select(x => new LookupDto { Id = x.TransportId, Name = x.TransportName })
                    .ToList(),

                Destinations = Context.Destination
                    .Where(d => d.IsActive && d.Accommodations.Any(a => a.IsActive && a.Rooms.Any()))
                    .OrderBy(d => d.Country.CountryName).ThenBy(d => d.DestinationName)
                    .Select(d => new TripFormDestinationDto
                    {
                        Id = d.DestinationId,
                        Name = d.DestinationName,
                        Country = d.Country.CountryName,
                        Accommodations = d.Accommodations
                            .Where(a => a.IsActive && a.Rooms.Any())
                            .OrderByDescending(a => a.AccommodationStars)
                            .Select(a => new TripFormAccommodationDto
                            {
                                Id = a.AccommodationId,
                                Name = a.AccommodationName,
                                Type = a.AccommodationType.AccommodationTypeName,
                                Stars = a.AccommodationStars,
                                Rooms = a.Rooms.Sum(r => r.NumberOfRooms)
                            })
                            .ToList(),
                        Attractions = d.Attractions
                            .OrderBy(a => a.AttractionName)
                            .Select(a => new AttractionDto
                            {
                                Id = a.AttractionId,
                                Name = a.AttractionName,
                                Description = a.AttractionDescription,
                                Category = a.AttractionCategory.AttractionCategoryName
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
