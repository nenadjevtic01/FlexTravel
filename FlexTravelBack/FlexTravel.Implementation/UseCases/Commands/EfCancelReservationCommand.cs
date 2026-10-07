using FlexTravel.Application.Constants;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands
{
    public class EfCancelReservationCommand : EfUseCase, ICancelReservationCommand
    {
        private readonly IUser _user;
        private readonly NotificationService _notifications;
        private readonly WaitlistService _waitlist;

        public EfCancelReservationCommand(FlexTravelContext context, IUser user,
            NotificationService notifications, WaitlistService waitlist) : base(context)
        {
            _user = user;
            _notifications = notifications;
            _waitlist = waitlist;
        }

        public int UseCaseId => UseCaseIds.CancelReservation;
        public string UseCaseName => "Cancel reservation";
        public string UseCaseDescription => "Customer cancels own reservation";

        public void Execute(int reservationId)
        {
            var reservation = Context.Reservation
                .Include(x => x.Trip)
                .FirstOrDefault(x => x.ReservationId == reservationId && x.UserId == _user.Id && x.IsActive);

            if (reservation == null)
            {
                throw new EntityNotFoundException("Reservation", reservationId);
            }

            if (!BookingPricing.HoldsCapacity(reservation.Status))
            {
                throw new ConflictException($"This reservation is already {reservation.Status.ToString().ToLower()}.");
            }

            var today = DateTime.Today;
            if (!CancellationRules.CanCancel(reservation.Status, reservation.Trip.StartDate, today))
            {
                throw new ConflictException("The trip has already departed, so it can no longer be cancelled.");
            }

            var feePercent = CancellationRules.FeePercent(reservation.Status, reservation.Trip.StartDate, today);
            var fee = CancellationRules.Fee(reservation.TotalPrice, feePercent);

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationFee = fee;
            reservation.StatusChangedAt = DateTime.UtcNow;
            reservation.StatusChanges.Add(new ReservationStatusChange
            {
                Status = ReservationStatus.Cancelled,
                ChangedAt = reservation.StatusChangedAt.Value,
                ChangedByUserId = _user.Id,
                Comment = fee > 0
                    ? $"Cancelled by the customer. Cancellation fee {feePercent}%: {BookingPricing.Euro(fee)}."
                    : "Cancelled by the customer, free of charge."
            });

            _notifications.SendToAgency(reservation.Trip.AgencyId, NotificationType.ReservationCancelled,
                $"Reservation {reservation.Code} cancelled",
                $"{_user.Identity} cancelled the reservation for {reservation.Trip.TripName}"
                    + (fee > 0 ? $" (cancellation fee {BookingPricing.Euro(fee)})." : "."),
                $"/reservations/{reservation.ReservationId}");
            Context.SaveChanges();

            _waitlist.SeatsReleased(reservation.TripId);
            _notifications.Push();
        }
    }
}
