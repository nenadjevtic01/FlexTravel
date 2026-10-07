using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FluentValidation;

namespace FlexTravel.Implementation.Validators
{
    public class CreateTripValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripValidator(FlexTravelContext context)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
            RuleFor(x => x.DestinationId)
                .Must(id => context.Destination.Any(d => d.DestinationId == id && d.IsActive))
                .WithMessage("Choose a destination.");
            RuleFor(x => x.TransportId)
                .Must(id => context.Transport.Any(t => t.TransportId == id))
                .WithMessage("Choose a means of transport.");
            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.Today)
                .WithMessage("The trip must start in the future.");
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("The trip must end after it starts.");
            RuleFor(x => x.Capacity).InclusiveBetween(1, 500);
            RuleFor(x => x.PricePerPerson).GreaterThan(0).LessThan(100000);
            RuleFor(x => x.ChildDiscountPercent).InclusiveBetween(0, 100);
            RuleFor(x => x.AccommodationIds)
                .NotEmpty().WithMessage("Offer at least one hotel.");
            RuleFor(x => x)
                .Must(x => x.AccommodationIds.All(id => context.Accommodation.Any(a => a.AccommodationId == id
                    && a.DestinationId == x.DestinationId
                    && a.IsActive && a.Rooms.Any())))
                .When(x => x.AccommodationIds.Any())
                .WithName("AccommodationIds")
                .WithMessage("Every hotel must be in the chosen destination and have rooms.");
            RuleFor(x => x.Itinerary)
                .Must(days => days != null && days.Any(d => !ItineraryRules.IsEmpty(d)))
                .WithMessage("Describe at least one day of the itinerary.");
            RuleFor(x => x)
                .Must(x => x.Itinerary.Count <= (x.EndDate.Date - x.StartDate.Date).Days + 1)
                .When(x => x.EndDate > x.StartDate && x.Itinerary != null)
                .WithName("Itinerary")
                .WithMessage("The itinerary has more days than the trip.");
            RuleForEach(x => x.Itinerary)
                .ChildRules(day =>
                {
                    day.RuleFor(d => d.Title).MaximumLength(100).WithMessage("A day title can have at most 100 characters.");
                    day.RuleFor(d => d.Activity).MaximumLength(1000).WithMessage("A day description can have at most 1000 characters.");
                    day.RuleFor(d => d.Overnight).MaximumLength(100).WithMessage("The overnight place can have at most 100 characters.");
                })
                .When(x => x.Itinerary != null);

            RuleFor(x => x.Extras)
                .Must(extras => extras == null || extras.Count <= BookingRules.MaxExtrasPerTrip)
                .WithMessage($"A trip can offer at most {BookingRules.MaxExtrasPerTrip} extras.");
            RuleForEach(x => x.Extras)
                .ChildRules(extra =>
                {
                    extra.RuleFor(e => e.Name).NotEmpty().WithMessage("Every extra needs a name.")
                        .MaximumLength(100).WithMessage("An extra name can have at most 100 characters.");
                    extra.RuleFor(e => e.Description).MaximumLength(300).WithMessage("An extra description can have at most 300 characters.");
                    extra.RuleFor(e => e.Price).GreaterThan(0).LessThan(10000).WithMessage("Every extra needs a price between €0 and €10,000.");
                    extra.RuleFor(e => e.Kind)
                        .Must(k => Enum.TryParse<ExtraKind>(k, true, out var kind) && Enum.IsDefined(kind))
                        .WithMessage("Choose the kind of every extra.");
                })
                .When(x => x.Extras != null);

            RuleFor(x => x.EarlyBookingPercent).InclusiveBetween(0, BookingRules.MaxDiscountPercent);
            RuleFor(x => x.LastMinutePercent).InclusiveBetween(0, BookingRules.MaxDiscountPercent);
            RuleFor(x => x.EarlyBookingUntil)
                .NotNull()
                .When(x => x.EarlyBookingPercent > 0)
                .WithMessage("Set the last day of the early booking offer.");
            RuleFor(x => x)
                .Must(x => x.EarlyBookingUntil.Value.Date < x.StartDate.Date)
                .When(x => x.EarlyBookingPercent > 0 && x.EarlyBookingUntil.HasValue)
                .WithName("EarlyBookingUntil")
                .WithMessage("Early booking must end before the trip starts.");
            RuleFor(x => x.LastMinuteDays)
                .InclusiveBetween(1, 60)
                .When(x => x.LastMinutePercent > 0)
                .WithMessage("Last minute must start 1 to 60 days before departure.");
            RuleFor(x => x)
                .Must(x => x.EarlyBookingUntil.Value.Date < x.StartDate.Date.AddDays(-x.LastMinuteDays))
                .When(x => x.EarlyBookingPercent > 0 && x.EarlyBookingUntil.HasValue && x.LastMinutePercent > 0)
                .WithName("EarlyBookingUntil")
                .WithMessage("Early booking must end before the last minute period starts.");

            RuleFor(x => x)
                .Must(x => x.AttractionIds.All(id => context.Attraction.Any(a => a.AttractionId == id && a.DestinationId == x.DestinationId)))
                .When(x => x.AttractionIds != null && x.AttractionIds.Any())
                .WithName("AttractionIds")
                .WithMessage("Every sight must be in the chosen destination.");
        }
    }
}
