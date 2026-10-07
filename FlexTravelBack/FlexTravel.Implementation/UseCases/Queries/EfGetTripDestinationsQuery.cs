using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetTripDestinationsQuery : EfUseCase, IGetTripDestinationsQuery
    {
        public EfGetTripDestinationsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetTripDestinations;
        public string UseCaseName => "Get trip destinations";
        public string UseCaseDescription => "Destinations that have upcoming trips";

        public IEnumerable<TripDestinationLookupDto> Execute(int request)
        {
            var today = DateTime.Today;

            return Context.Destination
                .Where(d => d.Trips.Any(t => t.IsActive && t.StartDate > today))
                .OrderBy(d => d.Country.CountryName).ThenBy(d => d.DestinationName)
                .Select(d => new TripDestinationLookupDto
                {
                    Id = d.DestinationId,
                    Name = d.DestinationName,
                    Country = d.Country.CountryName,
                    Trips = d.Trips.Count(t => t.IsActive && t.StartDate > today)
                })
                .ToList();
        }
    }
}
