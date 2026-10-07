using FlexTravel.Api.Core;
using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FlexTravel.Implementation.Account;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Jobs;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.UseCases.Commands.Account;
using FlexTravel.Implementation.UseCases.Queries;
using FlexTravel.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Tests
{
    public class JobsAndSessionsTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private readonly RecordingPublisher _publisher = new RecordingPublisher();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private ScheduledJobs Jobs()
        {
            var notifications = new NotificationService(_db.Context, _publisher);
            var calculator = new BookingCalculator(_db.Context);
            return new ScheduledJobs(_db.Context, new WaitlistService(_db.Context, calculator, notifications), notifications,
                new PriceAlertService(_db.Context, new EfSearchTripsQuery(_db.Context), notifications));
        }

        private List<string> Pushed(User user) =>
            _publisher.Published.Where(p => p.UserId == user.UserId).Select(p => p.Notification.Type).ToList();

        private void Age(Reservation reservation, int days)
        {
            reservation.CreatedAt = DateTime.Now.AddDays(-days);
            foreach (var change in reservation.StatusChanges)
            {
                change.ChangedAt = DateTime.UtcNow.AddDays(-days);
            }

            _db.Context.SaveChanges();
        }

        [Fact]
        public void An_unconfirmed_reservation_expires_and_its_seats_go_to_the_waitlist()
        {
            var trip = World.Trip(capacity: 2);
            var old = World.Reserve(trip, World.Customer, 2);
            Age(old, BookingRulesDays + 1);
            _db.Context.WaitlistEntry.Add(new WaitlistEntry { Trip = trip, User = World.OtherCustomer, Adults = 2, CreatedAt = DateTime.Now });
            var recent = World.Reserve(World.Trip(), World.OtherCustomer, 1);

            var run = Jobs().Run("Test", DateTime.Now);

            Assert.Equal(1, run.ExpiredReservations);
            var expired = _db.NewContext().Reservation.Include(r => r.StatusChanges).Single(r => r.ReservationId == old.ReservationId);
            Assert.Equal(ReservationStatus.Expired, expired.Status);
            Assert.Null(expired.StatusChanges.OrderBy(c => c.ChangedAt).Last().ChangedByUserId);
            Assert.Equal(ReservationStatus.Pending, _db.NewContext().Reservation.Single(r => r.ReservationId == recent.ReservationId).Status);

            Assert.Contains(nameof(NotificationType.ReservationExpired), Pushed(World.Customer));
            Assert.Contains(nameof(NotificationType.ReservationExpired), Pushed(World.Employee));
            Assert.Contains(nameof(NotificationType.SeatsAvailable), Pushed(World.OtherCustomer));

            Assert.Equal(0, PaymentRules.Summary(ReservationStatus.Expired, 1000, null, trip.StartDate, null, 0, DateTime.Today).AmountDue);
            Assert.Equal(0, Jobs().Run("Test", DateTime.Now).ExpiredReservations);
        }

        private const int BookingRulesDays = FlexTravel.Application.Constants.BookingRules.PendingExpiryDays;

        [Fact]
        public void A_reservation_booked_just_before_departure_expires_on_the_day_of_departure()
        {
            var bookedAt = new DateTime(2030, 5, 10, 18, 0, 0);
            var start = new DateTime(2030, 5, 11);

            Assert.Equal(start, PendingRules.DecideBy(bookedAt, start));
            Assert.False(PendingRules.IsExpired(bookedAt, start, start.AddMinutes(-1)));
            Assert.True(PendingRules.IsExpired(bookedAt, start, start));
            Assert.Equal(bookedAt.AddDays(BookingRulesDays), PendingRules.DecideBy(bookedAt, start.AddDays(30)));
        }

        [Fact]
        public void Customers_are_reminded_once_before_a_payment_is_due()
        {
            var trip = World.Trip(startsInDays: 60);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
            Age(reservation, 5);

            Assert.Equal(1, Jobs().Run("Test", DateTime.Now).PaymentReminders);
            Assert.Equal(0, Jobs().Run("Test", DateTime.Now).PaymentReminders);
            Assert.Contains(nameof(NotificationType.PaymentReminder), Pushed(World.Customer));

            reservation.Payments.Add(new Payment
            {
                Amount = 300,
                Method = PaymentMethod.Card,
                PaidAt = DateTime.Now,
                RecordedByUserId = World.Employee.UserId
            });
            _db.Context.SaveChanges();
            var jobs = Jobs();
            Assert.Equal(0, jobs.RemindPayments(DateTime.Today));
            Assert.Equal(1, jobs.RemindPayments(trip.StartDate.AddDays(-21 - 2)));
        }

        [Fact]
        public void Travelers_back_from_a_trip_are_invited_to_review_it_once()
        {
            var recent = World.Reserve(World.Trip(startsInDays: -8, nights: 5), World.Customer, 2, status: ReservationStatus.Confirmed);
            World.Reserve(World.Trip(startsInDays: -60, nights: 5), World.OtherCustomer, 2, status: ReservationStatus.Confirmed);
            World.Reserve(World.Trip(startsInDays: -8, nights: 5), World.OtherCustomer, 1);

            Assert.Equal(1, Jobs().Run("Test", DateTime.Now).ReviewInvitations);
            Assert.Equal(0, Jobs().Run("Test", DateTime.Now).ReviewInvitations);
            Assert.NotNull(_db.NewContext().Reservation.Single(r => r.ReservationId == recent.ReservationId).ReviewInvitationSentAt);
        }

        [Fact]
        public void Waiting_for_seats_ends_when_the_trip_departs()
        {
            var departed = World.Trip(startsInDays: 0);
            var upcoming = World.Trip(startsInDays: 10);
            _db.Context.WaitlistEntry.AddRange(
                new WaitlistEntry { Trip = departed, User = World.Customer, Adults = 1, CreatedAt = DateTime.Now },
                new WaitlistEntry { Trip = upcoming, User = World.Customer, Adults = 1, CreatedAt = DateTime.Now });
            _db.Context.SaveChanges();

            Assert.Equal(1, Jobs().Run("Test", DateTime.Now).ClosedWaitlists);
            Assert.True(_db.NewContext().WaitlistEntry.Single(w => w.TripId == upcoming.TripId).IsActive);
        }

        [Fact]
        public void Every_run_is_recorded()
        {
            Jobs().Run("Test", DateTime.Now);
            var run = _db.NewContext().ScheduledJobRun.Single();

            Assert.Equal("Test", run.StartedBy);
            Assert.NotNull(run.FinishedAt);
            Assert.Null(run.Error);
        }

        [Fact]
        public void A_refresh_token_is_replaced_on_every_use()
        {
            var sessions = new SessionStore(_db.Context);
            var first = sessions.Issue(World.Customer.UserId, rememberMe: true, DateTime.UtcNow);

            var second = sessions.Rotate(first, DateTime.UtcNow);
            Assert.NotNull(second);
            Assert.NotEqual(first, second.Value.Token);
            Assert.True(second.Value.RememberMe);

            Assert.DoesNotContain(_db.NewContext().RefreshToken, t => t.TokenHash == first || t.TokenHash == second.Value.Token);

            Assert.Null(sessions.Rotate(first, DateTime.UtcNow));
            Assert.NotNull(sessions.Rotate(second.Value.Token, DateTime.UtcNow));
        }

        [Fact]
        public void A_stolen_refresh_token_used_later_ends_all_sessions()
        {
            var sessions = new SessionStore(_db.Context);
            var stolen = sessions.Issue(World.Customer.UserId, rememberMe: false, DateTime.UtcNow);
            var current = sessions.Rotate(stolen, DateTime.UtcNow).Value.Token;
            var otherDevice = sessions.Issue(World.Customer.UserId, rememberMe: true, DateTime.UtcNow);

            Assert.Null(sessions.Rotate(stolen, DateTime.UtcNow.Add(SessionStore.ReuseGrace).AddSeconds(1)));

            Assert.Null(sessions.Rotate(current, DateTime.UtcNow));
            Assert.Null(sessions.Rotate(otherDevice, DateTime.UtcNow));
        }

        [Fact]
        public void Sessions_end_on_sign_out_expiry_or_a_blocked_account()
        {
            var sessions = new SessionStore(_db.Context);
            var now = DateTime.UtcNow;

            var signedOut = sessions.Issue(World.Customer.UserId, false, now);
            sessions.Revoke(signedOut, now);
            Assert.Null(sessions.Rotate(signedOut, now));

            var shortSession = sessions.Issue(World.Customer.UserId, false, now);
            Assert.Null(sessions.Rotate(shortSession, now.AddDays(1).AddMinutes(1)));

            var blocked = sessions.Issue(World.OtherCustomer.UserId, true, now);
            World.OtherCustomer.IsActive = false;
            _db.Context.SaveChanges();
            Assert.Null(sessions.Rotate(blocked, now));
            Assert.Null(sessions.Rotate("not-a-token", now));
        }

        [Fact]
        public void Changing_the_password_signs_out_the_other_sessions()
        {
            World.Customer.Password = BCrypt.Net.BCrypt.HashPassword("Old-password1");
            _db.Context.SaveChanges();
            var sessions = new SessionStore(_db.Context);
            var here = sessions.Issue(World.Customer.UserId, true, DateTime.UtcNow);
            var elsewhere = sessions.Issue(World.Customer.UserId, true, DateTime.UtcNow);

            new EfChangePasswordCommand(_db.Context, new TestUser(World.Customer), new ChangePasswordValidator())
                .Execute(new ChangePasswordDto { CurrentPassword = "Old-password1", NewPassword = "New-password2", KeepRefreshToken = here });

            Assert.NotNull(sessions.Rotate(here, DateTime.UtcNow));
            Assert.Null(sessions.Rotate(elsewhere, DateTime.UtcNow));
        }

        [Fact]
        public void Signing_in_returns_a_short_access_token_and_a_refresh_token()
        {
            World.Customer.Password = BCrypt.Net.BCrypt.HashPassword("Password1");
            _db.Context.SaveChanges();
            var jwt = new JwtManager(_db.NewContext(), new JwtSettings
            {
                Issuer = "tests",
                SecretKey = "a-secret-key-that-is-long-enough-for-hmac-sha256",
                Minutes = 15
            });

            var pair = jwt.SignIn(World.Customer.Email, "Password1", rememberMe: false);
            Assert.NotEmpty(pair.Token);
            Assert.InRange(pair.ExpiresAt, DateTime.UtcNow.AddMinutes(14), DateTime.UtcNow.AddMinutes(16));

            var next = jwt.Refresh(pair.RefreshToken);
            Assert.NotNull(next);
            Assert.NotEqual(pair.RefreshToken, next.RefreshToken);

            jwt.SignOut(next.RefreshToken);
            Assert.Null(jwt.Refresh(next.RefreshToken));
        }
    }
}
