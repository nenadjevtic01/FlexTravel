using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Tests.Infrastructure;

namespace FlexTravel.Tests
{
    public class BookingCalculatorTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private PriceQuoteDto Quote(Trip trip, int adults, int children, int doubleRooms = 0, int familyRooms = 0, string promoCode = null)
        {
            var rooms = new List<BookingRoomDto>();
            if (doubleRooms > 0) rooms.Add(new BookingRoomDto { RoomId = World.DoubleRoom.RoomId, Count = doubleRooms });
            if (familyRooms > 0) rooms.Add(new BookingRoomDto { RoomId = World.FamilyRoom.RoomId, Count = familyRooms });

            return new BookingCalculator(_db.Context).Quote(new BookingRequestDto
            {
                TripId = trip.TripId,
                AccommodationId = World.Hotel.AccommodationId,
                Adults = adults,
                Children = children,
                Rooms = rooms,
                PromoCode = promoCode
            });
        }

        [Fact]
        public void Total_follows_the_formula()
        {
            var trip = World.Trip(price: 500, childDiscount: 50, nights: 5);

            var quote = Quote(trip, adults: 2, children: 1, familyRooms: 1);

            Assert.True(quote.IsValid, string.Join("; ", quote.Problems));
            Assert.Equal(1250, quote.TravelersTotal);
            Assert.Equal(750, quote.RoomsTotal);
            Assert.Equal(2000, quote.Subtotal);
            Assert.Equal(2000, quote.Total);
            Assert.Null(quote.Offer);
        }

        [Fact]
        public void Cancelled_and_rejected_reservations_release_their_seats()
        {
            var trip = World.Trip(capacity: 8);
            World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Pending);
            World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Confirmed);
            World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Cancelled);
            World.Reserve(trip, World.OtherCustomer, 2, status: ReservationStatus.Rejected);

            var quote = Quote(trip, adults: 5, children: 0, familyRooms: 1, doubleRooms: 1);

            Assert.Equal(4, quote.SeatsLeft);
            Assert.Contains("Only 4 seats are left on this trip.", quote.Problems);
        }

        [Fact]
        public void A_full_trip_is_sold_out()
        {
            var trip = World.Trip(capacity: 2);
            World.Reserve(trip, World.OtherCustomer, 2);

            var quote = Quote(trip, adults: 1, children: 0, doubleRooms: 1);

            Assert.Contains("This trip is sold out.", quote.Problems);
        }

        [Fact]
        public void Rooms_must_sleep_all_travelers()
        {
            var trip = World.Trip();

            var quote = Quote(trip, adults: 3, children: 0, doubleRooms: 1);

            Assert.Contains("The selected rooms sleep 2, but 3 people are traveling.", quote.Problems);
        }

        [Fact]
        public void Every_room_needs_an_adult()
        {
            var trip = World.Trip();

            var quote = Quote(trip, adults: 1, children: 1, doubleRooms: 2);

            Assert.Contains("Every room needs at least one adult.", quote.Problems);
        }

        [Fact]
        public void Rooms_booked_by_any_agency_for_overlapping_dates_are_not_available()
        {
            var otherAgencyTrip = World.Trip(startsInDays: 33, nights: 5, agency: World.OtherAgency);
            World.Reserve(otherAgencyTrip, World.OtherCustomer, 4, doubleRooms: 2);

            var overlapping = World.Trip(startsInDays: 30, nights: 5);
            var startsOnCheckOutDay = World.Trip(startsInDays: 38, nights: 3);

            Assert.Contains("Double Room is fully booked for these dates.", Quote(overlapping, 2, 0, doubleRooms: 1).Problems);
            Assert.True(Quote(startsOnCheckOutDay, 2, 0, doubleRooms: 1).IsValid);
        }

        [Fact]
        public void Early_booking_and_promo_code_are_both_percentages_of_the_subtotal()
        {
            var trip = World.Trip(price: 500, configure: t =>
            {
                t.EarlyBookingPercent = 10;
                t.EarlyBookingUntil = DateTime.Today.AddDays(10);
            });
            World.Promo("SAVE5", 5);

            var quote = Quote(trip, adults: 2, children: 0, doubleRooms: 1, promoCode: "save5");

            Assert.Equal(1500, quote.Subtotal);
            Assert.Equal("EarlyBooking", quote.Offer);
            Assert.Equal(150, quote.OfferDiscount);
            Assert.Equal("SAVE5", quote.PromoCode);
            Assert.Equal(75, quote.PromoDiscount);
            Assert.Equal(1275, quote.Total);
        }

        [Fact]
        public void Last_minute_discount_applies_close_to_departure()
        {
            var trip = World.Trip(startsInDays: 5, configure: t =>
            {
                t.LastMinutePercent = 20;
                t.LastMinuteDays = 7;
            });

            var quote = Quote(trip, adults: 2, children: 0, doubleRooms: 1);

            Assert.Equal("LastMinute", quote.Offer);
            Assert.Equal(quote.Subtotal * 0.8m, quote.Total);
        }

        [Fact]
        public void A_code_of_another_agency_is_explained_but_does_not_block_the_quote()
        {
            var trip = World.Trip();
            World.Promo("OTHER10", 10, agency: World.OtherAgency);

            var quote = Quote(trip, adults: 2, children: 0, doubleRooms: 1, promoCode: "OTHER10");

            Assert.True(quote.IsValid);
            Assert.Equal(0, quote.PromoDiscount);
            Assert.Equal("The code OTHER10 is not valid for this agency's trips.", quote.PromoCodeError);
        }

        [Fact]
        public void Expired_and_future_codes_do_not_apply()
        {
            var trip = World.Trip();
            World.Promo("OLD", 10, validFromDays: -30, validToDays: -1);
            World.Promo("SOON", 10, validFromDays: 2, validToDays: 30);

            Assert.Contains("expired", Quote(trip, 2, 0, doubleRooms: 1, promoCode: "OLD").PromoCodeError);
            Assert.Contains("can be used from", Quote(trip, 2, 0, doubleRooms: 1, promoCode: "SOON").PromoCodeError);
        }

        [Fact]
        public void A_used_up_code_becomes_available_again_when_a_reservation_using_it_is_cancelled()
        {
            var trip = World.Trip();
            var promo = World.Promo("ONCE", 10, maxUses: 1);
            var used = World.Reserve(trip, World.OtherCustomer, 2);
            used.PromoCode = promo;
            _db.Context.SaveChanges();

            Assert.Equal("The code ONCE has been used up.", Quote(trip, 2, 0, doubleRooms: 1, promoCode: "ONCE").PromoCodeError);

            used.Status = ReservationStatus.Cancelled;
            _db.Context.SaveChanges();

            Assert.Null(Quote(trip, 2, 0, doubleRooms: 1, promoCode: "ONCE").PromoCodeError);
        }
    }
}
