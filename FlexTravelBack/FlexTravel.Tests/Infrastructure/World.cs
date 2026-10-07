using FlexTravel.DataAccess;
using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Tests.Infrastructure
{
    public class World
    {
        public Agency Agency { get; private set; }
        public Agency OtherAgency { get; private set; }
        public AgencyEmployee Employee { get; private set; }
        public AgencyEmployee OtherEmployee { get; private set; }
        public RegularUser Customer { get; private set; }
        public RegularUser OtherCustomer { get; private set; }
        public Destination Destination { get; private set; }
        public Accommodation Hotel { get; private set; }
        public Room DoubleRoom { get; private set; }
        public Room FamilyRoom { get; private set; }
        public Transport Bus { get; private set; }

        private FlexTravelContext _context;

        public static World Create(FlexTravelContext context)
        {
            var continent = new Continent { ContinentName = "Europe" };
            var country = new Country { CountryName = "Testland", Continent = continent };
            var destination = new Destination { DestinationName = "Testville", DestinationDescription = "A town for tests.", Country = country };
            var hotelType = new AccommodationType { AccommodationTypeName = "Hotel" };
            var doubleType = new RoomType { RoomTypeName = "Double Room", Capacity = 2 };
            var familyType = new RoomType { RoomTypeName = "Quadruple Room", Capacity = 4 };

            var hotel = new Accommodation
            {
                AccommodationName = "Test Hotel",
                AccommodationStars = 4,
                Destination = destination,
                AccommodationType = hotelType
            };
            var doubleRoom = new Room { Accommodation = hotel, RoomType = doubleType, NumberOfRooms = 2 };
            var familyRoom = new Room { Accommodation = hotel, RoomType = familyType, NumberOfRooms = 1 };
            doubleRoom.Prices.Add(new RoomPrice { Price = 100, ValidFrom = DateTime.Today.AddYears(-1) });
            familyRoom.Prices.Add(new RoomPrice { Price = 150, ValidFrom = DateTime.Today.AddYears(-1) });

            var agency = new Agency { AgencyName = "Agency A", Address = "Street 1", Phone = "555" };
            var otherAgency = new Agency { AgencyName = "Agency B", Address = "Street 2", Phone = "556" };

            var world = new World
            {
                _context = context,
                Agency = agency,
                OtherAgency = otherAgency,
                Employee = Person(new AgencyEmployee { Agency = agency }, "Erin", "employee@test.local"),
                OtherEmployee = Person(new AgencyEmployee { Agency = otherAgency }, "Oscar", "other.employee@test.local"),
                Customer = Person(new RegularUser(), "Cleo", "customer@test.local"),
                OtherCustomer = Person(new RegularUser(), "Carl", "other.customer@test.local"),
                Destination = destination,
                Hotel = hotel,
                DoubleRoom = doubleRoom,
                FamilyRoom = familyRoom,
                Bus = new Transport { TransportName = "Bus" }
            };

            context.AddRange(hotel, doubleRoom, familyRoom, world.Bus, world.Employee, world.OtherEmployee, world.Customer, world.OtherCustomer);
            context.SaveChanges();
            return world;
        }

        private static T Person<T>(T user, string firstName, string email) where T : User
        {
            user.FirstName = firstName;
            user.LastName = "Test";
            user.Email = email;
            user.Password = "hash";
            user.Phone = "555";
            user.Address = "Street";
            user.City = "City";
            return user;
        }

        public Trip Trip(int startsInDays = 30, int nights = 5, int capacity = 6, decimal price = 500,
            int childDiscount = 50, Agency agency = null, Action<Trip> configure = null, TripSeries series = null)
        {
            var start = DateTime.Today.AddDays(startsInDays);
            var trip = new Trip
            {
                Agency = agency ?? Agency,
                Series = series ?? new TripSeries { Agency = agency ?? Agency, Name = "Test trip" },
                Transport = Bus,
                Destination = Destination,
                TripName = "Test trip",
                TripDescription = "A trip for tests.",
                StartDate = start,
                EndDate = start.AddDays(nights),
                Capacity = capacity,
                ChildDiscountPercent = childDiscount,
                Itinerary = new Itinerary { ItineraryName = "Test trip" }
            };
            trip.Prices.Add(new TripPrice { Price = price, ValidFrom = DateTime.Today.AddDays(-100) });
            trip.Accommodations.Add(new TripAccommodation { Accommodation = Hotel });
            configure?.Invoke(trip);

            _context.Trip.Add(trip);
            _context.SaveChanges();
            return trip;
        }

        public Reservation Reserve(Trip trip, RegularUser customer, int adults, int children = 0,
            ReservationStatus status = ReservationStatus.Pending, int doubleRooms = 1)
        {
            var reservation = new Reservation
            {
                Code = ReservationCodes.New(),
                Trip = trip,
                User = customer,
                Accommodation = Hotel,
                NumberOfAdults = adults,
                NumberOfChildren = children,
                Status = status,
                PricePerPerson = 500,
                Subtotal = 1000,
                TotalPrice = 1000
            };
            reservation.Rooms.Add(new ReservationRoom { Room = DoubleRoom, NumberOfRooms = doubleRooms, PricePerNight = 100 });
            reservation.StatusChanges.Add(new ReservationStatusChange { Status = ReservationStatus.Pending, ChangedAt = DateTime.UtcNow, ChangedBy = customer });
            if (status == ReservationStatus.Confirmed)
            {
                reservation.StatusChanges.Add(new ReservationStatusChange { Status = ReservationStatus.Confirmed, ChangedAt = DateTime.UtcNow, ChangedBy = Employee });
            }

            for (var i = 0; i < adults + children; i++)
            {
                reservation.Travelers.Add(new ReservationTraveler
                {
                    FirstName = "Traveler",
                    LastName = (i + 1).ToString(),
                    IsChild = i >= adults,
                    DateOfBirth = DateTime.Today.AddYears(i >= adults ? -8 : -35)
                });
            }

            _context.Reservation.Add(reservation);
            _context.SaveChanges();
            return reservation;
        }

        public PromoCode Promo(string code, int percent, Agency agency = null, int validFromDays = -1, int validToDays = 30, int? maxUses = null)
        {
            var promo = new PromoCode
            {
                Agency = agency ?? Agency,
                Code = code,
                DiscountPercent = percent,
                ValidFrom = DateTime.Today.AddDays(validFromDays),
                ValidTo = DateTime.Today.AddDays(validToDays),
                MaxUses = maxUses
            };

            _context.PromoCode.Add(promo);
            _context.SaveChanges();
            return promo;
        }
    }
}
