using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;

namespace FlexTravel.Tests
{
    public class ReservationRulesTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private readonly RecordingPublisher _publisher = new RecordingPublisher();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private NotificationService Notifications() => new NotificationService(_db.Context, _publisher);

        private WaitlistService Waitlist(NotificationService notifications) =>
            new WaitlistService(_db.Context, new BookingCalculator(_db.Context), notifications);

        public static List<TravelerDto> Adults(int count) => Enumerable.Range(1, count)
            .Select(i => new TravelerDto { FirstName = "Adult", LastName = i.ToString(), DateOfBirth = DateTime.Today.AddYears(-30 - i) })
            .ToList();

        private CreatedReservationDto Book(User customer, Trip trip, int adults, int doubleRooms, string promoCode = null)
        {
            var command = new EfCreateReservationCommand(_db.Context, new TestUser(customer), new BookingCalculator(_db.Context),
                new BookingRequestValidator(), Notifications());

            return command.Execute(new BookingRequestDto
            {
                TripId = trip.TripId,
                AccommodationId = World.Hotel.AccommodationId,
                Adults = adults,
                Rooms = new List<BookingRoomDto> { new BookingRoomDto { RoomId = World.DoubleRoom.RoomId, Count = doubleRooms } },
                PromoCode = promoCode,
                Travelers = Adults(adults)
            });
        }

        private void Cancel(User customer, int reservationId)
        {
            var notifications = Notifications();
            new EfCancelReservationCommand(_db.Context, new TestUser(customer), notifications, Waitlist(notifications)).Execute(reservationId);
        }

        private void Decide(User employee, int reservationId, bool confirm, string reason = null)
        {
            var user = new TestUser(employee);
            var notifications = Notifications();
            new EfDecideReservationCommand(_db.Context, new CurrentAgency(_db.Context, user), user, notifications, Waitlist(notifications))
                .Execute(new DecideReservationDto { ReservationId = reservationId, Confirm = confirm, Reason = reason });
        }

        private void JoinWaitlist(User customer, Trip trip, int adults, int children = 0) =>
            new EfJoinWaitlistCommand(_db.Context, new TestUser(customer), new BookingCalculator(_db.Context), new JoinWaitlistValidator())
                .Execute(new JoinWaitlistDto { TripId = trip.TripId, Adults = adults, Children = children });

        [Fact]
        public void A_reservation_keeps_the_prices_and_discounts_of_the_moment_it_was_made()
        {
            var trip = World.Trip(price: 500, nights: 5);
            var promo = World.Promo("TEN", 10);

            var created = Book(World.Customer, trip, adults: 2, doubleRooms: 1, promoCode: "ten");

            trip.Prices.Add(new TripPrice { Price = 900, ValidFrom = DateTime.Now });
            _db.Context.SaveChanges();

            var reservation = _db.NewContext().Reservation.Single(r => r.ReservationId == created.Id);
            Assert.Equal(500, reservation.PricePerPerson);
            Assert.Equal(1500, reservation.Subtotal);
            Assert.Equal(promo.PromoCodeId, reservation.PromoCodeId);
            Assert.Equal(150, reservation.DiscountTotal);
            Assert.Equal(1350, reservation.TotalPrice);
            Assert.Equal(ReservationStatus.Pending, reservation.Status);
        }

        [Fact]
        public void The_agency_is_notified_live_about_a_new_reservation()
        {
            var trip = World.Trip();

            var created = Book(World.Customer, trip, adults: 2, doubleRooms: 1);

            var (userId, notification) = Assert.Single(_publisher.Published);
            Assert.Equal(World.Employee.UserId, userId);
            Assert.Equal("ReservationCreated", notification.Type);
            Assert.Equal($"/reservations/{created.Id}", notification.Link);
            Assert.True(notification.Id > 0, "Only saved notifications are pushed.");
        }

        [Fact]
        public void An_invalid_promo_code_stops_the_booking()
        {
            var trip = World.Trip();
            World.Promo("OLD", 10, validFromDays: -20, validToDays: -1);

            var error = Assert.Throws<ConflictException>(() => Book(World.Customer, trip, 2, 1, promoCode: "OLD"));

            Assert.Contains("expired", error.Message);
            Assert.Empty(_db.NewContext().Reservation);
        }

        [Fact]
        public void The_last_seats_cannot_be_sold_twice()
        {
            var trip = World.Trip(capacity: 2);
            Book(World.OtherCustomer, trip, adults: 2, doubleRooms: 1);

            var error = Assert.Throws<ConflictException>(() => Book(World.Customer, trip, adults: 2, doubleRooms: 1));

            Assert.Equal("This trip is sold out.", error.Message);
        }

        [Theory]
        [InlineData(40, 0)]
        [InlineData(30, 0)]
        [InlineData(29, 30)]
        [InlineData(15, 30)]
        [InlineData(10, 50)]
        [InlineData(7, 50)]
        [InlineData(3, 100)]
        public void Cancelling_a_confirmed_reservation_costs_more_closer_to_departure(int daysBefore, int feePercent)
        {
            var trip = World.Trip(startsInDays: daysBefore);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);

            Cancel(World.Customer, reservation.ReservationId);

