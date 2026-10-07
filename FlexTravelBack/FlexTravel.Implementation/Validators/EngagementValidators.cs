using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FlexTravel.Implementation.Validators
{
    public class JoinWaitlistValidator : AbstractValidator<JoinWaitlistDto>
    {
        public JoinWaitlistValidator()
        {
            RuleFor(x => x.Adults).GreaterThanOrEqualTo(1).WithMessage("At least one adult must travel.");
            RuleFor(x => x.Children).GreaterThanOrEqualTo(0);
            RuleFor(x => x)
                .Must(x => x.Adults + x.Children <= BookingRules.MaxTravelersPerReservation)
                .WithName("Adults")
                .WithMessage($"At most {BookingRules.MaxTravelersPerReservation} travelers.");
        }
    }

    public class CreatePromoCodeValidator : AbstractValidator<CreatePromoCodeDto>
    {
        private static readonly Regex CodePattern = new Regex("^[A-Z0-9-]{3,20}$");

        public CreatePromoCodeValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Code)
                .Must(code => code != null && CodePattern.IsMatch(BookingPricing.NormalizePromoCode(code) ?? ""))
                .WithMessage("The code has 3 to 20 letters, digits or dashes.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Code)
                        .Must(code => !context.PromoCode.Any(p => p.Code == BookingPricing.NormalizePromoCode(code)))
                        .WithMessage("This code is already taken.");
                });
            RuleFor(x => x.Description).MaximumLength(200);
            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(1, BookingRules.MaxDiscountPercent)
                .WithMessage($"The discount is between 1 and {BookingRules.MaxDiscountPercent} %.");
            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .WithMessage("The code must be valid until a day after it starts.");
            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("The code must be valid in the future.");
            RuleFor(x => x.MaxUses)
                .InclusiveBetween(1, 10000)
                .When(x => x.MaxUses.HasValue);
        }
    }
}
