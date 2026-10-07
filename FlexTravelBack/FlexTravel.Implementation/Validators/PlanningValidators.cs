using FlexTravel.Application.Dto;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class UpdateAgencyProfileValidator : AbstractValidator<UpdateAgencyProfileDto>
    {
        public UpdateAgencyProfileValidator()
        {
            RuleFor(x => x.Description).MaximumLength(1500).WithMessage("The description can have at most 1500 characters.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Enter the address of the agency.")
                .MaximumLength(100);
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Enter the phone number of the agency.")
                .MaximumLength(20)
                .Matches(@"^[0-9+()/\- ]+$").WithMessage("The phone number can contain digits, spaces and + ( ) / -.");
            RuleFor(x => x.Email).EmailAddress().MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("Enter a valid e-mail address.");
            RuleFor(x => x.Website)
                .Must(w => Uri.TryCreate(w.Trim(), UriKind.Absolute, out var uri) && (uri.Scheme == "https" || uri.Scheme == "http"))
                .When(x => !string.IsNullOrWhiteSpace(x.Website))
                .WithMessage("The website must be a full address starting with https://.")
                .MaximumLength(200);

            RuleFor(x => x.Logo)
                .Must(l => l.Trim().StartsWith("uploads/") && !l.Contains("..") && l.Trim().Length <= 200)
                .When(x => !string.IsNullOrWhiteSpace(x.Logo))
                .WithMessage("Upload the logo as an image.");
        }
    }
}
