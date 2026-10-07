using FlexTravel.Domain;

namespace FlexTravel.Implementation.Booking
{
    public static class BookingPricing
    {
        public static int Nights(DateTime start, DateTime end) => Math.Max(0, (end.Date - start.Date).Days);

        public static decimal ChildPrice(decimal pricePerPerson, int childDiscountPercent) =>
            Math.Round(pricePerPerson * (100 - childDiscountPercent) / 100m, 2);

        public static decimal TravelersTotal(decimal pricePerPerson, int childDiscountPercent, int adults, int children) =>
            adults * pricePerPerson + children * ChildPrice(pricePerPerson, childDiscountPercent);

        public static decimal RoomTotal(decimal pricePerNight, int rooms, int nights) => pricePerNight * rooms * nights;

        public static decimal PriceAt(IEnumerable<(DateTime ValidFrom, decimal Price)> prices, DateTime at)
        {
            var list = prices.OrderBy(x => x.ValidFrom).ToList();
            if (list.Count == 0)
            {
                return 0;
            }

            var valid = list.Where(x => x.ValidFrom <= at).ToList();
            return valid.Count > 0 ? valid[^1].Price : list[0].Price;
        }

        public static decimal TripPriceAt(IEnumerable<TripPrice> prices, DateTime at) =>
            PriceAt(prices.Select(x => (x.ValidFrom, x.Price)), at);

        public static decimal RoomPriceAt(IEnumerable<RoomPrice> prices, DateTime at) =>
            PriceAt(prices.Select(x => (x.ValidFrom, x.Price)), at);

        public static bool HoldsCapacity(ReservationStatus status) =>
            status == ReservationStatus.Pending || status == ReservationStatus.Confirmed;

        public static (TripOffer Offer, int Percent) OfferAt(
            DateTime startDate, int earlyBookingPercent, DateTime? earlyBookingUntil,
            int lastMinutePercent, int lastMinuteDays, DateTime bookedOn)
        {
            var day = bookedOn.Date;

            if (earlyBookingPercent > 0 && earlyBookingUntil.HasValue && day <= earlyBookingUntil.Value.Date)
            {
                return (TripOffer.EarlyBooking, earlyBookingPercent);
            }

            var daysBefore = (startDate.Date - day).Days;
            if (lastMinutePercent > 0 && lastMinuteDays > 0 && daysBefore > 0 && daysBefore <= lastMinuteDays)
            {
                return (TripOffer.LastMinute, lastMinutePercent);
            }

            return (TripOffer.None, 0);
        }

        public static (TripOffer Offer, int Percent) OfferAt(Trip trip, DateTime bookedOn) =>
            OfferAt(trip.StartDate, trip.EarlyBookingPercent, trip.EarlyBookingUntil,
                trip.LastMinutePercent, trip.LastMinuteDays, bookedOn);

        public static decimal Discount(decimal subtotal, int percent) =>
            Math.Round(subtotal * percent / 100m, 2, MidpointRounding.AwayFromZero);

        public static string OfferName(TripOffer offer) => offer == TripOffer.None ? null : offer.ToString();

        public static string Euro(decimal amount) =>
            "€" + amount.ToString(amount % 1 == 0 ? "#,0" : "#,0.00", System.Globalization.CultureInfo.InvariantCulture);

        public static string NormalizePromoCode(string code) =>
            string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
    }
}
