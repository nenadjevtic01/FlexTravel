using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class CreateAgencyValidator : AbstractValidator<CreateAgencyDto>
    {
        public CreateAgencyValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Must(name => !context.Agency.Any(a => a.AgencyName == name.Trim()))
                .WithMessage("An agency with this name already exists.");
            RuleFor(x => x.Address).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        }
    }

    public class CreateAgencyEmployeeValidator : AbstractValidator<CreateAgencyEmployeeDto>
    {
        public CreateAgencyEmployeeValidator(FlexTravelContext context, RegisterUserValidator accountRules)
        {
            Include(accountRules);
            RuleFor(x => x.AgencyId)
                .Must(id => context.Agency.Any(a => a.AgencyId == id))
                .WithMessage("Choose an agency.");
        }
    }

    public class UpdateUserUseCasesValidator : AbstractValidator<UpdateUserUseCasesDto>
    {
        public UpdateUserUseCasesValidator(FlexTravelContext context)
        {
            RuleFor(x => x.UseCaseIds).NotNull();
            RuleFor(x => x.UseCaseIds)
                .Must(ids => ids.Distinct().Count() == context.UseCase.Count(u => ids.Contains(u.UseCaseId)))
                .WithMessage("Unknown use case.");
        }
    }
}
