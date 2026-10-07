using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetAgencyReservationsQuery : EfUseCase, IGetAgencyReservationsQuery
    {
        private readonly CurrentAgency _agency;

        public EfGetAgencyReservationsQuery(FlexTravelContext context, CurrentAgency agency) : base(context)
        {
            _agency = agency;
        }

        public int UseCaseId => UseCaseIds.GetAgencyReservations;
        public string UseCaseName => "Get agency reservations";
        public string UseCaseDescription => "Reservations for the agency's trips";

        public PagedResponse<ReservationDto> Execute(ReservationSearchDto search)
        {
            var agencyId = _agency.Id(UseCaseName);
            var query = Context.Reservation.Where(x => x.Trip.AgencyId == agencyId && x.IsActive);
            query = ReservationListing.FilterByStatus(query, search.Status);

            if (search.TripId.HasValue)
            {
                query = query.Where(x => x.TripId == search.TripId.Value);
            }

            query = query
                .OrderBy(x => x.Status != ReservationStatus.Pending)
                .ThenByDescending(x => x.CreatedAt);

            return ReservationListing.Page(query, search, includeCustomer: true);
        }
    }
}
