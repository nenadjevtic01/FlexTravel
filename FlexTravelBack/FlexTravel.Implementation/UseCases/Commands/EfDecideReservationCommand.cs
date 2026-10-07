using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
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
    public class EfDecideReservationCommand : EfUseCase, IDecideReservationCommand
    {
        private readonly CurrentAgency _agency;
        private readonly IUser _user;
        private readonly NotificationService _notifications;
        private readonly WaitlistService _waitlist;

        public EfDecideReservationCommand(FlexTravelContext context, CurrentAgency agency, IUser user,
            NotificationService notifications, WaitlistService waitlist) : base(context)
        {
            _agency = agency;
            _user = user;
            _notifications = notifications;
            _waitlist = waitlist;
        }

        public int UseCaseId => UseCaseIds.DecideReservation;
        public string UseCaseName => "Decide reservation";
        public string UseCaseDescription => "Agency confirms or rejects a reservation";

        public void Execute(DecideReservationDto request)
        {
            var agencyId = _agency.Id(UseCaseName);

            var reservation = Context.Reservation
                .Include(x => x.Trip)
                .FirstOrDefault(x => x.ReservationId == request.ReservationId && x.Trip.AgencyId == agencyId && x.IsActive);

            if (reservation == null)
            {
                throw new EntityNotFoundException("Reservation", request.ReservationId);
            }

            if (reservation.Status != ReservationStatus.Pending)
            {
                throw new ConflictException($"Only pending reservations can be decided; this one is {reservation.Status.ToString().ToLower()}.");
            }

            var reason = string.IsNullOrWhiteSpace(request.Reason) ? null : request.Reason.Trim();
            if (reason != null && reason.Length > 500)
            {
                reason = reason[..500];
            }

            reservation.Status = request.Confirm ? ReservationStatus.Confirmed : ReservationStatus.Rejected;
            reservation.StatusChangedAt = DateTime.UtcNow;

            reservation.StatusChanges.Add(new ReservationStatusChange
            {
                Status = reservation.Status,
                ChangedAt = reservation.StatusChangedAt.Value,
                ChangedByUserId = _user.Id,
                Comment = reason
            });

            if (request.Confirm)
            {
                _notifications.Send(reservation.UserId, NotificationType.ReservationConfirmed,
                    $"Reservation {reservation.Code} confirmed",
                    $"Your trip {reservation.Trip.TripName} is confirmed. Have a great journey!",
                    $"/reservations/{reservation.ReservationId}");
            }
            else
            {
                _notifications.Send(reservation.UserId, NotificationType.ReservationRejected,
                    $"Reservation {reservation.Code} rejected",
                    reason == null
                        ? $"The agency could not confirm your reservation for {reservation.Trip.TripName}."
                        : $"The agency could not confirm your reservation for {reservation.Trip.TripName}: {reason}",
                    $"/reservations/{reservation.ReservationId}");
            }

            Context.SaveChanges();

            if (!request.Confirm)
            {
                _waitlist.SeatsReleased(reservation.TripId);
            }

            _notifications.Push();
        }
    }
}
