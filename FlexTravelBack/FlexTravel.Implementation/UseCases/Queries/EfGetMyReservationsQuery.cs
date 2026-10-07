using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetMyReservationsQuery : EfUseCase, IGetMyReservationsQuery
    {
        private readonly IUser _user;

        public EfGetMyReservationsQuery(FlexTravelContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public int UseCaseId => UseCaseIds.GetMyReservations;
        public string UseCaseName => "Get my reservations";
        public string UseCaseDescription => "Reservations of the signed-in customer";

        public PagedResponse<ReservationDto> Execute(ReservationSearchDto search)
        {
            var query = Context.Reservation.Where(x => x.UserId == _user.Id && x.IsActive);
            query = ReservationListing.FilterByStatus(query, search.Status);

            query = query
                .OrderBy(x => x.Trip.StartDate < DateTime.Today)
                .ThenBy(x => x.Trip.StartDate)
                .ThenByDescending(x => x.CreatedAt);

            return ReservationListing.Page(query, search, includeCustomer: false);
        }
    }
}
