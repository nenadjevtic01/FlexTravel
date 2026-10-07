using FlexTravel.Application.Dto;

namespace FlexTravel.Application.UseCases.Commands
{
    public interface IUpdateUserUseCasesCommand : ICommand<UpdateUserUseCasesDto>
    {
    }

    public interface ISetUserActiveCommand : ICommand<SetUserActiveDto>
    {
    }

    public interface ICreateAgencyCommand : ICommand<CreateAgencyDto, int>
    {
    }

    public interface ICreateAgencyEmployeeCommand : ICommand<CreateAgencyEmployeeDto, int>
    {
    }
}
