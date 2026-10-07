using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class BookingRequestValidator : AbstractValidator<BookingRequestDto>
    {
        public BookingRequestValidator()
        {
            RuleFor(x => x.TripId).GreaterThan(0);
            RuleFor(x => x.AccommodationId).GreaterThan(0).WithMessage("Choose a hotel.");
            RuleFor(x => x.Adults).GreaterThanOrEqualTo(1).WithMessage("At least one adult must travel.");
            RuleFor(x => x.Children).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Adults + x.Children)
                .LessThanOrEqualTo(BookingRules.MaxTravelersPerReservation)
                .WithName("Travelers")
                .WithMessage($"One reservation can include at most {BookingRules.MaxTravelersPerReservation} travelers.");
            RuleFor(x => x.Rooms)
                .Must(rooms => rooms != null && rooms.Any(r => r.Count > 0))
                .WithMessage("Choose at least one room.");
            RuleForEach(x => x.Rooms).ChildRules(room =>
            {
                room.RuleFor(r => r.Count).InclusiveBetween(0, 10);
            });
            RuleFor(x => x.Note).MaximumLength(500);
        }
    }
}
