using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Queries.Administration
{
    public class EfGetAgenciesQuery : EfUseCase, IGetAgenciesQuery
    {
        public EfGetAgenciesQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetAgencies;
        public string UseCaseName => "Get agencies";
        public string UseCaseDescription => "Agencies with employees and results";

        public IEnumerable<AgencyAdminDto> Execute(int request)
        {
            var today = DateTime.Today;

            return Context.Agency
                .OrderBy(a => a.AgencyName)
                .Select(a => new AgencyAdminDto
                {
                    Id = a.AgencyId,
                    Name = a.AgencyName,
                    Address = a.Address,
                    Phone = a.Phone,
                    Trips = a.Trips.Count,
                    TripsOnSale = a.Trips.Count(t => t.IsActive && t.StartDate > today),
                    Reservations = a.Trips.SelectMany(t => t.Reservations)
                        .Count(r => r.IsActive && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed)),
                    Revenue = a.Trips.SelectMany(t => t.Reservations)
                        .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed)
                        .Sum(r => (decimal?)r.TotalPrice) ?? 0,
                    Employees = a.Employees
                        .OrderBy(e => e.FirstName)
                        .Select(e => new AgencyTeamMemberDto
                        {
                            Id = e.UserId,
                            Name = e.FirstName + " " + e.LastName,
                            Email = e.Email,
                            Avatar = e.Avatar,
                            IsActive = e.IsActive,
                            TripsCreated = Context.Trip.Count(t => t.CreatedByUserId == e.UserId),
                            ReservationsHandled = Context.ReservationStatusChange.Count(c => c.ChangedByUserId == e.UserId
                                && (c.Status == ReservationStatus.Confirmed || c.Status == ReservationStatus.Rejected))
                        })
                        .ToList()
                })
                .ToList();
        }
    }
}
