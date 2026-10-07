using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;

namespace FlexTravel.Implementation.UseCases.Queries.Administration
{
    public class EfGetUserQuery : EfUseCase, IGetUserQuery
    {
        public EfGetUserQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetUser;
        public string UseCaseName => "Get user";
        public string UseCaseDescription => "User account with permissions";

        public UserDetailDto Execute(int id)
        {
            var summary = UserProjection.ToList(Context.User.Where(u => u.UserId == id)).FirstOrDefault();
            if (summary == null)
            {
                throw new EntityNotFoundException("User", id);
            }

            var user = Context.User
                .Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Phone,
                    u.Address,
                    u.City,
                    UseCaseIds = u.UseCases.Select(x => x.UseCaseId).OrderBy(x => x).ToList(),
                    Reservations = Context.Reservation.Count(r => r.UserId == u.UserId),
                    TripsCreated = Context.Trip.Count(t => t.CreatedByUserId == u.UserId),
                    u.LockedUntil
                })
                .First();

            return new UserDetailDto
            {
                Id = summary.Id,
                Name = summary.Name,
                Email = summary.Email,
                Avatar = summary.Avatar,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = summary.Role,
                Agency = summary.Agency,
                IsActive = summary.IsActive,
                CreatedAt = summary.CreatedAt,
                UseCases = summary.UseCases,
                Phone = user.Phone,
                Address = user.Address,
                City = user.City,
                Reservations = user.Reservations,
                TripsCreated = user.TripsCreated,
                LockedUntil = user.LockedUntil > DateTime.UtcNow ? user.LockedUntil : null,
                UseCaseIds = user.UseCaseIds,
                DefaultUseCaseIds = summary.Role switch
                {
                    "Admin" => UserUseCaseConstants.AdminUseCaseIds,
                    "AgencyEmployee" => UserUseCaseConstants.AgencyEmployeeUseCaseIds,
                    _ => UserUseCaseConstants.RegularUserUseCaseIds
                },
                AllUseCases = Context.UseCase
                    .OrderBy(u => u.UseCaseId)
                    .Select(u => new UseCaseInfoDto { Id = u.UseCaseId, Name = u.UseCaseName, Description = u.UseCaseDescription })
                    .ToList()
            };
        }
    }
}
