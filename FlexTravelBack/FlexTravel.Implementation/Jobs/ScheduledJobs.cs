using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FlexTravel.Implementation.Jobs
{
    public class ScheduledJobs
    {
        private static readonly SemaphoreSlim Running = new SemaphoreSlim(1, 1);

        private readonly FlexTravelContext _context;
        private readonly WaitlistService _waitlist;
        private readonly NotificationService _notifications;
        private readonly PriceAlertService _priceAlerts;

        public ScheduledJobs(FlexTravelContext context, WaitlistService waitlist, NotificationService notifications,
            PriceAlertService priceAlerts)
        {
            _context = context;
            _waitlist = waitlist;
            _notifications = notifications;
            _priceAlerts = priceAlerts;
        }

        public ScheduledJobRunDto Run(string startedBy, DateTime now)
        {
            if (!Running.Wait(0))
            {
                throw new ConflictException("The background jobs are running right now. Try again in a moment.");
            }

            try
            {
                var run = new ScheduledJobRun { StartedAt = DateTime.UtcNow, StartedBy = startedBy };
                _context.ScheduledJobRun.Add(run);
                _context.SaveChanges();

                try
                {
                    run.ExpiredReservations = ExpireUnconfirmed(now);
                    run.ClosedWaitlists = CloseDepartedWaitlists(now.Date);
                    run.PaymentReminders = RemindPayments(now.Date);
                    run.ReviewInvitations = InviteReviews(now.Date);
                    run.PriceAlerts = _priceAlerts.CheckAll();
                    Account.SessionStore.Purge(_context, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _context.ChangeTracker.Clear();
                    run = _context.ScheduledJobRun.First(r => r.ScheduledJobRunId == run.ScheduledJobRunId);
                    run.Error = ex.Message.Length > 1000 ? ex.Message[..1000] : ex.Message;
                }

                run.FinishedAt = DateTime.UtcNow;
                _context.SaveChanges();
                _notifications.Push();

                return ToDto(run);
            }
            finally
            {
                Running.Release();
            }
        }

        public static ScheduledJobRunDto ToDto(ScheduledJobRun run) => new ScheduledJobRunDto
        {
            Id = run.ScheduledJobRunId,
            StartedAt = run.StartedAt,
            FinishedAt = run.FinishedAt,
            StartedBy = run.StartedBy,
            ExpiredReservations = run.ExpiredReservations,
            PaymentReminders = run.PaymentReminders,
            ReviewInvitations = run.ReviewInvitations,
            PriceAlerts = run.PriceAlerts,
            ClosedWaitlists = run.ClosedWaitlists,
            Error = run.Error
        };

        public int ExpireUnconfirmed(DateTime now)
        {
            var expired = _context.Reservation
                .Include(r => r.Trip)
                .Where(r => r.IsActive && r.Status == ReservationStatus.Pending)
                .ToList()
                .Where(r => PendingRules.IsExpired(r.CreatedAt, r.Trip.StartDate, now))
                .ToList();

            foreach (var reservation in expired)
            {
                reservation.Status = ReservationStatus.Expired;
                reservation.StatusChangedAt = DateTime.UtcNow;
                reservation.StatusChanges.Add(new ReservationStatusChange
                {
                    Status = ReservationStatus.Expired,
                    ChangedAt = reservation.StatusChangedAt.Value,
                    ChangedByUserId = null,
                    Comment = "Not confirmed in time, so the seats were released automatically."
                });

                _notifications.Send(reservation.UserId, NotificationType.ReservationExpired,
                    $"Reservation {reservation.Code} expired",
                    $"The agency did not confirm your reservation for {reservation.Trip.TripName} in time, so it was released. "
                        + "Nothing is charged; you can book again while seats are available.",
                    $"/reservations/{reservation.ReservationId}");
                _notifications.SendToAgency(reservation.Trip.AgencyId, NotificationType.ReservationExpired,
                    $"Reservation {reservation.Code} expired",
                    $"The reservation for {reservation.Trip.TripName} was not confirmed within {BookingRules.PendingExpiryDays} days, so its seats are on sale again.",
                    $"/reservations/{reservation.ReservationId}");
            }

            _context.SaveChanges();

            foreach (var tripId in expired.Select(r => r.TripId).Distinct())
            {
                _waitlist.SeatsReleased(tripId);
            }

            return expired.Count;
        }

        public int CloseDepartedWaitlists(DateTime today)
        {
            var entries = _context.WaitlistEntry
                .Where(w => w.IsActive && w.Trip.StartDate <= today)
                .ToList();

            foreach (var entry in entries)
            {
                entry.IsActive = false;
            }

            _context.SaveChanges();
            return entries.Count;
        }

        public int RemindPayments(DateTime today)
        {
            var reservations = _context.Reservation
                .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed && r.Trip.StartDate > today
                    && (r.DepositReminderSentAt == null || r.BalanceReminderSentAt == null))
                .Select(r => new
                {
                    Reservation = r,
                    r.Trip.TripName,
                    r.Trip.StartDate,
                    Paid = r.Payments.Sum(p => (decimal?)p.Amount) ?? 0,
                    ConfirmedAt = r.StatusChanges
                        .Where(c => c.Status == ReservationStatus.Confirmed)
                        .Min(c => (DateTime?)c.ChangedAt)
                })
                .ToList();

            var sent = 0;
            foreach (var item in reservations)
            {
                var r = item.Reservation;
                var summary = PaymentRules.Summary(r.Status, r.TotalPrice, r.CancellationFee, item.StartDate,
                    item.ConfirmedAt, item.Paid, today);

                bool DueSoon(DateTime? date) =>
                    date.HasValue && date.Value.Date >= today && (date.Value.Date - today).Days <= BookingRules.PaymentReminderDaysBefore;

                if (r.DepositReminderSentAt == null && item.Paid < summary.Deposit && DueSoon(summary.DepositDueDate))
                {
                    var amount = summary.Deposit - item.Paid;
                    r.DepositReminderSentAt = DateTime.UtcNow;

                    if (summary.DepositDueDate == summary.BalanceDueDate)
                    {
                        amount = summary.Balance;
                        r.BalanceReminderSentAt = r.DepositReminderSentAt;
                    }

                    Remind(r, item.TripName, amount, summary.DepositDueDate.Value);
                    sent++;
                }
                else if (r.BalanceReminderSentAt == null && item.Paid >= summary.Deposit && summary.Balance > 0
                    && DueSoon(summary.BalanceDueDate))
                {
                    r.BalanceReminderSentAt = DateTime.UtcNow;
                    Remind(r, item.TripName, summary.Balance, summary.BalanceDueDate.Value);
                    sent++;
                }
            }

            _context.SaveChanges();
            return sent;
        }

        private void Remind(Reservation reservation, string tripName, decimal amount, DateTime dueDate)
        {
            var due = dueDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
            _notifications.Send(reservation.UserId, NotificationType.PaymentReminder,
                $"Payment due {due}",
                $"Please pay {BookingPricing.Euro(amount)} for {tripName} (reservation {reservation.Code}) by {due}.",
                $"/reservations/{reservation.ReservationId}");
        }

        public int InviteReviews(DateTime today)
        {
            var since = today.AddDays(-BookingRules.ReviewInvitationWithinDays);
            var reservations = _context.Reservation
                .Include(r => r.Trip)
                .Where(r => r.IsActive && r.Status == ReservationStatus.Confirmed
                    && r.Trip.EndDate < today && r.Trip.EndDate >= since
                    && r.Review == null && r.ReviewInvitationSentAt == null)
                .ToList();

            foreach (var reservation in reservations)
            {
                reservation.ReviewInvitationSentAt = DateTime.UtcNow;
                _notifications.Send(reservation.UserId, NotificationType.ReviewInvitation,
                    $"How was {reservation.Trip.TripName}?",
                    "Welcome back! Rate the trip and the hotel to help other travelers choose.",
                    $"/reservations/{reservation.ReservationId}");
            }

            _context.SaveChanges();
            return reservations.Count;
        }
    }
}
