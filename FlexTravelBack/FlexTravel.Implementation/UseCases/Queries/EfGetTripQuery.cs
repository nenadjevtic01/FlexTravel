using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Queries
{
    public class EfGetTripQuery : EfUseCase, IGetTripQuery
    {
        private readonly BookingCalculator _calculator;

        public EfGetTripQuery(FlexTravelContext context, BookingCalculator calculator) : base(context)
        {
            _calculator = calculator;
        }

        public int UseCaseId => UseCaseIds.GetTrip;
        public string UseCaseName => "Get trip";
        public string UseCaseDescription => "Trip details with itinerary, hotels and availability";

        public TripDetailDto Execute(int id)
        {
            var trip = Context.Trip
                .Include(x => x.Agency)
                .Include(x => x.Transport)
                .Include(x => x.Prices)
                .Include(x => x.Itinerary).ThenInclude(i => i.ItineraryActivities)
                .Include(x => x.Destination).ThenInclude(d => d.Country)
                .Include(x => x.Destination).ThenInclude(d => d.Images)
                .Include(x => x.Accommodations).ThenInclude(a => a.Accommodation).ThenInclude(a => a.AccommodationType)
                .Include(x => x.Accommodations).ThenInclude(a => a.Accommodation).ThenInclude(a => a.Rooms).ThenInclude(r => r.RoomType)
                .Include(x => x.Accommodations).ThenInclude(a => a.Accommodation).ThenInclude(a => a.Rooms).ThenInclude(r => r.Prices)
                .Include(x => x.Attractions).ThenInclude(a => a.Attraction).ThenInclude(a => a.AttractionCategory)
                .Include(x => x.Extras)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefault(x => x.TripId == id && x.IsActive);

            if (trip == null)
            {
                throw new EntityNotFoundException("Trip", id);
            }

            var rooms = trip.Accommodations.SelectMany(a => a.Accommodation.Rooms).ToList();
            var booked = _calculator.BookedRooms(rooms.Select(r => r.RoomId), trip.StartDate, trip.EndDate);
            var (offer, offerPercent) = BookingPricing.OfferAt(trip, DateTime.Today);

            var hotelIds = trip.Accommodations.Select(a => a.AccommodationId).ToList();
            var hotelRatings = Context.Review
                .Where(r => r.IsActive && hotelIds.Contains(r.AccommodationId))
                .GroupBy(r => r.AccommodationId)
                .Select(g => new { Id = g.Key, Average = g.Average(r => (double)r.HotelRating), Count = g.Count() })
                .ToList();

            return new TripDetailDto
            {
                Id = trip.TripId,
                Name = trip.TripName,
                Description = trip.TripDescription,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Nights = BookingPricing.Nights(trip.StartDate, trip.EndDate),
                Capacity = trip.Capacity,
                SeatsLeft = Math.Max(0, trip.Capacity - _calculator.SeatsTaken(trip.TripId)),
                PricePerPerson = BookingPricing.TripPriceAt(trip.Prices, DateTime.Now),
                ChildDiscountPercent = trip.ChildDiscountPercent,
                Transport = trip.Transport.TransportName,
                FreeCancellationDays = BookingRules.FreeCancellationDays,
                FreeCancellationUntil = CancellationRules.FreeUntil(trip.StartDate),
                CancellationPolicy = CancellationRules.Policy(trip.StartDate),
                Offer = BookingPricing.OfferName(offer),
                OfferPercent = offerPercent,
                EarlyBookingPercent = trip.EarlyBookingPercent,
                EarlyBookingUntil = trip.EarlyBookingUntil,
                LastMinutePercent = trip.LastMinutePercent,
                LastMinuteDays = trip.LastMinuteDays,
                Rating = Ratings.Trip(Context.Review.Where(r => r.Trip.TripSeriesId == trip.TripSeriesId)),
                RatingDetails = Ratings.Breakdown(Context.Review.Where(r => r.Trip.TripSeriesId == trip.TripSeriesId)),
                Extras = trip.Extras
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.Kind)
                    .ThenBy(e => e.Name)
                    .Select(ExtraRules.ToDto)
                    .ToList(),
                Attractions = trip.Attractions
                    .Select(a => a.Attraction)
                    .OrderBy(a => a.AttractionName)
                    .Select(a => new AttractionDto
                    {
                        Id = a.AttractionId,
                        Name = a.AttractionName,
                        Description = a.AttractionDescription,
                        Category = a.AttractionCategory.AttractionCategoryName
                    })
                    .ToList(),
                Agency = new TripAgencyDto
                {
                    Id = trip.Agency.AgencyId,
                    Name = trip.Agency.AgencyName,
                    Address = trip.Agency.Address,
                    Phone = trip.Agency.Phone
                },
                Destination = new TripDestinationDto
                {
                    Id = trip.Destination.DestinationId,
                    Name = trip.Destination.DestinationName,
                    Country = trip.Destination.Country.CountryName,
                    Images = trip.Destination.Images
                        .OrderBy(i => i.DestinationImageId)
                        .Select(i => new ImageDto { Src = i.Src, Alt = i.Alt })
                        .ToList()
                },
                Itinerary = trip.Itinerary.ItineraryActivities
                    .OrderBy(a => a.Day)
                    .Select(ItineraryRules.ToDto)
                    .ToList(),
                Accommodations = trip.Accommodations
                    .Select(a => a.Accommodation)
                    .Where(a => a.Rooms.Any())
                    .OrderByDescending(a => a.AccommodationStars)
                    .Select(a => new TripAccommodationDto
                    {
                        Id = a.AccommodationId,
                        Name = a.AccommodationName,
                        Type = a.AccommodationType.AccommodationTypeName,
                        Stars = a.AccommodationStars,
                        Rating = Ratings.Create(
                            hotelRatings.FirstOrDefault(h => h.Id == a.AccommodationId)?.Average,
                            hotelRatings.FirstOrDefault(h => h.Id == a.AccommodationId)?.Count ?? 0),
                        Rooms = a.Rooms
                            .OrderBy(r => r.RoomType.Capacity)
                            .Select(r => new TripRoomDto
                            {
                                Id = r.RoomId,
                                RoomType = r.RoomType.RoomTypeName,
                                Capacity = r.RoomType.Capacity,
                                AC = r.AC,
                                Balcony = r.Balcony,
                                PricePerNight = BookingPricing.RoomPriceAt(r.Prices, trip.StartDate),
                                Available = Math.Max(0, r.NumberOfRooms - booked.GetValueOrDefault(r.RoomId))
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }
    }
}
