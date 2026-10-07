using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface IMarkNotificationsReadCommand : ICommand<MarkNotificationsReadDto>
    {
    }

    public interface ISetFavoriteCommand : ICommand<SetFavoriteDto>
    {
    }

    public interface ICreateReviewCommand : ICommand<CreateReviewDto, int>
    {
    }

    public interface IReplyToReviewCommand : ICommand<ReplyToReviewDto>
    {
    }

    public interface IModerateReviewCommand : ICommand<ModerateReviewDto>
    {
    }

    public interface IJoinWaitlistCommand : ICommand<JoinWaitlistDto, int>
    {
    }

    public interface ILeaveWaitlistCommand : ICommand<int>
    {
    }

    public interface ICreatePromoCodeCommand : ICommand<CreatePromoCodeDto, int>
    {
    }

    public interface ISetPromoCodeActiveCommand : ICommand<SetPromoCodeActiveDto>
    {
    }
}
