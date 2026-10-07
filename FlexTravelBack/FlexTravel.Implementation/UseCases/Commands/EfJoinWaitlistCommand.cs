using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Validators;
using FluentValidation;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfJoinWaitlistCommand : EfUseCase, IJoinWaitlistCommand
    {
        private readonly IUser _user;
        private readonly BookingCalculator _calculator;
        private readonly JoinWaitlistValidator _validator;

        public EfJoinWaitlistCommand(FlexTravelContext context, IUser user, BookingCalculator calculator,
            JoinWaitlistValidator validator) : base(context)
        {
            _user = user;
            _calculator = calculator;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.JoinWaitlist;
        public string UseCaseName => "Join waitlist";
        public string UseCaseDescription => "Wait for seats on a sold-out trip";

        public int Execute(JoinWaitlistDto request)
        {
            _validator.ValidateAndThrow(request);
            var travelers = request.Adults + request.Children;

            var trip = Context.Trip
                .Where(t => t.TripId == request.TripId && t.IsActive)
                .Select(t => new { t.TripId, t.Capacity, t.StartDate })
                .FirstOrDefault();

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", request.TripId);
            }

            if (trip.StartDate.Date <= DateTime.Today)
            {
                throw new ConflictException("This trip has already departed.");
            }

            if (Context.WaitlistEntry.Any(w => w.TripId == trip.TripId && w.UserId == _user.Id && w.IsActive))
            {
                throw new ConflictException("You are already on the waitlist for this trip.");
            }

            var seatsLeft = trip.Capacity - _calculator.SeatsTaken(trip.TripId);
            if (seatsLeft >= travelers)
            {
                throw new ConflictException("There are enough free seats, so you can book this trip right away.");
            }

            var entry = new WaitlistEntry
            {
                TripId = trip.TripId,
                UserId = _user.Id,
                Adults = request.Adults,
                Children = request.Children
            };

            Context.WaitlistEntry.Add(entry);
            Context.SaveChanges();

            return entry.WaitlistEntryId;
        }
    }
}
