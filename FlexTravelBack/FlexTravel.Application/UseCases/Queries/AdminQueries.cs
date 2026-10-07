using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetAdminStatsQuery : IQuery<int, AdminStatsDto>
    {
    }

    public interface ISearchUsersQuery : IQuery<UserSearchDto, PagedResponse<UserListDto>>
    {
    }

    public interface IGetUserQuery : IQuery<int, UserDetailDto>
    {
    }

    public interface IGetAgenciesQuery : IQuery<int, IEnumerable<AgencyAdminDto>>
    {
    }
}
