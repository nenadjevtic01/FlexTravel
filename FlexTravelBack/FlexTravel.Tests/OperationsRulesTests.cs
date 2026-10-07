using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.UseCases.Queries;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Tests
{
    public class OperationsRulesTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private readonly RecordingPublisher _publisher = new RecordingPublisher();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private NotificationService Notifications() => new NotificationService(_db.Context, _publisher);

        private BookingRequestDto Request(Trip trip, int adults, int children, List<TravelerDto> travelers) => new BookingRequestDto
        {
            TripId = trip.TripId,
            AccommodationId = World.Hotel.AccommodationId,
            Adults = adults,
            Children = children,
            Rooms = new List<BookingRoomDto> { new BookingRoomDto { RoomId = World.FamilyRoom.RoomId, Count = 1 } },
            Travelers = travelers
        };

        private CreatedReservationDto Book(BookingRequestDto request) =>
            new EfCreateReservationCommand(_db.Context, new TestUser(World.Customer), new BookingCalculator(_db.Context),
                new BookingRequestValidator(), Notifications()).Execute(request);

        private int Pay(int reservationId, decimal amount, bool refund = false, User employee = null)
        {
            var user = new TestUser(employee ?? World.Employee);
            return new EfRecordPaymentCommand(_db.Context, new CurrentAgency(_db.Context, user), user, Notifications())
                .Execute(new RecordPaymentDto { ReservationId = reservationId, Amount = amount, Method = "Card", IsRefund = refund });
        }

        private static TravelerDto Person(string name, int ageOnDeparture, DateTime departure) =>
            new TravelerDto { FirstName = name, LastName = "Test", DateOfBirth = departure.AddYears(-ageOnDeparture).AddDays(-10) };

        [Fact]
        public void Every_traveler_needs_a_name_and_the_ages_must_match_the_booked_children()
        {
            var trip = World.Trip();
            var departure = trip.StartDate;

            Assert.Throws<ValidationException>(() => Book(Request(trip, 2, 1, new List<TravelerDto>
            {
                Person("Ana", 35, departure), Person("Marko", 36, departure)
            })));

            var error = Assert.Throws<ValidationException>(() => Book(Request(trip, 2, 2, new List<TravelerDto>
            {
                Person("Ana", 35, departure), Person("Marko", 36, departure), Person("Mia", 8, departure), Person("Luka", 14, departure)
            })));
            Assert.Contains("1 child", error.Message);

            var created = Book(Request(trip, 2, 2, new List<TravelerDto>
            {
                Person("Ana", 35, departure), Person("Marko", 36, departure), Person("Mia", 8, departure), Person("Luka", 11, departure)
            }));

            var travelers = _db.NewContext().ReservationTraveler.Where(t => t.ReservationId == created.Id).ToList();
            Assert.Equal(4, travelers.Count);
            Assert.Equal(2, travelers.Count(t => t.IsChild));
        }

        [Fact]
        public void The_passenger_list_shows_every_traveler_of_active_reservations()
        {
            var trip = World.Trip();
            World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
            World.Reserve(trip, World.OtherCustomer, 1, children: 1);
            World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Cancelled);

            var manifest = new EfGetTripManifestQuery(_db.Context, new CurrentAgency(_db.Context, new TestUser(World.Employee)))
                .Execute(trip.TripId);

            Assert.Equal(4, manifest.Travelers);
            Assert.Equal(2, manifest.Reservations.Count());
            Assert.Throws<EntityNotFoundException>(() =>
                new EfGetTripManifestQuery(_db.Context, new CurrentAgency(_db.Context, new TestUser(World.OtherEmployee))).Execute(trip.TripId));
        }

        [Fact]
        public void Payment_status_follows_deposit_and_balance()
        {
            var trip = World.Trip(startsInDays: 60);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);

            PaymentSummaryDto Summary()
            {
                var r = _db.NewContext().Reservation.Include(x => x.Payments).Single(x => x.ReservationId == reservation.ReservationId);
                return PaymentRules.Summary(r.Status, r.TotalPrice, r.CancellationFee, trip.StartDate, DateTime.UtcNow,
                    r.Payments.Sum(p => p.Amount), DateTime.Today);
            }

            Assert.Equal("Unpaid", Summary().Status);
            Assert.Equal(300, Summary().Deposit);

            Pay(reservation.ReservationId, 300);
            Assert.Equal("Deposit paid", Summary().Status);

            Assert.Throws<ConflictException>(() => Pay(reservation.ReservationId, 701));
            Pay(reservation.ReservationId, 700);
            Assert.Equal("Paid", Summary().Status);
            Assert.Equal(0, Summary().Balance);

            Assert.Contains(_publisher.Published, p => p.UserId == World.Customer.UserId && p.Notification.Type == "PaymentRecorded");
        }

        [Fact]
        public void The_balance_is_due_three_weeks_before_departure_and_late_payment_is_overdue()
        {
            var start = DateTime.Today.AddDays(10);
            var confirmed = DateTime.Today.AddDays(-30);

            var summary = PaymentRules.Summary(ReservationStatus.Confirmed, 1000, null, start, confirmed, 300, DateTime.Today);

            Assert.Equal(start.AddDays(-21), summary.BalanceDueDate);
            Assert.True(summary.IsOverdue);
        }

        [Fact]
        public void After_a_cancellation_the_customer_gets_back_what_was_paid_above_the_fee()
        {
            var trip = World.Trip(startsInDays: 20);
            var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
            Pay(reservation.ReservationId, 1000);

            var notifications = Notifications();
            new EfCancelReservationCommand(_db.Context, new TestUser(World.Customer), notifications,
                new WaitlistService(_db.Context, new BookingCalculator(_db.Context), notifications)).Execute(reservation.ReservationId);

            Assert.Throws<ConflictException>(() => Pay(reservation.ReservationId, 701, refund: true));
            Pay(reservation.ReservationId, 700, refund: true);
            Assert.Throws<ConflictException>(() => Pay(reservation.ReservationId, 1, refund: true));

            Assert.Equal(300, _db.NewContext().Payment.Where(p => p.ReservationId == reservation.ReservationId).Sum(p => p.Amount));
        }

        [Fact]
        public void Only_the_trips_agency_takes_payments_and_only_after_confirming()
        {
            var trip = World.Trip();
            var pending = World.Reserve(trip, World.Customer, 2);
            var confirmed = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);

            Assert.Throws<ConflictException>(() => Pay(pending.ReservationId, 100));
            Assert.Throws<EntityNotFoundException>(() => Pay(confirmed.ReservationId, 100, employee: World.OtherEmployee));
        }

        [Fact]
        public void Messages_go_between_the_customer_and_the_agency_and_are_marked_read_when_opened()
        {
            var trip = World.Trip();
            var reservation = World.Reserve(trip, World.Customer, 2);

            void Send(User user, string text) =>
                new EfSendReservationMessageCommand(_db.Context, new ReservationAccess(_db.Context, new TestUser(user)), new TestUser(user), Notifications())
                    .Execute(new SendMessageDto { ReservationId = reservation.ReservationId, Text = text });

            List<ReservationMessageDto> Read(User user) =>
                new EfGetReservationMessagesQuery(_db.Context, new ReservationAccess(_db.Context, new TestUser(user)), new TestUser(user))
                    .Execute(reservation.ReservationId).ToList();

            Send(World.Customer, "Is the airport transfer included?");
            Assert.Contains(_publisher.Published, p => p.UserId == World.Employee.UserId && p.Notification.Type == "NewMessage");

            Assert.Throws<EntityNotFoundException>(() => Send(World.OtherCustomer, "Hello?"));
            Assert.Throws<EntityNotFoundException>(() => Read(World.OtherEmployee));

            var seenByAgency = Read(World.Employee);
            Assert.False(Assert.Single(seenByAgency).FromAgency);

            Send(World.Employee, "Yes, it is.");
            var seenByCustomer = Read(World.Customer);
            Assert.Equal(2, seenByCustomer.Count);
            Assert.True(seenByCustomer[1].FromAgency);
            Assert.True(_db.NewContext().ReservationMessage.All(m => m.ReadAt != null));
        }

        [Fact]
        public void A_new_departure_copies_the_trip_and_shares_its_reviews()
        {
            var category = new AttractionCategory { AttractionCategoryName = "Landmark" };
            var sight = new Attraction { AttractionName = "Bridge", AttractionDescription = "Old.", AttractionCategory = category, Destination = World.Destination };
            var trip = World.Trip(nights: 4, price: 450, configure: t =>
            {
                t.Attractions.Add(new TripAttraction { Attraction = sight });
                t.Itinerary.ItineraryActivities.Add(new ItineraryActivity
                {
                    Day = 1,
                    Title = "Arrival",
                    Activity = "Arrival.",
                    Meals = Meals.Dinner,
                    Overnight = "Testville"
                });
                t.Extras.Add(new TripExtra { Kind = ExtraKind.Insurance, Name = "Insurance", Price = 20, PerPerson = true });
                t.Extras.Add(new TripExtra { Kind = ExtraKind.Transfer, Name = "Old transfer", Price = 30, IsActive = false });
            });

            var user = new TestUser(World.Employee);
            var notifications = Notifications();
            var priceAlerts = new PriceAlertService(_db.Context, new EfSearchTripsQuery(_db.Context), notifications);
            var command = new EfAddDepartureCommand(_db.Context, new CurrentAgency(_db.Context, user), user, notifications, priceAlerts);
            var id = command.Execute(new AddDepartureDto { TripId = trip.TripId, StartDate = DateTime.Today.AddDays(90) });

            var copy = _db.NewContext().Trip
                .Include(t => t.Prices).Include(t => t.Attractions).Include(t => t.Accommodations).Include(t => t.Extras)
                .Include(t => t.Itinerary).ThenInclude(i => i.ItineraryActivities)
                .Single(t => t.TripId == id);
            Assert.Equal(trip.TripSeriesId, copy.TripSeriesId);
            Assert.Equal(4, (copy.EndDate - copy.StartDate).Days);
            Assert.Equal(450, copy.Prices.Single().Price);
            Assert.Single(copy.Attractions);
            Assert.Single(copy.Accommodations);
            var day = copy.Itinerary.ItineraryActivities.Single();
            Assert.Equal(("Arrival", "Arrival.", Meals.Dinner, "Testville"), (day.Title, day.Activity, day.Meals, day.Overnight));

            Assert.Equal("Insurance", copy.Extras.Single().Name);

            Assert.Throws<ConflictException>(() => command.Execute(new AddDepartureDto { TripId = trip.TripId, StartDate = DateTime.Today.AddDays(90) }));
            Assert.Throws<ValidationException>(() => command.Execute(new AddDepartureDto { TripId = trip.TripId, StartDate = DateTime.Today }));
        }

        [Fact]
        public void Hotels_and_destinations_on_trips_for_sale_cannot_be_hidden()
        {
            var trip = World.Trip();

            Assert.Throws<ConflictException>(() => new EfSetHotelActiveCommand(_db.Context)
                .Execute(new SetActiveDto { Id = World.Hotel.AccommodationId, IsActive = false }));
            Assert.Throws<ConflictException>(() => new EfSetDestinationActiveCommand(_db.Context)
                .Execute(new SetActiveDto { Id = World.Destination.DestinationId, IsActive = false }));

            trip.IsActive = false;
            _db.Context.SaveChanges();

            new EfSetHotelActiveCommand(_db.Context).Execute(new SetActiveDto { Id = World.Hotel.AccommodationId, IsActive = false });
            Assert.False(_db.NewContext().Accommodation.Single().IsActive);
        }
    }
}
