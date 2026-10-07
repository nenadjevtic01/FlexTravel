using FlexTravel.Application.UseCases;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Exceptions;

namespace FlexTravel.Implementation.Booking
{
    public class CurrentAgency
    {
        private readonly FlexTravelContext _context;
        private readonly IUser _user;

        public CurrentAgency(FlexTravelContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public int Id(string useCaseName)
        {
            var agencyId = _context.User
                .OfType<AgencyEmployee>()
                .Where(x => x.UserId == _user.Id)
                .Select(x => (int?)x.AgencyId)
                .FirstOrDefault();

            if (agencyId == null)
            {
                throw new ForbiddenUseCaseExecutionException(useCaseName, _user.Identity);
            }

            return agencyId.Value;
        }
    }
}
