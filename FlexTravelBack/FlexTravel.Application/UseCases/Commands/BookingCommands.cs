using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface IRegisterUserCommand : ICommand<RegisterUserDto>
    {
    }

    public interface ICreateReservationCommand : ICommand<BookingRequestDto, CreatedReservationDto>
    {
    }

    public interface ICancelReservationCommand : ICommand<int>
    {
    }

    public interface ICreateTripCommand : ICommand<CreateTripDto, int>
    {
    }

    public interface IDeactivateTripCommand : ICommand<int>
    {
    }

    public interface IDecideReservationCommand : ICommand<DecideReservationDto>
    {
    }

    public interface IUpdateTripCommand : ICommand<UpdateTripDto>
    {
    }
}
