using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.UseCases.Queries;
using FlexTravel.DataAccess;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.UseCases.Commands.Catalog;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Queries.Catalog
{
    public class EfGetCatalogOptionsQuery : EfUseCase, IGetCatalogOptionsQuery
    {
        public EfGetCatalogOptionsQuery(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.GetCatalogOptions;
        public string UseCaseName => "Get catalog options";
        public string UseCaseDescription => "Continents, hotel types, room types and sight categories";

        public CatalogOptionsDto Execute(int request) => new CatalogOptionsDto
        {
            Continents = Context.Continent.OrderBy(c => c.ContinentName)
                .Select(c => new LookupDto { Id = c.ContinentId, Name = c.ContinentName }).ToList(),
            AccommodationTypes = Context.AccommodationsType.OrderBy(t => t.AccommodationTypeName)
                .Select(t => new LookupDto { Id = t.AccommodationTypeId, Name = t.AccommodationTypeName }).ToList(),
            RoomTypes = Context.RoomType.OrderBy(t => t.Capacity).ThenBy(t => t.RoomTypeName)
                .Select(t => new RoomTypeLookupDto { Id = t.RoomTypeId, Name = t.RoomTypeName, Capacity = t.Capacity }).ToList(),
            AttractionCategories = Context.AttractionCategory.OrderBy(c => c.AttractionCategoryName)
                .Select(c => new LookupDto { Id = c.AttractionCategoryId, Name = c.AttractionCategoryName }).ToList()
        };
    }

    public class EfGetDestinationForEditQuery : EfUseCase, IGetDestinationForEditQuery
    {
        private readonly BookingCalculator _calculator;

        public EfGetDestinationForEditQuery(FlexTravelContext context, BookingCalculator calculator) : base(context)
        {
            _calculator = calculator;
        }

        public int UseCaseId => UseCaseIds.GetDestinationForEdit;
        public string UseCaseName => "Get destination for edit";
        public string UseCaseDescription => "Destination with hotels, rooms, prices and sights for the catalog editor";

        public DestinationForEditDto Execute(int id)
        {
            var today = DateTime.Today;
            var destination = Context.Destination
                .Where(d => d.DestinationId == id)
                .Select(d => new
                {
                    d.DestinationId,
                    d.CountryId,
                    Country = d.Country.CountryName,
                    d.DestinationName,
                    d.DestinationDescription,
                    Climate = d.Climate == null ? null : d.Climate.ClimateInfo,
                    d.Latitude,
                    d.Longitude,
                    d.IsActive,
                    UpcomingTrips = d.Trips.Count(t => t.IsActive && t.StartDate > today),
                    Images = d.Images.OrderBy(i => i.DestinationImageId).Select(i => new ImageDto { Src = i.Src, Alt = i.Alt }).ToList(),
                    Sights = d.Attractions.OrderBy(a => a.AttractionName).Select(a => new SightForEditDto
                    {
                        Id = a.AttractionId,
                        Name = a.AttractionName,
                        Description = a.AttractionDescription,
                        CategoryId = a.AttractionCategoryId,
                        Trips = a.Trips.Count
                    }).ToList()
                })
                .AsSplitQuery()
                .FirstOrDefault();

            if (destination == null)
            {
                throw new EntityNotFoundException("Destination", id);
            }

            var hotels = Context.Accommodation
                .Where(a => a.DestinationId == id)
                .Include(a => a.Rooms).ThenInclude(r => r.Prices)
                .Include(a => a.Rooms).ThenInclude(r => r.RoomType)
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(a => a.AccommodationStars).ThenBy(a => a.AccommodationName)
                .ToList();

            var hotelIds = hotels.Select(h => h.AccommodationId).ToList();
            var tripsPerHotel = Context.TripAccommodation
                .Where(t => hotelIds.Contains(t.AccommodationId))
                .GroupBy(t => t.AccommodationId)
                .Select(g => new { g.Key, Count = g.Count(), OnSale = g.Count(t => t.Trip.IsActive && t.Trip.StartDate > today) })
                .ToDictionary(x => x.Key);
            var roomIds = hotels.SelectMany(h => h.Rooms).Select(r => r.RoomId).ToList();
            var withReservations = Context.ReservationRoom.Where(rr => roomIds.Contains(rr.RoomId)).Select(rr => rr.RoomId).Distinct().ToHashSet();

            return new DestinationForEditDto
            {
                Id = destination.DestinationId,
                CountryId = destination.CountryId,
                Country = destination.Country,
                Name = destination.DestinationName,
                Description = destination.DestinationDescription,
                Climate = destination.Climate,
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                IsActive = destination.IsActive,
                UpcomingTrips = destination.UpcomingTrips,
                Images = destination.Images,
                Sights = destination.Sights,
                Hotels = hotels.Select(h =>
                {
                    var maxBooked = CatalogRules.MaxBookedRooms(Context, _calculator, h.AccommodationId);
                    return new HotelForEditDto
                    {
                        Id = h.AccommodationId,
                        Name = h.AccommodationName,
                        TypeId = h.AccommodationTypeId,
                        Stars = h.AccommodationStars,
                        Trips = tripsPerHotel.GetValueOrDefault(h.AccommodationId)?.Count ?? 0,
                        UpcomingTrips = tripsPerHotel.GetValueOrDefault(h.AccommodationId)?.OnSale ?? 0,
                        IsActive = h.IsActive,
                        Rooms = h.Rooms
                            .OrderBy(r => r.RoomType.Capacity)
                            .Select(r => new RoomForEditDto
                            {
                                Id = r.RoomId,
                                RoomTypeId = r.RoomTypeId,
                                NumberOfRooms = r.NumberOfRooms,
                                AC = r.AC,
                                Balcony = r.Balcony,
                                PricePerNight = r.Prices.OrderByDescending(p => p.ValidFrom).Select(p => p.Price).FirstOrDefault(),
                                MaxBooked = maxBooked.GetValueOrDefault(r.RoomId),
                                HasReservations = withReservations.Contains(r.RoomId)
                            })
                            .ToList()
                    };
                }).ToList()
            };
        }
    }
}
