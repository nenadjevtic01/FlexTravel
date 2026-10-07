using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetMyNotificationsQuery : IQuery<int, NotificationsDto>
    {
    }

    public interface IGetMyFavoritesQuery : IQuery<int, IEnumerable<FavoriteDestinationDto>>
    {
    }

    public interface IGetReviewsQuery : IQuery<ReviewSearchDto, ReviewPageDto>
    {
    }

    public interface IGetMyWaitlistQuery : IQuery<int, IEnumerable<WaitlistEntryDto>>
    {
    }

    public interface IGetPromoCodesQuery : IQuery<int, IEnumerable<PromoCodeDto>>
    {
    }
}
