using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetCountriesQuery : EfUseCase, IGetCountriesQuery
    {
        public EfGetCountriesQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetCountries;
        public string UseCaseName => "Get countries";
        public string UseCaseDescription => "List of countries with visa and safety info";

        public IEnumerable<CountryDto> Execute(CountrySearchDto search)
        {
            var query = Context.Country.AsQueryable();

            if (search.ContinentId.HasValue)
            {
                query = query.Where(x => x.ContinentId == search.ContinentId.Value);
            }

            return query
                .OrderBy(x => x.CountryName)
                .Select(x => new CountryDto
                {
                    Id = x.CountryId,
                    Name = x.CountryName,
                    ContinentId = x.ContinentId,
                    Continent = x.Continent.ContinentName,
                    CapitalCity = x.CapitalCity,
                    Currency = x.Currency,
                    Language = x.Language,
                    Population = x.Population,
                    TotalArea = x.TotalArea,
                    DestinationsCount = x.Destination.Count(d => d.IsActive),
                    Visa = x.VisaInfo == null ? null : new VisaDto
                    {
                        Type = x.VisaInfo.VisaType,
                        Duration = x.VisaInfo.Duration,
                        Requirements = x.VisaInfo.Requirements
                    },
                    Safety = x.Safety == null ? null : new SafetyDto
                    {
                        Level = x.Safety.SafetyLevel,
                        Advices = x.Safety.Advices
                    }
                })
                .ToList();
        }
    }
}
