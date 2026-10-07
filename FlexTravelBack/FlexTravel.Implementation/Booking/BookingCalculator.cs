using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FlexTravel.Implementation.Booking
{
    public class BookingCalculator
    {
        private readonly FlexTravelContext _context;

        public BookingCalculator(FlexTravelContext context)
        {
            _context = context;
        }

        public class Calculation
        {
            public PriceQuoteDto Quote { get; set; }
            public TripOffer Offer { get; set; }
            public int? PromoCodeId { get; set; }
        }

        public IQueryable<Reservation> ActiveReservations() =>
            _context.Reservation.Where(r => r.IsActive
                && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed));

        public int SeatsTaken(int tripId) =>
            ActiveReservations()
                .Where(r => r.TripId == tripId)
                .Sum(r => (int?)(r.NumberOfAdults + r.NumberOfChildren)) ?? 0;

        public int PromoCodeUses(int promoCodeId) =>
            ActiveReservations().Count(r => r.PromoCodeId == promoCodeId);

        public Dictionary<int, int> BookedRooms(IEnumerable<int> roomIds, DateTime start, DateTime end)
        {
            var ids = roomIds.ToList();

            return _context.ReservationRoom
                .Where(rr => ids.Contains(rr.RoomId)
                    && rr.Reservation.IsActive
                    && (rr.Reservation.Status == ReservationStatus.Pending || rr.Reservation.Status == ReservationStatus.Confirmed)
                    && rr.Reservation.Trip.StartDate < end
                    && start < rr.Reservation.Trip.EndDate)
                .GroupBy(rr => rr.RoomId)
                .Select(g => new { RoomId = g.Key, Booked = g.Sum(x => x.NumberOfRooms) })
                .ToDictionary(x => x.RoomId, x => x.Booked);
        }

        public (PromoCode PromoCode, string Error) FindPromoCode(string code, int agencyId, DateTime today)
        {
            var normalized = BookingPricing.NormalizePromoCode(code);
            if (normalized == null)
            {
                return (null, null);
            }

            var promo = _context.PromoCode.AsNoTracking().FirstOrDefault(p => p.Code == normalized);

            if (promo == null || promo.AgencyId != agencyId)
            {
                return (null, $"The code {normalized} is not valid for this agency's trips.");
            }

            if (!promo.IsActive)
            {
                return (null, $"The code {normalized} is no longer active.");
            }

            if (today.Date < promo.ValidFrom.Date)
            {
                return (null, $"The code {normalized} can be used from {ShortDate(promo.ValidFrom)}.");
            }

            if (today.Date > promo.ValidTo.Date)
            {
                return (null, $"The code {normalized} expired on {ShortDate(promo.ValidTo)}.");
            }

            if (promo.MaxUses.HasValue && PromoCodeUses(promo.PromoCodeId) >= promo.MaxUses.Value)
            {
                return (null, $"The code {normalized} has been used up.");
            }

            return (promo, null);
        }

        public PriceQuoteDto Quote(BookingRequestDto request) => Calculate(request).Quote;

        public Calculation Calculate(BookingRequestDto request)
        {
            var trip = _context.Trip
                .Include(x => x.Prices)
                .Include(x => x.Accommodations)
                .Include(x => x.Extras)
                .FirstOrDefault(x => x.TripId == request.TripId && x.IsActive);

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", request.TripId);
            }

            var quote = new PriceQuoteDto
            {
                Adults = request.Adults,
                Children = request.Children,
                Nights = BookingPricing.Nights(trip.StartDate, trip.EndDate),
                PricePerPerson = BookingPricing.TripPriceAt(trip.Prices, DateTime.Now),
                ChildDiscountPercent = trip.ChildDiscountPercent
            };
            quote.ChildPrice = BookingPricing.ChildPrice(quote.PricePerPerson, quote.ChildDiscountPercent);
            quote.TravelersTotal = BookingPricing.TravelersTotal(quote.PricePerPerson, quote.ChildDiscountPercent, request.Adults, request.Children);
            quote.SeatsLeft = trip.Capacity - SeatsTaken(trip.TripId);

            var travelers = request.Adults + request.Children;

            if (trip.StartDate.Date <= DateTime.Today)
            {
                quote.Problems.Add("This trip has already departed.");
            }

            if (request.Adults < 1)
            {
                quote.Problems.Add("At least one adult must travel.");
            }

            if (travelers > BookingRules.MaxTravelersPerReservation)
            {
                quote.Problems.Add($"One reservation can include at most {BookingRules.MaxTravelersPerReservation} travelers.");
            }

            if (travelers > quote.SeatsLeft)
            {
                quote.Problems.Add(quote.SeatsLeft > 0
                    ? $"Only {quote.SeatsLeft} seats are left on this trip."
                    : "This trip is sold out.");
            }

            AddExtras(quote, trip, request.Extras, travelers);

            if (!trip.Accommodations.Any(a => a.AccommodationId == request.AccommodationId))
            {
                quote.Problems.Add("Choose one of the hotels offered for this trip.");
                quote.Subtotal = quote.TravelersTotal;
                return ApplyDiscounts(quote, trip, request.PromoCode);
            }

            var requested = (request.Rooms ?? new List<BookingRoomDto>())
                .Where(r => r.Count > 0)
                .GroupBy(r => r.RoomId)
                .Select(g => new { RoomId = g.Key, Count = g.Sum(x => x.Count) })
                .ToList();

            var rooms = _context.Room
                .Include(r => r.RoomType)
                .Include(r => r.Prices)
                .Where(r => r.AccommodationId == request.AccommodationId)
                .ToList();

            var booked = BookedRooms(rooms.Select(r => r.RoomId), trip.StartDate, trip.EndDate);
            var quoteRooms = new List<PriceQuoteRoomDto>();

            foreach (var item in requested)
            {
                var room = rooms.FirstOrDefault(r => r.RoomId == item.RoomId);
                if (room == null)
                {
                    quote.Problems.Add("A selected room does not belong to the chosen hotel.");
                    continue;
                }

                var available = room.NumberOfRooms - booked.GetValueOrDefault(room.RoomId);
                if (item.Count > available)
                {
                    quote.Problems.Add(available > 0
                        ? $"Only {available} × {room.RoomType.RoomTypeName} available for these dates."
                        : $"{room.RoomType.RoomTypeName} is fully booked for these dates.");
                }

                var pricePerNight = BookingPricing.RoomPriceAt(room.Prices, trip.StartDate);
                quoteRooms.Add(new PriceQuoteRoomDto
                {
                    RoomId = room.RoomId,
                    RoomType = room.RoomType.RoomTypeName,
                    Count = item.Count,
                    Capacity = room.RoomType.Capacity,
                    PricePerNight = pricePerNight,
                    Total = BookingPricing.RoomTotal(pricePerNight, item.Count, quote.Nights)
                });
            }

            quote.Rooms = quoteRooms;
            quote.Beds = quoteRooms.Sum(r => r.Capacity * r.Count);
            quote.RoomsTotal = quoteRooms.Sum(r => r.Total);
            quote.Subtotal = quote.TravelersTotal + quote.RoomsTotal;

            var roomCount = quoteRooms.Sum(r => r.Count);
            if (roomCount == 0)
            {
                quote.Problems.Add("Choose at least one room.");
            }
            else if (quote.Beds < travelers)
            {
                quote.Problems.Add($"The selected rooms sleep {quote.Beds}, but {travelers} people are traveling.");
            }
            else if (roomCount > Math.Max(request.Adults, 1))
            {
                quote.Problems.Add("Every room needs at least one adult.");
            }

            return ApplyDiscounts(quote, trip, request.PromoCode);
        }

        private Calculation ApplyDiscounts(PriceQuoteDto quote, Trip trip, string promoCode)
        {
            var today = DateTime.Today;
            var (offer, offerPercent) = BookingPricing.OfferAt(trip, today);

            quote.Offer = BookingPricing.OfferName(offer);
            quote.OfferPercent = offerPercent;
            quote.OfferDiscount = BookingPricing.Discount(quote.Subtotal, offerPercent);

            var (promo, error) = FindPromoCode(promoCode, trip.AgencyId, today);
            quote.PromoCode = promo?.Code;
            quote.PromoPercent = promo?.DiscountPercent ?? 0;
            quote.PromoDiscount = BookingPricing.Discount(quote.Subtotal, quote.PromoPercent);
            quote.PromoCodeError = error;

            quote.Total = quote.Subtotal - quote.OfferDiscount - quote.PromoDiscount + quote.ExtrasTotal;

            return new Calculation { Quote = quote, Offer = offer, PromoCodeId = promo?.PromoCodeId };
        }

        private static void AddExtras(PriceQuoteDto quote, Trip trip, IEnumerable<BookingExtraDto> requested, int travelers)
        {
            var extras = new List<PriceQuoteExtraDto>();

            foreach (var item in (requested ?? Enumerable.Empty<BookingExtraDto>()).GroupBy(e => e.ExtraId).Select(g => g.First()))
            {
                var extra = trip.Extras.FirstOrDefault(e => e.TripExtraId == item.ExtraId && e.IsActive);
                if (extra == null)
                {
                    quote.Problems.Add("One of the selected extras is no longer offered for this trip.");
                    continue;
                }

                var quantity = extra.PerPerson ? item.Quantity : 1;
                if (extra.PerPerson && (quantity < 1 || quantity > Math.Max(travelers, 1)))
                {
                    quote.Problems.Add($"{extra.Name}: choose for how many travelers (1 to {Math.Max(travelers, 1)}).");
                    continue;
                }

                extras.Add(new PriceQuoteExtraDto
                {
                    ExtraId = extra.TripExtraId,
                    Kind = extra.Kind.ToString(),
                    Name = extra.Name,
                    UnitPrice = extra.Price,
                    Quantity = quantity,
                    Total = ExtraRules.Total(extra.Price, quantity)
                });
            }

            quote.Extras = extras;
            quote.ExtrasTotal = extras.Sum(e => e.Total);
        }

        private static string ShortDate(DateTime date) => date.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
    }
}
