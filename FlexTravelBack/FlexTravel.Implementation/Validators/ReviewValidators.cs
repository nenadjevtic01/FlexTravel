using FlexTravel.Application.Dto;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.TripRating).InclusiveBetween(1, 5).WithMessage("Rate the trip from 1 to 5 stars.");
            RuleFor(x => x.HotelRating).InclusiveBetween(1, 5).WithMessage("Rate the hotel from 1 to 5 stars.");
            RuleFor(x => x.GuideRating).InclusiveBetween(1, 5).When(x => x.GuideRating.HasValue)
                .WithMessage("Rate the guide from 1 to 5 stars, or leave it out.");
            RuleFor(x => x.OrganisationRating).InclusiveBetween(1, 5).When(x => x.OrganisationRating.HasValue)
                .WithMessage("Rate the organisation from 1 to 5 stars, or leave it out.");
            RuleFor(x => x.ValueRating).InclusiveBetween(1, 5).When(x => x.ValueRating.HasValue)
                .WithMessage("Rate the value for money from 1 to 5 stars, or leave it out.");
            RuleFor(x => x.Comment)
                .Must(c => !string.IsNullOrWhiteSpace(c) && c.Trim().Length >= 20)
                .WithMessage("Write at least 20 characters about your trip.")
                .MaximumLength(1000);
        }
    }

    public class ReplyToReviewValidator : AbstractValidator<ReplyToReviewDto>
    {
        public ReplyToReviewValidator()
        {
            RuleFor(x => x.Reply)
                .Must(r => !string.IsNullOrWhiteSpace(r) && r.Trim().Length >= 5)
                .WithMessage("Write a reply of at least 5 characters.")
                .MaximumLength(1000);
        }
    }

    public class ModerateReviewValidator : AbstractValidator<ModerateReviewDto>
    {
        public ModerateReviewValidator()
        {
            RuleFor(x => x.Note)
                .Must(n => !string.IsNullOrWhiteSpace(n))
                .When(x => !x.Visible)
                .WithMessage("Say why the review is hidden; the customer will see the reason.");
            RuleFor(x => x.Note).MaximumLength(300);
        }
    }
}
