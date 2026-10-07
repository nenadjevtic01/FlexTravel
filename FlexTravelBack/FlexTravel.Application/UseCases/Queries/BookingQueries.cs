using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Queries
{
    public interface ISearchTripsQuery : IQuery<TripSearchDto, PagedResponse<TripListDto>>
    {
    }

    public interface IGetTripQuery : IQuery<int, TripDetailDto>
    {
    }

    public interface ICalculatePriceQuery : IQuery<BookingRequestDto, PriceQuoteDto>
    {
    }

    public interface IGetMyReservationsQuery : IQuery<ReservationSearchDto, PagedResponse<ReservationDto>>
    {
    }

    public interface IGetAgencyTripsQuery : IQuery<int, AgencyOverviewDto>
    {
    }

    public interface IGetAgencyReservationsQuery : IQuery<ReservationSearchDto, PagedResponse<ReservationDto>>
    {
    }

    public interface IGetTripFormOptionsQuery : IQuery<int, TripFormOptionsDto>
    {
    }

    public interface IGetTripDestinationsQuery : IQuery<int, IEnumerable<TripDestinationLookupDto>>
    {
    }

    public interface IGetReservationQuery : IQuery<int, ReservationDetailDto>
    {
    }

    public interface IGetTripForEditQuery : IQuery<int, TripForEditDto>
    {
    }
}
