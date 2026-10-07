using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetContinentsQuery : IQuery<int, IEnumerable<ContinentDto>>
    {
    }

    public interface IGetCountriesQuery : IQuery<CountrySearchDto, IEnumerable<CountryDto>>
    {
    }

    public interface IGetDestinationsQuery : IQuery<DestinationSearchDto, PagedResponse<DestinationListDto>>
    {
    }

    public interface IGetDestinationQuery : IQuery<int, DestinationDetailDto>
    {
    }

    public interface IGetAuditLogsQuery : IQuery<UseCaseLogSearchDto, PagedResponse<UseCaseLogDto>>
    {
    }
}
