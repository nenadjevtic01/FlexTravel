using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetContinentsQuery : EfUseCase, IGetContinentsQuery
    {
        public EfGetContinentsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetContinents;
        public string UseCaseName => "Get continents";
        public string UseCaseDescription => "List of continents";

        public IEnumerable<ContinentDto> Execute(int request)
        {
            return Context.Continent
                .OrderBy(x => x.ContinentName)
                .Select(x => new ContinentDto
                {
                    Id = x.ContinentId,
                    Name = x.ContinentName,
                    CountriesCount = x.Countries.Count,
                    DestinationsCount = x.Countries.SelectMany(c => c.Destination).Count(d => d.IsActive)
                })
                .ToList();
        }
    }
}
