using FlexTravel.Application.UseCases;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.Booking
{
    public class ReservationAccess
    {
        private readonly FlexTravelContext _context;
        private readonly IUser _user;
        private int? _agencyId;
        private bool _agencyLoaded;

        public ReservationAccess(FlexTravelContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public int? AgencyId
        {
            get
            {
                if (!_agencyLoaded)
                {
                    _agencyId = _context.User
                        .OfType<AgencyEmployee>()
                        .Where(e => e.UserId == _user.Id)
                        .Select(e => (int?)e.AgencyId)
                        .FirstOrDefault();
                    _agencyLoaded = true;
                }

                return _agencyId;
            }
        }

        public bool IsAgency => AgencyId.HasValue;

        public IQueryable<Reservation> Visible()
        {
            var query = _context.Reservation.Where(r => r.IsActive);
            var agencyId = AgencyId;

            return agencyId.HasValue
                ? query.Where(r => r.Trip.AgencyId == agencyId.Value)
                : query.Where(r => r.UserId == _user.Id);
        }
    }
}
