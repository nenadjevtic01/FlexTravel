using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface IUpdateMyProfileCommand : ICommand<UpdateProfileDto>
    {
    }

    public interface IChangePasswordCommand : ICommand<ChangePasswordDto>
    {
    }

    public interface IUpdateTravelersCommand : ICommand<UpdateTravelersDto>
    {
    }

    public interface IRecordPaymentCommand : ICommand<RecordPaymentDto, int>
    {
    }

    public interface ISendReservationMessageCommand : ICommand<SendMessageDto, int>
    {
    }

    public interface ISetDestinationActiveCommand : ICommand<SetActiveDto>
    {
    }

    public interface ISetHotelActiveCommand : ICommand<SetActiveDto>
    {
    }

    public interface ICleanUpImagesCommand : ICommand<int, int>
    {
    }

    public interface IAddDepartureCommand : ICommand<AddDepartureDto, int>
    {
    }
}

namespace FlexTravel.Application.UseCases.Queries
{
    public interface IGetMyProfileQuery : IQuery<int, ProfileDto>
    {
    }

    public interface IGetTripManifestQuery : IQuery<int, TripManifestDto>
    {
    }

    public interface IGetReservationMessagesQuery : IQuery<int, IEnumerable<ReservationMessageDto>>
    {
    }

    public interface IGetSimilarTripsQuery : IQuery<int, SimilarTripsDto>
    {
    }
}
