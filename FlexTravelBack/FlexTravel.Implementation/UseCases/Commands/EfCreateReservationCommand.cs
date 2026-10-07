using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfCreateReservationCommand : EfUseCase, ICreateReservationCommand
    {
        private readonly IUser _user;
        private readonly BookingCalculator _calculator;
        private readonly BookingRequestValidator _validator;
        private readonly NotificationService _notifications;

        public EfCreateReservationCommand(
            FlexTravelContext context,
            IUser user,
            BookingCalculator calculator,
            BookingRequestValidator validator,
            NotificationService notifications) : base(context)
        {
            _user = user;
            _calculator = calculator;
            _validator = validator;
            _notifications = notifications;
        }

        public int UseCaseId => UseCaseIds.CreateReservation;
        public string UseCaseName => "Create reservation";
        public string UseCaseDescription => "Book a trip";

        public CreatedReservationDto Execute(BookingRequestDto request)
        {
            _validator.ValidateAndThrow(request);

            using var transaction = Context.Database.BeginTransaction(IsolationLevel.Serializable);

            var calculation = _calculator.Calculate(request);
            var quote = calculation.Quote;
            if (!quote.IsValid)
            {
                throw new ConflictException(quote.Problems.First());
            }

            if (quote.PromoCodeError != null)
            {
                throw new ConflictException(quote.PromoCodeError);
            }

            var trip = Context.Trip
                .Where(t => t.TripId == request.TripId)
                .Select(t => new { t.AgencyId, t.TripName, t.StartDate })
                .First();

            var travelerProblems = TravelerRules.Check(request.Travelers, request.Adults, request.Children, trip.StartDate, DateTime.Today);
            if (travelerProblems.Count > 0)
            {
                throw new ValidationException(travelerProblems);
            }

            var reservation = new Reservation
            {
                Code = NewUniqueCode(),
                UserId = _user.Id,
                TripId = request.TripId,
                AccommodationId = request.AccommodationId,
                NumberOfAdults = request.Adults,
                NumberOfChildren = request.Children,
                Status = ReservationStatus.Pending,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                PricePerPerson = quote.PricePerPerson,
                ChildDiscountPercent = quote.ChildDiscountPercent,
                Subtotal = quote.Subtotal,
                Offer = calculation.Offer,
                OfferPercent = quote.OfferPercent,
                PromoCodeId = calculation.PromoCodeId,
                PromoPercent = quote.PromoPercent,
                DiscountTotal = quote.OfferDiscount + quote.PromoDiscount,
                ExtrasTotal = quote.ExtrasTotal,
                TotalPrice = quote.Total
            };

            foreach (var extra in quote.Extras)
            {
                reservation.Extras.Add(new ReservationExtra
                {
                    TripExtraId = extra.ExtraId,
                    Kind = ExtraRules.ParseKind(extra.Kind),
                    Name = extra.Name,
                    UnitPrice = extra.UnitPrice,
                    Quantity = extra.Quantity,
                    Total = extra.Total
                });
            }

            reservation.StatusChanges.Add(new ReservationStatusChange
            {
                Status = ReservationStatus.Pending,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = _user.Id
            });

            foreach (var room in quote.Rooms)
            {
                reservation.Rooms.Add(new ReservationRoom
                {
                    RoomId = room.RoomId,
                    NumberOfRooms = room.Count,
                    PricePerNight = room.PricePerNight
                });
            }

            foreach (var traveler in TravelerRules.ToEntities(request.Travelers, trip.StartDate))
            {
                reservation.Travelers.Add(traveler);
            }

            Context.Reservation.Add(reservation);

            foreach (var entry in Context.WaitlistEntry.Where(w => w.TripId == request.TripId && w.UserId == _user.Id && w.IsActive))
            {
                entry.IsActive = false;
            }

            Context.SaveChanges();

            _notifications.SendToAgency(trip.AgencyId, NotificationType.ReservationCreated,
                $"New reservation {reservation.Code}",
                $"{_user.Identity} booked {trip.TripName} for {Travelers(request.Adults, request.Children)}.",
                $"/reservations/{reservation.ReservationId}");
            Context.SaveChanges();
            transaction.Commit();
            _notifications.Push();

            return new CreatedReservationDto { Id = reservation.ReservationId, Code = reservation.Code };
        }

        private static string Travelers(int adults, int children) =>
            (adults == 1 ? "1 adult" : $"{adults} adults") +
            (children == 0 ? "" : children == 1 ? ", 1 child" : $", {children} children");

        private string NewUniqueCode()
        {
            string code;
            do
            {
                code = ReservationCodes.New();
            }
            while (Context.Reservation.AsNoTracking().Any(x => x.Code == code));

            return code;
        }
    }
}
