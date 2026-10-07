using FlexTravel.Application.Constants;
using FlexTravel.Application.Dto;
using FlexTravel.Application.Exceptions;
using FlexTravel.Application.Files;
using FlexTravel.Application.UseCases.Commands;
using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;
using FlexTravel.Implementation.Exceptions;
using FlexTravel.Implementation.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.Implementation.UseCases.Commands.Catalog
{
    public class EfSaveCountryCommand : EfUseCase, ISaveCountryCommand
    {
        private readonly SaveCountryValidator _validator;

        public EfSaveCountryCommand(FlexTravelContext context, SaveCountryValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.SaveCountry;
        public string UseCaseName => "Save country";
        public string UseCaseDescription => "Add or edit a country with visa and safety info";

        public int Execute(SaveCountryDto request)
        {
            _validator.ValidateAndThrow(request);

            var country = request.Id == 0
                ? new Country()
                : Context.Country.Include(c => c.VisaInfo).Include(c => c.Safety).FirstOrDefault(c => c.CountryId == request.Id)
                    ?? throw new EntityNotFoundException("Country", request.Id);

            country.CountryName = request.Name.Trim();
            country.ContinentId = request.ContinentId;
            country.CapitalCity = Clean(request.CapitalCity);
            country.Currency = Clean(request.Currency);
            country.Language = Clean(request.Language);
            country.Population = request.Population;
            country.TotalArea = request.TotalArea;

            if (string.IsNullOrWhiteSpace(request.VisaType))
            {
                if (country.VisaInfo != null)
                {
                    Context.Remove(country.VisaInfo);
                }
            }
            else
            {
                country.VisaInfo ??= new Visa();
                country.VisaInfo.VisaType = request.VisaType.Trim();
                country.VisaInfo.Duration = request.VisaDuration.Trim();
                country.VisaInfo.Requirements = request.VisaRequirements.Trim();
            }

            if (request.SafetyLevel == null)
            {
                if (country.Safety != null)
                {
                    Context.Remove(country.Safety);
                }
            }
            else
            {
                country.Safety ??= new Safety();
                country.Safety.SafetyLevel = request.SafetyLevel.Value;
                country.Safety.Advices = request.SafetyAdvices.Trim();
            }

            if (request.Id == 0)
            {
                Context.Country.Add(country);
            }

            Context.SaveChanges();
            return country.CountryId;
        }

        internal static string Clean(string text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    public class EfSaveDestinationCommand : EfUseCase, ISaveDestinationCommand
    {
        private readonly SaveDestinationValidator _validator;

        public EfSaveDestinationCommand(FlexTravelContext context, SaveDestinationValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.SaveDestination;
        public string UseCaseName => "Save destination";
        public string UseCaseDescription => "Add or edit a destination with climate and images";

        public int Execute(SaveDestinationDto request)
        {
            request.Images ??= new List<ImageDto>();
            request.Hotels ??= new List<SaveHotelDto>();
            request.Sights ??= new List<SaveSightDto>();
            _validator.ValidateAndThrow(request);

            var destination = request.Id == 0
                ? new Destination()
                : Context.Destination.Include(d => d.Climate).Include(d => d.Images).FirstOrDefault(d => d.DestinationId == request.Id)
                    ?? throw new EntityNotFoundException("Destination", request.Id);

            destination.CountryId = request.CountryId;
            destination.DestinationName = request.Name.Trim();
            destination.DestinationDescription = request.Description.Trim();
            destination.Latitude = request.Latitude;
            destination.Longitude = request.Longitude;

            var climate = EfSaveCountryCommand.Clean(request.Climate);
            if (climate == null)
            {
                if (destination.Climate != null)
                {
                    Context.Remove(destination.Climate);
                }
            }
            else
            {
                destination.Climate ??= new Climate();
                destination.Climate.ClimateInfo = climate;
            }

            Context.RemoveRange(destination.Images);
            destination.Images = request.Images
                .Select(i => new DestinationImage
                {
                    Src = i.Src.Trim(),
                    Alt = EfSaveCountryCommand.Clean(i.Alt) ?? destination.DestinationName
                })
                .ToList();

            if (request.Id == 0)
            {
                foreach (var h in request.Hotels)
                {
                    var hotel = new Accommodation
                    {
                        AccommodationName = h.Name.Trim(),
                        AccommodationTypeId = h.TypeId,
                        AccommodationStars = h.Stars
                    };

                    foreach (var r in h.Rooms)
                    {
                        var room = new Room { RoomTypeId = r.RoomTypeId, NumberOfRooms = r.NumberOfRooms, AC = r.AC, Balcony = r.Balcony };
                        room.Prices.Add(new RoomPrice { Price = r.PricePerNight, ValidFrom = DateTime.Today });
                        hotel.Rooms.Add(room);
                    }

                    destination.Accommodations.Add(hotel);
                }

                foreach (var s in request.Sights)
                {
                    destination.Attractions.Add(new Attraction
                    {
                        AttractionName = s.Name.Trim(),
                        AttractionDescription = s.Description.Trim(),
                        AttractionCategoryId = s.CategoryId
                    });
                }

                Context.Destination.Add(destination);
            }

            Context.SaveChanges();
            return destination.DestinationId;
        }
    }

    public class EfSaveHotelCommand : EfUseCase, ISaveHotelCommand
    {
        private readonly SaveHotelValidator _validator;
        private readonly BookingCalculator _calculator;

        public EfSaveHotelCommand(FlexTravelContext context, SaveHotelValidator validator, BookingCalculator calculator)
            : base(context)
        {
            _validator = validator;
            _calculator = calculator;
        }

        public int UseCaseId => UseCaseIds.SaveHotel;
        public string UseCaseName => "Save hotel";
        public string UseCaseDescription => "Add or edit a hotel with room categories and prices";

        public int Execute(SaveHotelDto request)
        {
            Accommodation hotel;
            if (request.Id == 0)
            {
                hotel = new Accommodation { DestinationId = request.DestinationId };
            }
            else
            {
                hotel = Context.Accommodation
                    .Include(a => a.Rooms).ThenInclude(r => r.Prices)
                    .Include(a => a.Rooms).ThenInclude(r => r.RoomType)
                    .FirstOrDefault(a => a.AccommodationId == request.Id)
                    ?? throw new EntityNotFoundException("Hotel", request.Id);

                request.DestinationId = hotel.DestinationId;
            }

            request.Rooms ??= new List<SaveRoomDto>();
            _validator.ValidateAndThrow(request);

            hotel.AccommodationName = request.Name.Trim();
            hotel.AccommodationTypeId = request.TypeId;
            hotel.AccommodationStars = request.Stars;

            var maxBooked = request.Id == 0 ? new Dictionary<int, int>() : CatalogRules.MaxBookedRooms(Context, _calculator, hotel.AccommodationId);
            var withReservations = hotel.Rooms.Select(r => r.RoomId).Where(id => Context.ReservationRoom.Any(rr => rr.RoomId == id)).ToHashSet();
            var roomTypes = Context.RoomType.ToDictionary(t => t.RoomTypeId, t => t.RoomTypeName);

            foreach (var removed in hotel.Rooms.Where(r => request.Rooms.All(x => x.Id != r.RoomId)).ToList())
            {
                if (withReservations.Contains(removed.RoomId))
                {
                    throw new ConflictException(
                        $"{removed.RoomType.RoomTypeName} has reservations and cannot be removed. Set its number of rooms to 0 instead.");
                }

                Context.RemoveRange(removed.Prices);
                hotel.Rooms.Remove(removed);
                Context.Remove(removed);
            }

            foreach (var item in request.Rooms)
            {
                Room room;
                if (item.Id == 0)
                {
                    room = new Room();
                    hotel.Rooms.Add(room);
                }
                else
                {
                    room = hotel.Rooms.FirstOrDefault(r => r.RoomId == item.Id)
                        ?? throw new EntityNotFoundException("Room category", item.Id);

                    if (room.RoomTypeId != item.RoomTypeId && withReservations.Contains(room.RoomId))
                    {
                        throw new ConflictException($"{room.RoomType.RoomTypeName} has reservations, so its room type cannot change.");
                    }

                    var booked = maxBooked.GetValueOrDefault(room.RoomId);
                    if (item.NumberOfRooms < booked)
                    {
                        throw new ConflictException(
                            $"{booked} × {room.RoomType.RoomTypeName} are already booked for an upcoming trip, so there must be at least {booked}.");
                    }
                }

                room.RoomTypeId = item.RoomTypeId;
                room.NumberOfRooms = item.NumberOfRooms;
                room.AC = item.AC;
                room.Balcony = item.Balcony;
                SetPrice(room, item.PricePerNight);
            }

            if (request.Id == 0)
            {
                Context.Accommodation.Add(hotel);
            }

            Context.SaveChanges();
            return hotel.AccommodationId;
        }

        private static void SetPrice(Room room, decimal price)
        {
            var today = DateTime.Today;
            var latest = room.Prices.OrderByDescending(p => p.ValidFrom).FirstOrDefault();

            if (latest != null && latest.Price == price)
            {
                return;
            }

            if (latest != null && latest.ValidFrom.Date == today)
            {
                latest.Price = price;
                return;
            }

            room.Prices.Add(new RoomPrice { Price = price, ValidFrom = today });
        }
    }

    public class EfSaveSightCommand : EfUseCase, ISaveSightCommand
    {
        private readonly SaveSightValidator _validator;

        public EfSaveSightCommand(FlexTravelContext context, SaveSightValidator validator) : base(context)
        {
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.SaveSight;
        public string UseCaseName => "Save sight";
        public string UseCaseDescription => "Add or edit a sight of a destination";

        public int Execute(SaveSightDto request)
        {
            Attraction sight;
            if (request.Id == 0)
            {
                sight = new Attraction { DestinationId = request.DestinationId };
            }
            else
            {
                sight = Context.Attraction.FirstOrDefault(a => a.AttractionId == request.Id)
                    ?? throw new EntityNotFoundException("Sight", request.Id);
                request.DestinationId = sight.DestinationId;
            }

            _validator.ValidateAndThrow(request);

            sight.AttractionName = request.Name.Trim();
            sight.AttractionDescription = request.Description.Trim();
            sight.AttractionCategoryId = request.CategoryId;

            if (request.Id == 0)
            {
                Context.Attraction.Add(sight);
            }

            Context.SaveChanges();
            return sight.AttractionId;
        }
    }

    public class EfDeleteSightCommand : EfUseCase, IDeleteSightCommand
    {
        public EfDeleteSightCommand(FlexTravelContext context) : base(context)
        {
        }

        public int UseCaseId => UseCaseIds.DeleteSight;
        public string UseCaseName => "Delete sight";
        public string UseCaseDescription => "Remove a sight that no trip uses";

        public void Execute(int id)
        {
            var sight = Context.Attraction.Include(a => a.AttractionImages).FirstOrDefault(a => a.AttractionId == id)
                ?? throw new EntityNotFoundException("Sight", id);

            var trips = Context.TripAttraction.Count(t => t.AttractionId == id);
            if (trips > 0)
            {
                throw new ConflictException(
                    $"{sight.AttractionName} is on the program of {trips} trip(s). The agencies have to remove it from their trips first.");
            }

            Context.RemoveRange(sight.AttractionImages);
            Context.Attraction.Remove(sight);
            Context.SaveChanges();
        }
    }

    public class EfUploadImageCommand : IUploadImageCommand
    {
        private readonly IFileStorage _storage;
        private readonly UploadImageValidator _validator;

        public EfUploadImageCommand(IFileStorage storage, UploadImageValidator validator)
        {
            _storage = storage;
            _validator = validator;
        }

        public int UseCaseId => UseCaseIds.UploadImage;
        public string UseCaseName => "Upload image";
        public string UseCaseDescription => "Upload a destination image";

        public string Execute(UploadImageDto request)
        {
            _validator.ValidateAndThrow(request);

            return _storage.Save(UploadImageValidator.Extension(request), request.Content);
        }
    }

    public static class CatalogRules
    {
        public static Dictionary<int, int> MaxBookedRooms(FlexTravelContext context, BookingCalculator calculator, int hotelId)
        {
            var roomIds = context.Room.Where(r => r.AccommodationId == hotelId).Select(r => r.RoomId).ToList();
            var periods = context.Trip
                .Where(t => t.EndDate > DateTime.Today && t.Accommodations.Any(a => a.AccommodationId == hotelId))
                .Select(t => new { t.StartDate, t.EndDate })
                .Distinct()
                .ToList();

            var result = new Dictionary<int, int>();
            foreach (var period in periods)
            {
                foreach (var (roomId, booked) in calculator.BookedRooms(roomIds, period.StartDate, period.EndDate))
                {
                    result[roomId] = Math.Max(result.GetValueOrDefault(roomId), booked);
                }
            }

            return result;
        }
    }
}