            var saved = _db.NewContext().Reservation.Single(r => r.ReservationId == reservation.ReservationId);
            Assert.Equal(ReservationStatus.Cancelled, saved.Status);
            Assert.Equal(1000m * feePercent / 100, saved.CancellationFee);
        }

        [Fact]
        public void A_reservation_the_agency_has_not_confirmed_is_cancelled_for_free()
        {
            var trip = World.Trip(startsInDays: 3);
            var reservation = World.Reserve(trip, World.Customer, 2);

            Cancel(World.Customer, reservation.ReservationId);

            Assert.Equal(0, _db.NewContext().Reservation.Single(r => r.ReservationId == reservation.ReservationId).CancellationFee);
        }

        [Fact]
        public void A_departed_trip_cannot_be_cancelled()
        {
            var trip = World.Trip(startsInDays: 0);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);

            Assert.Throws<ConflictException>(() => Cancel(World.Customer, reservation.ReservationId));
        }

        [Fact]
        public void A_customer_cannot_cancel_someone_elses_reservation()
        {
            var trip = World.Trip();
            var reservation = World.Reserve(trip, World.OtherCustomer, 2);

            Assert.Throws<EntityNotFoundException>(() => Cancel(World.Customer, reservation.ReservationId));
        }

        [Fact]
        public void An_employee_decides_only_reservations_of_their_own_agency()
        {
            var trip = World.Trip();
            var reservation = World.Reserve(trip, World.Customer, 2);

            Assert.Throws<EntityNotFoundException>(() => Decide(World.OtherEmployee, reservation.ReservationId, confirm: true));

            Decide(World.Employee, reservation.ReservationId, confirm: true);
            Assert.Throws<ConflictException>(() => Decide(World.Employee, reservation.ReservationId, confirm: false));
        }

        [Fact]
        public void A_rejection_tells_the_customer_why_and_records_who_decided()
        {
            var trip = World.Trip();
            var reservation = World.Reserve(trip, World.Customer, 2);

            Decide(World.Employee, reservation.ReservationId, confirm: false, reason: "  The hotel is closed for renovation.  ");

            var context = _db.NewContext();
            var change = context.ReservationStatusChange.Where(c => c.ReservationId == reservation.ReservationId).OrderBy(c => c.ChangedAt).Last();
            Assert.Equal(ReservationStatus.Rejected, change.Status);
            Assert.Equal(World.Employee.UserId, change.ChangedByUserId);
            Assert.Equal("The hotel is closed for renovation.", change.Comment);

            var pushed = Assert.Single(_publisher.Published, p => p.UserId == World.Customer.UserId);
            Assert.Equal("ReservationRejected", pushed.Notification.Type);
            Assert.EndsWith("The hotel is closed for renovation.", pushed.Notification.Message);
        }

        [Fact]
        public void The_waitlist_is_open_only_when_there_are_not_enough_seats()
        {
            var trip = World.Trip(capacity: 4);
            World.Reserve(trip, World.OtherCustomer, 3);

            Assert.Throws<ConflictException>(() => JoinWaitlist(World.Customer, trip, adults: 1));
            JoinWaitlist(World.Customer, trip, adults: 2);
            Assert.Throws<ConflictException>(() => JoinWaitlist(World.Customer, trip, adults: 2));
            Assert.Throws<ValidationException>(() => JoinWaitlist(World.OtherCustomer, trip, adults: 0, children: 2));
        }

        [Fact]
        public void Released_seats_are_announced_to_everyone_on_the_waitlist_whose_party_fits()
        {
            var trip = World.Trip(capacity: 4);
            var blocking = World.Reserve(trip, World.OtherCustomer, 4, doubleRooms: 2);
            JoinWaitlist(World.Customer, trip, adults: 2);

            Cancel(World.OtherCustomer, blocking.ReservationId);

            var pushed = Assert.Single(_publisher.Published, p => p.UserId == World.Customer.UserId);
            Assert.Equal("SeatsAvailable", pushed.Notification.Type);
            Assert.NotNull(_db.NewContext().WaitlistEntry.Single().NotifiedAt);

            Assert.Contains(_publisher.Published, p => p.UserId == World.Employee.UserId && p.Notification.Type == "ReservationCancelled");
        }

        [Fact]
        public void A_party_bigger_than_the_released_seats_is_not_told()
        {
            var trip = World.Trip(capacity: 4);
            var small = World.Reserve(trip, World.OtherCustomer, 1);
            World.Reserve(trip, World.OtherCustomer, 3);
            JoinWaitlist(World.Customer, trip, adults: 2);

            Decide(World.Employee, small.ReservationId, confirm: false);

            Assert.DoesNotContain(_publisher.Published, p => p.Notification.Type == "SeatsAvailable");
        }

        [Fact]
        public void Booking_the_trip_takes_the_customer_off_the_waitlist()
        {
            var trip = World.Trip(capacity: 4);
            var blocking = World.Reserve(trip, World.OtherCustomer, 4, doubleRooms: 2);
            JoinWaitlist(World.Customer, trip, adults: 2);
            Cancel(World.OtherCustomer, blocking.ReservationId);

            Book(World.Customer, trip, adults: 2, doubleRooms: 1);

            Assert.False(_db.NewContext().WaitlistEntry.Single().IsActive);
        }
    }
}
