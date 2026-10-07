using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Notifications;
using FlexTravel.Implementation.UseCases.Commands;
using FlexTravel.Implementation.UseCases.Queries;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Tests
{
    public class PlanningRulesTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private readonly RecordingPublisher _publisher = new RecordingPublisher();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private NotificationService Notifications() => new NotificationService(_db.Context, _publisher);

        private PriceAlertService PriceAlerts(NotificationService notifications = null) =>
            new PriceAlertService(_db.Context, new EfSearchTripsQuery(_db.Context), notifications ?? Notifications());

        private Trip TripWithExtras(Action<Trip> configure = null) => World.Trip(configure: t =>
        {
            t.Extras.Add(new TripExtra { Kind = ExtraKind.Insurance, Name = "Insurance", Price = 20, PerPerson = true });
            t.Extras.Add(new TripExtra { Kind = ExtraKind.Transfer, Name = "Private transfer", Price = 40, PerPerson = false });
            configure?.Invoke(t);
        });

        private BookingRequestDto Request(Trip trip, params BookingExtraDto[] extras) => new BookingRequestDto
        {
            TripId = trip.TripId,
            AccommodationId = World.Hotel.AccommodationId,
            Adults = 2,
            Rooms = new List<BookingRoomDto> { new BookingRoomDto { RoomId = World.DoubleRoom.RoomId, Count = 1 } },
            Extras = extras.ToList(),
            Travelers = new List<TravelerDto>
            {
                new TravelerDto { FirstName = "Cleo", LastName = "Test", DateOfBirth = new DateTime(1990, 1, 1) },
                new TravelerDto { FirstName = "Max", LastName = "Test", DateOfBirth = new DateTime(1991, 1, 1) }
            }
        };

        private static BookingExtraDto Extra(Trip trip, string name, int quantity = 1) =>
            new BookingExtraDto { ExtraId = trip.Extras.Single(e => e.Name == name).TripExtraId, Quantity = quantity };

        [Fact]
        public void Extras_are_added_after_the_discount_and_stored_with_the_reservation()
        {
            var trip = TripWithExtras(t =>
            {
                t.EarlyBookingPercent = 10;
                t.EarlyBookingUntil = DateTime.Today.AddDays(5);
            });

            var quote = new BookingCalculator(_db.Context).Quote(Request(trip, Extra(trip, "Insurance", 2), Extra(trip, "Private transfer")));

            Assert.True(quote.IsValid, string.Join(" ", quote.Problems));
            Assert.Equal(1500, quote.Subtotal);
            Assert.Equal(150, quote.OfferDiscount);
            Assert.Equal(80, quote.ExtrasTotal);
            Assert.Equal(1430, quote.Total);

            var created = new EfCreateReservationCommand(_db.Context, new TestUser(World.Customer), new BookingCalculator(_db.Context),
                new BookingRequestValidator(), Notifications()).Execute(Request(trip, Extra(trip, "Insurance", 2), Extra(trip, "Private transfer")));

            var saved = _db.NewContext().Reservation.Include(r => r.Extras).Single(r => r.ReservationId == created.Id);
            Assert.Equal(80, saved.ExtrasTotal);
            Assert.Equal(1430, saved.TotalPrice);
            Assert.Equal(new[] { ("Insurance", 2, 40m), ("Private transfer", 1, 40m) },
                saved.Extras.OrderBy(e => e.Name).Select(e => (e.Name, e.Quantity, e.Total)).ToArray());
        }

        [Fact]
        public void A_per_person_extra_is_for_one_to_all_travelers_and_withdrawn_extras_cannot_be_booked()
        {
            var trip = TripWithExtras(t =>
                t.Extras.Add(new TripExtra { Kind = ExtraKind.Excursion, Name = "Old excursion", Price = 50, PerPerson = true, IsActive = false }));
            var calculator = new BookingCalculator(_db.Context);

            Assert.False(calculator.Quote(Request(trip, Extra(trip, "Insurance", 3))).IsValid);
            Assert.False(calculator.Quote(Request(trip, Extra(trip, "Insurance", 0))).IsValid);
            Assert.False(calculator.Quote(Request(trip, Extra(trip, "Old excursion", 1))).IsValid);

            var quote = calculator.Quote(Request(trip, Extra(trip, "Private transfer", 5)));
            Assert.True(quote.IsValid);
            Assert.Equal(40, quote.ExtrasTotal);
        }

        [Fact]
        public void Changing_the_extras_of_a_trip_keeps_what_was_already_booked()
        {
            var trip = TripWithExtras();
            var insurance = trip.Extras.Single(e => e.Name == "Insurance");

            ExtraRules.Apply(trip, new[]
            {
                new TripExtraInputDto { Id = insurance.TripExtraId, Kind = "Insurance", Name = "Full insurance", Price = 25, PerPerson = true },
                new TripExtraInputDto { Kind = "Excursion", Name = "Boat trip", Price = 60, PerPerson = true }
            });
            _db.Context.SaveChanges();

            var extras = _db.NewContext().TripExtra.Where(e => e.TripId == trip.TripId).ToList();
            Assert.Equal(3, extras.Count);
            Assert.False(extras.Single(e => e.Name == "Private transfer").IsActive);
            Assert.Equal(25, extras.Single(e => e.TripExtraId == insurance.TripExtraId).Price);
            Assert.True(extras.Single(e => e.Name == "Boat trip").IsActive);
        }

        [Fact]
        public void Trip_extras_need_a_name_a_price_and_a_known_kind()
        {
            var validator = new CreateTripValidator(_db.Context);
            var request = new CreateTripDto
            {
                Name = "Trip",
                Description = "A trip.",
                DestinationId = World.Destination.DestinationId,
                TransportId = World.Bus.TransportId,
                StartDate = DateTime.Today.AddDays(30),
                EndDate = DateTime.Today.AddDays(35),
                Capacity = 10,
                PricePerPerson = 300,
                AccommodationIds = new List<int> { World.Hotel.AccommodationId },
                Itinerary = new List<ItineraryDayInputDto> { new ItineraryDayInputDto { Title = "Arrival" } },
                Extras = new List<TripExtraInputDto> { new TripExtraInputDto { Kind = "Insurance", Name = "Insurance", Price = 20 } }
            };

            Assert.True(validator.Validate(request).IsValid, string.Join(" ", validator.Validate(request).Errors));

            request.Extras[0].Kind = "Spa";
            Assert.False(validator.Validate(request).IsValid);
            request.Extras[0] = new TripExtraInputDto { Kind = "Other", Name = "", Price = 0 };
            Assert.Equal(2, validator.Validate(request).Errors.Count);
        }

        [Fact]
        public void Empty_days_are_free_days_and_the_rest_keep_their_day_numbers()
        {
            var days = new List<ItineraryDayInputDto>
            {
                new ItineraryDayInputDto { Title = "Arrival", Activity = "Flight and transfer.", Dinner = true, Overnight = "Rome" },
                new ItineraryDayInputDto(),
                new ItineraryDayInputDto { Title = "Vatican", Breakfast = true, Lunch = true }
            };

            var activities = ItineraryRules.ToActivities(days);

            Assert.Equal(new[] { 1, 3 }, activities.Select(a => a.Day));
            Assert.Equal(Meals.Dinner, activities[0].Meals);
            Assert.Equal(Meals.Breakfast | Meals.Lunch, activities[1].Meals);
            Assert.Equal("Vatican", activities[1].Activity);

            var form = ItineraryRules.ToForm(activities);
            Assert.Equal(3, form.Count);
            Assert.True(ItineraryRules.IsEmpty(form[1]));
            Assert.Equal("Rome", form[0].Overnight);
            Assert.True(form[2].Lunch);
        }

        [Fact]
        public void The_rating_breakdown_counts_stars_and_averages_only_given_details()
        {
            var trip = World.Trip(startsInDays: -10, nights: 3);
            void Review(int tripRating, int hotel, int? guide, bool visible = true)
            {
                var reservation = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
                _db.Context.Review.Add(new Review
                {
                    Reservation = reservation,
                    User = World.Customer,
                    Trip = trip,
                    Accommodation = World.Hotel,
                    TripRating = tripRating,
                    HotelRating = hotel,
                    GuideRating = guide,
                    Comment = "A review long enough.",
                    IsActive = visible
                });
                _db.Context.SaveChanges();
            }

            Review(5, 4, 5);
            Review(4, 2, null);
            Review(1, 1, 1, visible: false);

            var breakdown = Ratings.Breakdown(_db.NewContext().Review.Where(r => r.TripId == trip.TripId));

            Assert.Equal(2, breakdown.Count);
            Assert.Equal(4.5, breakdown.Average);
            Assert.Equal(new[] { 1, 1, 0, 0, 0 }, breakdown.Stars);
            Assert.Equal(3, breakdown.Categories.Single(c => c.Name == "Hotel").Average);
            Assert.Equal((5.0, 1), breakdown.Categories.Select(c => (c.Average ?? 0, c.Count)).ElementAt(1));
            Assert.DoesNotContain(breakdown.Categories, c => c.Name == "Value for money");
        }

        [Fact]
        public void A_price_alert_is_announced_once_per_lower_price()
        {
            var trip = World.Trip(price: 500);
            _db.Context.PriceAlert.Add(new PriceAlert
            {
                UserId = World.Customer.UserId,
                DestinationId = World.Destination.DestinationId,
                MaxPrice = 800,
                CreatedAt = DateTime.UtcNow
            });
            _db.Context.SaveChanges();

            var notifications = Notifications();
            var alerts = PriceAlerts(notifications);

            Assert.Equal(1, alerts.Check(World.Destination.DestinationId));
            Assert.Equal(0, alerts.Check(World.Destination.DestinationId));

            trip.Prices.Add(new TripPrice { Price = 450, ValidFrom = DateTime.Now.AddMinutes(-1) });
            _db.Context.SaveChanges();
            Assert.Equal(1, alerts.Check(World.Destination.DestinationId));

            trip.Prices.Add(new TripPrice { Price = 900, ValidFrom = DateTime.Now.AddSeconds(-30) });
            _db.Context.SaveChanges();
            Assert.Equal(0, alerts.Check(World.Destination.DestinationId));
            trip.Prices.Add(new TripPrice { Price = 500, ValidFrom = DateTime.Now.AddSeconds(-10) });
            _db.Context.SaveChanges();
            Assert.Equal(1, alerts.Check(World.Destination.DestinationId));

            notifications.Push();
            Assert.Equal(3, _publisher.Published.Count(p => p.Notification.Type == nameof(NotificationType.PriceDrop)));
        }

        [Fact]
        public void An_alert_that_is_met_when_it_is_set_is_not_announced_again()
        {
            World.Trip(price: 500);
            var user = new TestUser(World.Customer);
            var command = new EfSetPriceAlertCommand(_db.Context, user, PriceAlerts());

            command.Execute(new SetPriceAlertDto { DestinationId = World.Destination.DestinationId, MaxPrice = 1000 });
            Assert.Equal(0, PriceAlerts().Check(World.Destination.DestinationId));

            var alerts = new EfGetMyPriceAlertsQuery(_db.Context, user, PriceAlerts()).Execute(0).ToList();
            Assert.True(alerts.Single().IsMet);
            Assert.Equal(688, alerts.Single().LowestPrice);

            Assert.Throws<ValidationException>(() => command.Execute(new SetPriceAlertDto { DestinationId = World.Destination.DestinationId, MaxPrice = -5 }));
            command.Execute(new SetPriceAlertDto { DestinationId = World.Destination.DestinationId, MaxPrice = null });
            Assert.Empty(_db.NewContext().PriceAlert);
        }

        [Fact]
        public void The_calendar_shows_departures_and_what_is_due()
        {
            var trip = World.Trip(startsInDays: 40);
            World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
            World.Reserve(trip, World.OtherCustomer, 1);
            World.Trip(startsInDays: 41, agency: World.OtherAgency);

            var user = new TestUser(World.Employee);
            var calendar = new EfGetAgencyCalendarQuery(_db.Context, new CurrentAgency(_db.Context, user))
                .Execute(new AgencyCalendarRequestDto { From = DateTime.Today, To = DateTime.Today.AddDays(45) });

            var departure = calendar.Departures.Single();
            Assert.Equal(trip.TripId, departure.TripId);
            Assert.Equal(3, departure.SeatsBooked);
            Assert.Equal(1, departure.Pending);

            Assert.Equal(new[] { "Decision", "Deposit", "Balance" }, calendar.Deadlines.Select(d => d.Kind));
            Assert.Equal(300, calendar.Deadlines.Single(d => d.Kind == "Deposit").Amount);
            Assert.Equal(trip.StartDate.AddDays(-21), calendar.Deadlines.Single(d => d.Kind == "Balance").Date);
        }

        [Fact]
        public void The_report_counts_sales_cancellation_fees_and_collected_payments()
        {
            var trip = World.Trip(startsInDays: 20, capacity: 10);
            var confirmed = World.Reserve(trip, World.Customer, 2, status: ReservationStatus.Confirmed);
            var cancelled = World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Cancelled);
            cancelled.CancellationFee = 300;
            World.Reserve(trip, World.OtherCustomer, 1, status: ReservationStatus.Rejected);
            confirmed.Payments.Add(new Payment { Amount = 300, Method = PaymentMethod.Card, PaidAt = DateTime.Now, RecordedByUserId = World.Employee.UserId });
            _db.Context.SaveChanges();

            var user = new TestUser(World.Employee);
            var report = new EfGetAgencyReportQuery(_db.Context, new CurrentAgency(_db.Context, user)).Execute(0);

            Assert.Equal(3, report.Totals.Reservations);
            Assert.Equal(1300, report.Totals.Sales);
            Assert.Equal(300, report.Totals.Collected);
            Assert.Equal(2, report.Totals.Travelers);
            Assert.Equal(1300, report.Months.Single(m => m.Month == DateTime.Today.Month).Sales);
            if (trip.StartDate.Year == DateTime.Today.Year)
            {
                Assert.Equal(20, report.Departures.Single().Occupancy);
            }
        }

        [Fact]
        public void An_agency_profile_needs_contacts_and_accepts_only_uploaded_logos()
        {
            var validator = new UpdateAgencyProfileValidator();
            var profile = new UpdateAgencyProfileDto
            {
                Address = "Street 1",
                Phone = "+381 11 555 0101",
                Email = "info@agency.example",
                Website = "https://agency.example",
                Logo = "uploads/logo.png"
            };

            Assert.True(validator.Validate(profile).IsValid);

            profile.Website = "javascript:alert(1)";
            profile.Logo = "https://elsewhere.example/logo.png";
            profile.Phone = "call me";
            Assert.Equal(3, validator.Validate(profile).Errors.Count);
        }
    }
}
