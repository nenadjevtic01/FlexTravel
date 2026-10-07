using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Tests
{
    public class BookingPricingTests
    {
        private static readonly DateTime Today = DateTime.Today;

        [Fact]
        public void Nights_are_counted_between_calendar_dates()
        {
            Assert.Equal(5, BookingPricing.Nights(new DateTime(2027, 6, 1, 23, 0, 0), new DateTime(2027, 6, 6, 6, 0, 0)));
            Assert.Equal(0, BookingPricing.Nights(new DateTime(2027, 6, 6), new DateTime(2027, 6, 1)));
        }

        [Theory]
        [InlineData(500, 50, 250)]
        [InlineData(333, 30, 233.10)]
        [InlineData(290, 0, 290)]
        [InlineData(290, 100, 0)]
        public void Child_price_applies_the_child_discount(decimal price, int discount, decimal expected)
        {
            Assert.Equal(expected, BookingPricing.ChildPrice(price, discount));
        }

        [Fact]
        public void Travelers_total_is_adults_at_full_price_plus_children_at_child_price()
        {
            Assert.Equal(1250, BookingPricing.TravelersTotal(500, 50, adults: 2, children: 1));
        }

        [Fact]
        public void Room_total_is_price_per_night_times_rooms_times_nights()
        {
            Assert.Equal(1500, BookingPricing.RoomTotal(150, rooms: 2, nights: 5));
        }

        [Fact]
        public void Price_at_a_moment_is_the_latest_price_already_valid()
        {
            var prices = new[]
            {
                (Today.AddDays(-10), 300m),
                (Today.AddDays(-2), 280m),
                (Today.AddDays(5), 260m)
            };

            Assert.Equal(280, BookingPricing.PriceAt(prices, Today));
            Assert.Equal(260, BookingPricing.PriceAt(prices, Today.AddDays(6)));
        }

        [Fact]
        public void Price_before_the_first_one_is_valid_falls_back_to_the_earliest()
        {
            var prices = new[] { (Today.AddDays(3), 300m), (Today.AddDays(9), 250m) };

            Assert.Equal(300, BookingPricing.PriceAt(prices, Today));
            Assert.Equal(0, BookingPricing.PriceAt(Array.Empty<(DateTime, decimal)>(), Today));
        }

        [Fact]
        public void Early_booking_applies_until_and_including_the_last_day()
        {
            var start = Today.AddDays(100);

            Assert.Equal((TripOffer.EarlyBooking, 15), BookingPricing.OfferAt(start, 15, Today, 0, 0, Today.AddHours(23)));
            Assert.Equal((TripOffer.None, 0), BookingPricing.OfferAt(start, 15, Today.AddDays(-1), 0, 0, Today));
        }

        [Theory]
        [InlineData(7, true)]
        [InlineData(1, true)]
        [InlineData(8, false)]
        [InlineData(0, false)]
        public void Last_minute_applies_within_the_last_days_before_departure(int daysBefore, bool applies)
        {
            var (offer, percent) = BookingPricing.OfferAt(Today.AddDays(daysBefore), 0, null, 20, 7, Today);

            Assert.Equal(applies ? TripOffer.LastMinute : TripOffer.None, offer);
            Assert.Equal(applies ? 20 : 0, percent);
        }

        [Fact]
        public void A_discount_of_zero_percent_is_no_offer()
        {
            Assert.Equal((TripOffer.None, 0), BookingPricing.OfferAt(Today.AddDays(3), 0, Today.AddDays(1), 0, 7, Today));
        }

        [Theory]
        [InlineData(1532, 10, 153.20)]
        [InlineData(0.05, 50, 0.03)]
        [InlineData(2690, 15, 403.50)]
        public void Discount_is_a_percentage_rounded_to_cents(decimal subtotal, int percent, decimal expected)
        {
            Assert.Equal(expected, BookingPricing.Discount(subtotal, percent));
        }

        [Fact]
        public void Promo_codes_are_compared_trimmed_and_in_upper_case()
        {
            Assert.Equal("HORIZON10", BookingPricing.NormalizePromoCode("  horizon10 "));
            Assert.Null(BookingPricing.NormalizePromoCode("   "));
            Assert.Null(BookingPricing.NormalizePromoCode(null));
        }

        [Theory]
        [InlineData(ReservationStatus.Pending, true)]
        [InlineData(ReservationStatus.Confirmed, true)]
        [InlineData(ReservationStatus.Rejected, false)]
        [InlineData(ReservationStatus.Cancelled, false)]
        public void Only_pending_and_confirmed_reservations_hold_seats(ReservationStatus status, bool holds)
        {
            Assert.Equal(holds, BookingPricing.HoldsCapacity(status));
        }
    }
}
