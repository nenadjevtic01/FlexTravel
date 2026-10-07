using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetDestinationsQuery : EfUseCase, IGetDestinationsQuery
    {
        private readonly FlexTravel.Application.UseCases.IUser _user;

        public EfGetDestinationsQuery(FlexTravelContext context, FlexTravel.Application.UseCases.IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetDestinations;
        public string UseCaseName => "Search destinations";
        public string UseCaseDescription => "Search destinations by keyword, continent and country";

        public PagedResponse<DestinationListDto> Execute(DestinationSearchDto search)
        {
            var showHidden = search.IncludeHidden && _user.UseCaseIds.Contains(UseCaseIds.SetDestinationActive);
            var query = showHidden ? Context.Destination.AsQueryable() : Context.Destination.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var keyword = search.Keyword.Trim();
                query = query.Where(x => x.DestinationName.Contains(keyword)
                                      || x.DestinationDescription.Contains(keyword)
                                      || x.Country.CountryName.Contains(keyword));
            }

            if (search.ContinentId.HasValue)
            {
                query = query.Where(x => x.Country.ContinentId == search.ContinentId.Value);
            }

            if (search.CountryId.HasValue)
            {
                query = query.Where(x => x.CountryId == search.CountryId.Value);
            }

            var perPage = Math.Clamp(search.PerPage, 1, 100);
            var page = Math.Max(search.Page, 1);

            return new PagedResponse<DestinationListDto>
            {
                TotalCount = query.Count(),
                CurrentPage = page,
                ItemsPerPage = perPage,
                Items = query
                    .OrderBy(x => x.DestinationName)
                    .Skip((page - 1) * perPage)
                    .Take(perPage)
                    .Select(x => new DestinationListDto
                    {
                        Id = x.DestinationId,
                        Name = x.DestinationName,
                        Description = x.DestinationDescription,
                        Country = x.Country.CountryName,
                        Continent = x.Country.Continent.ContinentName,
                        Image = x.Images.OrderBy(i => i.DestinationImageId).Select(i => i.Src).FirstOrDefault(),
                        AccommodationsCount = x.Accommodations.Count(a => a.IsActive),
                        SafetyLevel = x.Country.Safety == null ? 0 : x.Country.Safety.SafetyLevel,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        IsActive = x.IsActive
                    })
                    .ToList()
            };
        }
    }
}
