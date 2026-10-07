using FlexTravel.Application.Dto;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.UseCases.Commands.Catalog;
using FlexTravel.Implementation.Validators;
using FlexTravel.Tests.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Tests
{
    public class CatalogRulesTests : IDisposable
    {
        private readonly TestDatabase _db = new TestDatabase();
        private World World => _db.World;

        public void Dispose() => _db.Dispose();

        private void SaveHotel(params SaveRoomDto[] rooms) =>
            new EfSaveHotelCommand(_db.Context, new SaveHotelValidator(_db.Context), new BookingCalculator(_db.Context))
                .Execute(new SaveHotelDto
                {
                    Id = World.Hotel.AccommodationId,
                    Name = World.Hotel.AccommodationName,
                    TypeId = World.Hotel.AccommodationTypeId,
                    Stars = 4,
                    Rooms = rooms.ToList()
                });

        private SaveRoomDto Double(int rooms, decimal price = 100) => new SaveRoomDto
        {
            Id = World.DoubleRoom.RoomId,
            RoomTypeId = World.DoubleRoom.RoomTypeId,
            NumberOfRooms = rooms,
            PricePerNight = price
        };

        private SaveRoomDto Family(int rooms = 1, decimal price = 150) => new SaveRoomDto
        {
            Id = World.FamilyRoom.RoomId,
            RoomTypeId = World.FamilyRoom.RoomTypeId,
            NumberOfRooms = rooms,
            PricePerNight = price
        };

        [Fact]
        public void Rooms_cannot_drop_below_what_is_booked_for_an_upcoming_trip()
        {
            var trip = World.Trip();
            World.Reserve(trip, World.Customer, 4, doubleRooms: 2);

            var error = Assert.Throws<ConflictException>(() => SaveHotel(Double(1), Family()));
            Assert.Contains("at least 2", error.Message);

            SaveHotel(Double(2), Family());
        }

        [Fact]
        public void Bookings_of_finished_trips_do_not_limit_the_rooms()
        {
            var past = World.Trip(startsInDays: -20, nights: 5);
            World.Reserve(past, World.Customer, 4, doubleRooms: 2);

            SaveHotel(Double(1), Family());

            Assert.Equal(1, _db.NewContext().Room.Single(r => r.RoomId == World.DoubleRoom.RoomId).NumberOfRooms);
        }

        [Fact]
        public void A_room_category_with_reservations_cannot_be_removed()
        {
            var trip = World.Trip(startsInDays: -20);
            World.Reserve(trip, World.Customer, 2);

            Assert.Throws<ConflictException>(() => SaveHotel(Family()));

            SaveHotel(Double(2));
            Assert.False(_db.NewContext().Room.Any(r => r.RoomId == World.FamilyRoom.RoomId));
        }

        [Fact]
        public void A_new_price_is_added_to_the_history_and_reservations_keep_theirs()
        {
            var trip = World.Trip();
            var reservation = World.Reserve(trip, World.Customer, 2);

            SaveHotel(Double(2, price: 120), Family());

            var prices = _db.NewContext().RoomPrice.Where(p => p.RoomId == World.DoubleRoom.RoomId).OrderBy(p => p.ValidFrom).ToList();
            Assert.Equal(new[] { 100m, 120m }, prices.Select(p => p.Price));
            Assert.Equal(DateTime.Today, prices.Last().ValidFrom);
            Assert.Equal(100, _db.NewContext().ReservationRoom.Single(r => r.ReservationId == reservation.ReservationId).PricePerNight);

            var quote = new BookingCalculator(_db.NewContext()).Quote(new BookingRequestDto
            {
                TripId = trip.TripId,
                AccommodationId = World.Hotel.AccommodationId,
                Adults = 2,
                Rooms = new List<BookingRoomDto> { new BookingRoomDto { RoomId = World.DoubleRoom.RoomId, Count = 1 } }
            });
            Assert.Equal(120, quote.Rooms.Single().PricePerNight);
        }

        [Fact]
        public void Each_room_type_is_listed_once_and_needs_a_price()
        {
            Assert.Throws<ValidationException>(() => SaveHotel(Double(2), new SaveRoomDto { RoomTypeId = World.DoubleRoom.RoomTypeId, NumberOfRooms = 1, PricePerNight = 90 }));
            Assert.Throws<ValidationException>(() => SaveHotel(Double(2, price: 0)));
        }

        [Fact]
        public void A_sight_on_a_trip_program_cannot_be_deleted()
        {
            var category = new AttractionCategory { AttractionCategoryName = "Landmark" };
            var sight = new Attraction { AttractionName = "Old bridge", AttractionDescription = "A bridge.", AttractionCategory = category, Destination = World.Destination };
            World.Trip(configure: t => t.Attractions.Add(new TripAttraction { Attraction = sight }));
            var unused = new Attraction { AttractionName = "Museum", AttractionDescription = "A museum.", AttractionCategory = category, Destination = World.Destination };
            _db.Context.Attraction.Add(unused);
            _db.Context.SaveChanges();

            Assert.Throws<ConflictException>(() => new EfDeleteSightCommand(_db.Context).Execute(sight.AttractionId));
            new EfDeleteSightCommand(_db.Context).Execute(unused.AttractionId);

            Assert.Equal(1, _db.NewContext().Attraction.Count());
        }

        [Fact]
        public void Destination_names_are_unique_within_a_country_and_image_paths_stay_inside_the_site()
        {
            var validator = new SaveDestinationValidator(_db.Context);
            var dto = new SaveDestinationDto { CountryId = World.Destination.CountryId, Name = "Testville", Description = "Again." };

            Assert.False(validator.Validate(dto).IsValid);

            dto.Name = "Newtown";
            Assert.True(validator.Validate(dto).IsValid);

            dto.Images.Add(new ImageDto { Src = "../../appsettings.json", Alt = "x" });
            Assert.False(validator.Validate(dto).IsValid);
        }

        [Fact]
        public void A_new_destination_is_created_with_its_hotels_and_sights_or_not_at_all()
        {
            var category = new AttractionCategory { AttractionCategoryName = "Beach" };
            _db.Context.AttractionCategory.Add(category);
            _db.Context.SaveChanges();

            SaveDestinationDto Request(decimal price) => new SaveDestinationDto
            {
                CountryId = World.Destination.CountryId,
                Name = "Seaside",
                Description = "A town by the sea.",
                Hotels = new List<SaveHotelDto>
                {
                    new SaveHotelDto
                    {
                        Name = "Sea Hotel", TypeId = World.Hotel.AccommodationTypeId, Stars = 3,
                        Rooms = new List<SaveRoomDto> { new SaveRoomDto { RoomTypeId = World.DoubleRoom.RoomTypeId, NumberOfRooms = 5, PricePerNight = price } }
                    }
                },
                Sights = new List<SaveSightDto> { new SaveSightDto { Name = "Long beach", Description = "Sand.", CategoryId = category.AttractionCategoryId } }
            };
            var command = new EfSaveDestinationCommand(_db.Context, new SaveDestinationValidator(_db.Context));

            Assert.Throws<ValidationException>(() => command.Execute(Request(price: 0)));
            Assert.False(_db.NewContext().Destination.Any(d => d.DestinationName == "Seaside"));

            var id = command.Execute(Request(price: 80));

            var saved = _db.NewContext().Destination
                .Include(d => d.Accommodations).ThenInclude(a => a.Rooms).ThenInclude(r => r.Prices)
                .Include(d => d.Attractions)
                .Single(d => d.DestinationId == id);
            Assert.Equal("Sea Hotel", saved.Accommodations.Single().AccommodationName);
            Assert.Equal(80, saved.Accommodations.Single().Rooms.Single().Prices.Single().Price);
            Assert.Equal("Long beach", saved.Attractions.Single().AttractionName);
        }

        [Theory]
        [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0, 0, 0, 0, 0, 0, 0, 0 }, ".jpg")]
        [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0 }, ".png")]
        [InlineData(new byte[] { (byte)'R', (byte)'I', (byte)'F', (byte)'F', 0, 0, 0, 0, (byte)'W', (byte)'E', (byte)'B', (byte)'P' }, ".webp")]
        [InlineData(new byte[] { (byte)'<', (byte)'s', (byte)'v', (byte)'g', 0, 0, 0, 0, 0, 0, 0, 0 }, null)]
        public void Uploads_are_recognised_by_their_content_not_their_name(byte[] content, string extension)
        {
            var image = new UploadImageDto { FileName = "photo.jpg", ContentType = "image/jpeg", Content = content };

            Assert.Equal(extension, UploadImageValidator.Extension(image));
            Assert.Equal(extension != null, new UploadImageValidator().Validate(image).IsValid);
        }
    }
}
