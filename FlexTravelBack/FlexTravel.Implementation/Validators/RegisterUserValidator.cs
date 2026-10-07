using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator(FlexTravelContext context)
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(254)
                .Must(email => !context.User.Any(u => u.Email == email))
                .WithMessage("An account with this email already exists.");
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter.");
            RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        }
    }
}
