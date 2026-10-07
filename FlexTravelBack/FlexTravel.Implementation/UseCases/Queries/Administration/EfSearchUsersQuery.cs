using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Queries.Administration
{
    public class EfSearchUsersQuery : EfUseCase, ISearchUsersQuery
    {
        public EfSearchUsersQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.SearchUsers;
        public string UseCaseName => "Search users";
        public string UseCaseDescription => "Search user accounts";

        public PagedResponse<UserListDto> Execute(UserSearchDto search)
        {
            var query = Context.User.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var keyword = search.Keyword.Trim();
                query = query.Where(u => u.FirstName.Contains(keyword)
                                      || u.LastName.Contains(keyword)
                                      || u.Email.Contains(keyword));
            }

            query = search.Role switch
            {
                "Customer" => query.Where(u => u is RegularUser),
                "AgencyEmployee" => query.Where(u => u is AgencyEmployee),
                "Admin" => query.Where(u => u is Domain.Admin),
                _ => query
            };

            var perPage = Math.Clamp(search.PerPage, 1, 50);
            var page = Math.Max(search.Page, 1);

            return new PagedResponse<UserListDto>
            {
                TotalCount = query.Count(),
                CurrentPage = page,
                ItemsPerPage = perPage,
                Items = UserProjection.ToList(query
                    .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                    .Skip((page - 1) * perPage)
                    .Take(perPage))
            };
        }
    }

    internal static class UserProjection
    {
        public static List<UserListDto> ToList(IQueryable<User> query) =>
            query.Select(u => new UserListDto
            {
                Id = u.UserId,
                Name = u.FirstName + " " + u.LastName,
                Email = u.Email,
                Avatar = u.Avatar,
                City = u.City,
                Role = u is Domain.Admin ? "Admin" : u is AgencyEmployee ? "AgencyEmployee" : "Customer",
                Agency = u is AgencyEmployee ? ((AgencyEmployee)u).Agency.AgencyName : null,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UseCases = u.UseCases.Count
            }).ToList();
    }
}
