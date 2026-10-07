using FlexTravel.Domain;
using FlexTravel.Implementation.Booking;

namespace FlexTravel.Implementation.UseCases.Commands.SampleData
{
    public static class BookingSampleData
    {
        public class Result
        {
            public List<Transport> Transports { get; } = new List<Transport>();
            public List<Agency> Agencies { get; } = new List<Agency>();
            public List<User> Users { get; } = new List<User>();
            public List<Trip> Trips { get; } = new List<Trip>();
            public List<Reservation> Reservations { get; } = new List<Reservation>();
            public List<PromoCode> PromoCodes { get; } = new List<PromoCode>();
            public List<Review> Reviews { get; } = new List<Review>();
            public List<WaitlistEntry> Waitlist { get; } = new List<WaitlistEntry>();
            public List<UserFavorites> Favorites { get; } = new List<UserFavorites>();
            public List<PriceAlert> PriceAlerts { get; } = new List<PriceAlert>();
            public AttractionSampleData.Result Attractions { get; set; }
            public Dictionary<Agency, List<AgencyEmployee>> Staff { get; } = new Dictionary<Agency, List<AgencyEmployee>>();
        }

        public static Result Create(
            IReadOnlyCollection<Destination> destinations,
            IReadOnlyCollection<Accommodation> accommodations,
            RegularUser existingCustomer,
            Admin admin,
            Func<string, string> hashPassword,
            Func<User, User> withUseCases)
        {
            var today = DateTime.Today;
            var result = new Result();

            DestinationCoordinates.Apply(destinations);

            AddRoomPrices(accommodations, today);

            var plane = new Transport { TransportName = "Plane" };
            var bus = new Transport { TransportName = "Bus" };
            var train = new Transport { TransportName = "Train" };
            var ship = new Transport { TransportName = "Ship" };
            result.Transports.AddRange(new[] { plane, bus, train, ship });

            var horizon = new Agency
            {
                AgencyName = "Horizon Travel",
                Address = "Terazije 23, Belgrade",
                Phone = "+381 11 555 0101",
                Email = "info@horizon-travel.example",
                Website = "https://horizon-travel.example",
                Description = "City breaks and long-haul journeys from Belgrade since 2009. Small groups, English-speaking "
                    + "guides and flights with reliable connections. We plan every trip as if we were going ourselves."
            };
            var blueLagoon = new Agency
            {
                AgencyName = "Blue Lagoon Tours",
                Address = "Zmaj Jovina 8, Novi Sad",
                Phone = "+381 21 555 0202",
                Email = "hello@bluelagoon-tours.example",
                Website = "https://bluelagoon-tours.example",
                Description = "Beach holidays for families and couples: the Aegean, the Turkish Riviera, the Red Sea and the Caribbean. "
                    + "Direct charter flights where possible and a representative at every hotel."
            };
            var atlas = new Agency
            {
                AgencyName = "Atlas Journeys",
                Address = "Obrenoviceva 15, Nis",
                Phone = "+381 18 555 0303",
                Email = "office@atlas-journeys.example",
                Website = "https://atlas-journeys.example",
                Description = "History and culture trips with licensed local guides: Egypt, Greece, Provence and Russia, "
                    + "plus relaxed coach trips to Istanbul and Athens for every budget."
            };
            result.Agencies.AddRange(new[] { horizon, blueLagoon, atlas });

            var agencyPassword = hashPassword("Agency123!");
            var customerPassword = hashPassword("Korisnik123!");

            AgencyEmployee Employee(Agency agency, string first, string last, string email, string phone, string city) =>
                (AgencyEmployee)withUseCases(new AgencyEmployee
                {
                    FirstName = first,
                    LastName = last,
                    Email = email,
                    Password = agencyPassword,
                    Phone = phone,
                    Address = agency.Address.Split(',')[0],
                    City = city,
                    Agency = agency
                });

            var staff = result.Staff;
            staff[horizon] = new()
            {
                Employee(horizon, "Jelena", "Markovic", "horizon@flextravel.local", "+381 64 555 1001", "Belgrade"),
                Employee(horizon, "Nikola", "Stankovic", "horizon.nikola@flextravel.local", "+381 64 555 1004", "Belgrade")
            };
            staff[blueLagoon] = new()
            {
                Employee(blueLagoon, "Stefan", "Ilic", "bluelagoon@flextravel.local", "+381 64 555 1002", "Novi Sad"),
                Employee(blueLagoon, "Ivana", "Popovic", "bluelagoon.ivana@flextravel.local", "+381 64 555 1005", "Novi Sad")
            };
            staff[atlas] = new()
            {
                Employee(atlas, "Milica", "Djordjevic", "atlas@flextravel.local", "+381 64 555 1003", "Nis")
            };
            result.Users.AddRange(staff.Values.SelectMany(x => x));

            var turn = new Dictionary<Agency, int>();
            AgencyEmployee NextEmployee(Agency agency)
            {
                var index = turn.GetValueOrDefault(agency);
                turn[agency] = index + 1;
                return staff[agency][index % staff[agency].Count];
            }

            RegularUser Customer(string first, string last, string email, string phone, string address, string city)
            {
                var customer = (RegularUser)withUseCases(new RegularUser
                {
                    FirstName = first,
                    LastName = last,
                    Email = email,
                    Password = customerPassword,
                    Phone = phone,
                    Address = address,
                    City = city
                });
                result.Users.Add(customer);
                return customer;
            }

            var ana = Customer("Ana", "Jovanovic", "ana@flextravel.local", "+381 63 555 2001", "Bulevar kralja Aleksandra 100", "Belgrade");
            var marko = Customer("Marko", "Nikolic", "marko@flextravel.local", "+381 63 555 2002", "Kneza Milosa 12", "Kragujevac");
            var jovana = Customer("Jovana", "Savic", "jovana@flextravel.local", "+381 63 555 2003", "Njegoseva 7", "Belgrade");
            var luka = Customer("Luka", "Milosevic", "luka@flextravel.local", "+381 63 555 2004", "Futoska 44", "Novi Sad");
            var sara = Customer("Sara", "Kovacevic", "sara@flextravel.local", "+381 63 555 2005", "Vozdova 3", "Nis");
            var milan = Customer("Milan", "Todorovic", "milan@flextravel.local", "+381 63 555 2006", "Korzo 18", "Subotica");
            var petar = existingCustomer;

            DateTime Next(int month, int day)
            {
                var date = new DateTime(today.Year, month, day);
                return date < today.AddDays(21) ? date.AddYears(1) : date;
            }

            Trip NewTrip(Agency agency, Transport transport, string destinationName, string name, string description,
                DateTime start, int nights, int capacity, decimal price, int childDiscount, params (int Day, string Activity)[] days)
            {
                var destination = destinations.Single(d => d.DestinationName == destinationName);
                var trip = new Trip
                {
                    Agency = agency,
                    Transport = transport,
                    Destination = destination,
                    TripName = name,
                    TripDescription = description,
                    StartDate = start,
                    EndDate = start.AddDays(nights),
                    Capacity = capacity,
                    ChildDiscountPercent = childDiscount,
                    Itinerary = new Itinerary { ItineraryName = name },
                    CreatedBy = NextEmployee(agency),
                    Series = new TripSeries { Agency = agency, Name = name }
                };

                foreach (var (day, activity) in days)
                {
                    trip.Itinerary.ItineraryActivities.Add(new ItineraryActivity { Day = day, Activity = activity });
                }

                trip.Prices.Add(new TripPrice { Price = price, ValidFrom = today });

                foreach (var accommodation in accommodations.Where(a => a.Destination == destination && a.Rooms.Any()))
                {
                    trip.Accommodations.Add(new TripAccommodation { Accommodation = accommodation });
                }

                result.Trips.Add(trip);
                return trip;
            }

            Trip PastDeparture(Trip template, int startedDaysAgo, decimal price) =>
                Departure(template, today.AddDays(-startedDaysAgo), price, today.AddDays(-startedDaysAgo - 90));

            Trip Departure(Trip template, DateTime start, decimal price, DateTime published)
            {
                var trip = new Trip
                {
                    Agency = template.Agency,
                    Series = template.Series,
                    Transport = template.Transport,
                    Destination = template.Destination,
                    TripName = template.TripName,
                    TripDescription = template.TripDescription,
                    StartDate = start,
                    EndDate = start.AddDays((template.EndDate - template.StartDate).Days),
                    Capacity = template.Capacity,
                    ChildDiscountPercent = template.ChildDiscountPercent,
                    Itinerary = new Itinerary { ItineraryName = template.TripName },
                    CreatedBy = template.CreatedBy,
                    CreatedAt = published
                };

                foreach (var activity in template.Itinerary.ItineraryActivities)
                {
                    trip.Itinerary.ItineraryActivities.Add(new ItineraryActivity { Day = activity.Day, Activity = activity.Activity });
                }

                trip.Prices.Add(new TripPrice { Price = price, ValidFrom = published });

                foreach (var accommodation in template.Accommodations)
                {
                    trip.Accommodations.Add(new TripAccommodation { Accommodation = accommodation.Accommodation });
                }

                result.Trips.Add(trip);
                return trip;
            }

            var havana = NewTrip(horizon, plane, "Havana", "Havana: Rhythm of the Old Town",
                "Nine nights in Cuba's capital with guided walks through Old Havana, a classic-car ride along the Malecón and an evening of live son music. Return flights from Belgrade with one connection, airport transfers and an English-speaking guide are included.",
                Next(11, 12), 9, 30, 890, 40,
                (1, "Flight from Belgrade via Madrid to Havana. Transfer to the hotel and welcome drink."),
                (2, "Guided walking tour of Old Havana: Plaza de Armas, Plaza de la Catedral and Plaza Vieja."),
                (3, "Classic-car ride along the Malecón and through Vedado, ending at Revolution Square."),
                (5, "Optional day trip to the Viñales Valley and its tobacco farms."),
                (7, "Free day; evening of live son music in a Havana club."),
                (10, "Transfer to the airport and flight back to Belgrade (arrival the next day)."));

            NewTrip(atlas, plane, "Varadero", "Varadero Beach Escape",
                "Ten nights on one of the longest white-sand beaches in the Caribbean. A relaxed beach holiday with an optional catamaran trip and a day in Havana.",
                Next(1, 14), 10, 35, 920, 50,
                (1, "Flight from Belgrade to Varadero with one connection. Transfer to the hotel."),
                (2, "Welcome meeting with the agency representative; free time on the beach."),
                (4, "Optional catamaran trip with snorkelling at a coral reef."),
                (6, "Optional day trip to Havana."),
                (11, "Transfer to the airport and return flight to Belgrade."));

            NewTrip(atlas, plane, "Trinidad", "Colonial Trinidad & the Sugar Valley",
                "Cobbled streets, pastel houses and the Valley of the Sugar Mills: a UNESCO World Heritage town combined with beach days on nearby Playa Ancón.",
                Next(3, 11), 8, 25, 870, 40,
                (1, "Flight from Belgrade to Havana and transfer to Trinidad."),
                (2, "Guided tour of the historic centre and Plaza Mayor."),
                (3, "Excursion to the Valley of the Sugar Mills (Valle de los Ingenios) and the Manaca Iznaga tower."),
                (5, "Beach day at Playa Ancón."),
                (9, "Transfer to Havana and return flight to Belgrade."));

            var puntaCana = NewTrip(blueLagoon, plane, "Punta Cana", "Punta Cana All Inclusive",
                "Palm-fringed beaches and turquoise water on the eastern tip of the Dominican Republic. Made for families and couples who want a carefree holiday.",
                Next(2, 5), 10, 40, 950, 50,
                (1, "Flight from Belgrade to Punta Cana with one connection. Transfer to the hotel."),
                (2, "Welcome meeting and free time on the beach."),
                (4, "Optional catamaran excursion to Saona Island."),
                (7, "Optional buggy safari through the countryside and a swim in a cenote."),
                (11, "Transfer to the airport and return flight to Belgrade."));

            var santoDomingo = NewTrip(blueLagoon, plane, "Santo Domingo", "Santo Domingo: First City of the Americas",
                "A week in the oldest European city in the Americas: the Colonial Zone, caves of Los Tres Ojos and the beaches of Boca Chica. Small group of ten travelers.",
                Next(3, 4), 7, 10, 860, 30,
                (1, "Flight from Belgrade to Santo Domingo with one connection."),
                (2, "Guided tour of the Colonial Zone: Alcázar de Colón and the Cathedral of Santa María la Menor."),
                (4, "Los Tres Ojos caves and a walk along the Malecón."),
                (6, "Free day for shopping or the beach at Boca Chica."),
                (8, "Transfer to the airport and return flight."));

            NewTrip(horizon, plane, "Cancun", "Cancún Caribbean Holiday",
                "Ten nights in Cancún's hotel zone with optional trips to Chichén Itzá and Isla Mujeres.",
                Next(1, 21), 10, 30, 990, 40,
                (1, "Flight from Belgrade to Cancún with one connection. Transfer to the hotel zone."),
                (2, "Free day on the beach."),
                (4, "Optional excursion to Chichén Itzá and a swim in a cenote."),
                (6, "Optional catamaran trip to Isla Mujeres."),
                (11, "Transfer to the airport and return flight."));

            NewTrip(blueLagoon, plane, "Mexico City", "Mexico City: From the Aztecs to Frida",
                "A culture-packed week in one of the world's largest cities: the historic centre, the pyramids of Teotihuacan and the colourful canals of Xochimilco.",
                Next(3, 18), 7, 25, 880, 30,
                (1, "Flight from Belgrade to Mexico City with one connection."),
                (2, "Zócalo, the Metropolitan Cathedral and Templo Mayor."),
                (3, "Teotihuacan: Pyramids of the Sun and the Moon."),
                (4, "Frida Kahlo Museum in Coyoacán and the canals of Xochimilco."),
                (5, "National Museum of Anthropology and Chapultepec Park."),
                (8, "Transfer to the airport and return flight."));

            NewTrip(horizon, plane, "Rio de Janeiro", "Rio de Janeiro Discovery",
                "Christ the Redeemer, Sugarloaf Mountain and the beaches of Copacabana and Ipanema in nine unforgettable nights.",
                Next(11, 5), 9, 25, 1150, 30,
                (1, "Flight from Belgrade to Rio de Janeiro with one connection."),
                (2, "Christ the Redeemer on Corcovado and the Selarón Steps."),
                (3, "Cable car to Sugarloaf Mountain at sunset."),
                (5, "Beach day at Copacabana and Ipanema."),
                (7, "Optional boat trip to Ilha Grande."),
                (10, "Transfer to the airport and return flight."));

            var cairo = NewTrip(atlas, plane, "Cairo", "Cairo & the Pyramids of Giza",
                "Five nights in Cairo with the Pyramids of Giza, the Egyptian Museum and the Khan el-Khalili bazaar, guided by a licensed Egyptologist.",
                Next(11, 19), 5, 35, 390, 30,
                (1, "Flight from Belgrade to Cairo. Transfer to the hotel."),
                (2, "Pyramids of Giza and the Great Sphinx with an Egyptologist guide."),
                (3, "Egyptian Museum and the Khan el-Khalili bazaar."),
                (4, "Optional trip to Saqqara and Memphis."),
                (6, "Transfer to the airport and return flight to Belgrade."));

            NewTrip(atlas, plane, "Luxor", "Luxor: Valley of the Kings",
                "The temples of Karnak and Luxor, the royal tombs of the West Bank and an optional sunrise balloon flight over the Nile.",
                Next(2, 17), 6, 30, 420, 30,
                (1, "Flight from Belgrade to Luxor via Cairo. Transfer to the hotel."),
                (2, "Karnak and Luxor temples on the East Bank."),
                (3, "West Bank: Valley of the Kings, Temple of Hatshepsut and the Colossi of Memnon."),
                (5, "Optional sunrise hot-air balloon flight over the West Bank."),
                (7, "Transfer to the airport and return flight."));

            var sharm = NewTrip(blueLagoon, plane, "Sharm El Sheikh", "Red Sea Diving in Sharm El Sheikh",
                "A spring week on the Red Sea: coral reefs, Ras Mohammed National Park and an optional night climb of Mount Sinai.",
                Next(4, 8), 7, 45, 360, 50,
                (1, "Charter flight from Belgrade to Sharm El Sheikh. Transfer to the hotel."),
                (2, "Free day on the beach; snorkelling on the house reef."),
                (3, "Optional boat trip to Ras Mohammed National Park."),
                (5, "Optional night trip to Mount Sinai and St. Catherine's Monastery."),
                (8, "Transfer to the airport and return charter flight."));

            var paris = NewTrip(horizon, plane, "Paris", "Paris City Break",
                "Four nights in Paris before Christmas: the Louvre, Montmartre, an evening Seine cruise and an optional visit to Versailles.",
                Next(12, 3), 4, 35, 290, 20,
                (1, "Morning flight from Belgrade to Paris. Panoramic tour: Champs-Élysées, Arc de Triomphe, Eiffel Tower."),
                (2, "Guided visit to the Louvre; afternoon in Montmartre and Sacré-Cœur."),
                (3, "Optional trip to the Palace of Versailles. Evening cruise on the Seine."),
                (5, "Free morning and return flight to Belgrade."));

            NewTrip(horizon, plane, "Nice", "Nice & the Côte d'Azur",
                "The French Riviera in late spring: Old Nice, Monaco and the medieval village of Èze.",
                Next(5, 20), 6, 30, 320, 30,
                (1, "Flight from Belgrade to Nice. Walk along the Promenade des Anglais."),
                (2, "Old Town (Vieux Nice), the flower market and Castle Hill."),
                (3, "Excursion to Monaco and Èze."),
                (5, "Optional trip to Cannes and Antibes."),
                (7, "Transfer to the airport and return flight."));

            NewTrip(atlas, plane, "Avignon", "Provence: Avignon & the Lavender Fields",
                "Provence at its most fragrant: the Palace of the Popes, Sénanque Abbey among lavender fields and the Roman Pont du Gard.",
                Next(7, 1), 6, 30, 340, 30,
                (1, "Flight from Belgrade to Marseille and transfer to Avignon."),
                (2, "Palace of the Popes and the Pont d'Avignon."),
                (3, "Sénanque Abbey and the lavender fields of the Luberon."),
                (4, "Pont du Gard Roman aqueduct and Nîmes."),
                (7, "Transfer to Marseille and return flight."));

            var istanbul = NewTrip(atlas, bus, "Istanbul", "Istanbul: Between Two Continents",
                "Four nights in Istanbul by comfortable coach: Hagia Sophia, the Blue Mosque, Topkapı Palace, the Grand Bazaar and a Bosphorus cruise.",
                Next(4, 22), 4, 50, 150, 30,
                (1, "Morning departure by coach from Belgrade; arrival in Istanbul in the evening."),
                (2, "Sultanahmet: Hagia Sophia, the Blue Mosque and the Basilica Cistern."),
                (3, "Topkapı Palace and the Grand Bazaar."),
                (4, "Bosphorus cruise and the Asian side (Üsküdar)."),
                (5, "Morning departure and return to Belgrade in the evening."));

            var lastMinute = NewTrip(horizon, bus, "Istanbul", "Last Minute: Istanbul Long Weekend",
                "A short coach trip to Istanbul departing in a few days. Last free seats at a reduced price.",
                today.AddDays(5), 3, 50, 130, 30,
                (1, "Morning departure by coach from Belgrade; arrival in Istanbul in the evening."),
                (2, "Sultanahmet: Hagia Sophia, the Blue Mosque and the Basilica Cistern."),
                (3, "Grand Bazaar and an afternoon Bosphorus cruise."),
                (4, "Morning departure and return to Belgrade in the evening."));

            var cappadocia = NewTrip(horizon, plane, "Cappadocia", "Cappadocia Balloon Adventure",
                "Fairy chimneys, cave churches and underground cities, with an optional sunrise balloon flight over Göreme.",
                Next(5, 6), 5, 30, 340, 30,
                (1, "Flight from Belgrade to Kayseri via Istanbul and transfer to Göreme."),
                (2, "Optional sunrise hot-air balloon flight; Göreme Open-Air Museum."),
                (3, "Derinkuyu underground city and Uçhisar castle."),
                (4, "Hike through the Rose and Red valleys."),
                (6, "Transfer to Kayseri and return flight."));

            var antalyaSummer = NewTrip(blueLagoon, plane, "Antalya", "Antalya Summer Holiday",
                "Ten nights on the Turkish Riviera in early summer, with optional trips to the Düden Waterfalls and the old town of Kaleiçi.",
                Next(6, 18), 10, 45, 280, 50,
                (1, "Charter flight from Belgrade to Antalya. Transfer to the hotel."),
                (2, "Free day on the beach."),
                (4, "Optional trip to the Düden Waterfalls and Kaleiçi old town."),
                (7, "Optional boat trip along the coast."),
                (11, "Transfer to the airport and return charter flight."));

            NewTrip(horizon, plane, "Antalya", "Antalya Late Summer",
                "A week of warm sea and fewer crowds in September, with an optional visit to ancient Perge and the theatre of Aspendos.",
                Next(9, 2), 7, 40, 260, 50,
                (1, "Charter flight from Belgrade to Antalya. Transfer to the hotel."),
                (2, "Free day on the beach."),
                (4, "Optional excursion to ancient Perge and the theatre of Aspendos."),
                (8, "Transfer to the airport and return charter flight."));

            NewTrip(atlas, bus, "Athens", "Athens: Cradle of Democracy",
                "Five nights in Athens by coach: the Acropolis, Plaka and an optional sunset at Cape Sounion.",
                Next(4, 29), 5, 50, 140, 30,
                (1, "Early morning departure by coach from Belgrade; arrival in Athens in the evening."),
                (2, "Acropolis, the Parthenon and the Acropolis Museum."),
                (3, "Plaka and Monastiraki; changing of the guard at Syntagma Square."),
                (4, "Optional trip to Cape Sounion and the Temple of Poseidon."),
                (6, "Morning departure and return to Belgrade late in the evening."));

            var santorini = NewTrip(blueLagoon, plane, "Santorini", "Santorini Sunsets",
                "A week on the most photographed island of the Aegean: the caldera path, sunsets in Oia and a catamaran cruise to the volcano.",
                Next(6, 4), 7, 35, 310, 30,
                (1, "Flight from Belgrade to Santorini (Thira). Transfer to the hotel."),
                (2, "Fira and the caldera path to Imerovigli."),
                (3, "Sunset in Oia."),
                (4, "Optional catamaran cruise to the volcano and hot springs."),
                (5, "Akrotiri archaeological site and the Red Beach."),
                (8, "Transfer to the airport and return flight."));

            NewTrip(horizon, plane, "Santorini", "Santorini in September",
                "Six nights on Santorini after the summer peak, with warm sea, quieter villages and wine tasting near Pyrgos.",
                Next(9, 9), 6, 30, 290, 30,
                (1, "Flight from Belgrade to Santorini (Thira). Transfer to the hotel."),
                (2, "Fira, Imerovigli and sunset in Oia."),
                (4, "Optional wine tasting in the villages around Pyrgos."),
                (7, "Transfer to the airport and return flight."));

            var crete = NewTrip(atlas, plane, "Crete", "Crete Family Summer",
                "Ten nights of beaches and history on Greece's largest island, with optional trips to Knossos and Spinalonga. Children travel at half price.",
                Next(7, 15), 10, 45, 290, 50,
                (1, "Charter flight from Belgrade to Heraklion. Transfer to the hotel."),
                (2, "Free day on the beach."),
                (3, "Optional visit to the Palace of Knossos."),
                (6, "Optional boat trip to Spinalonga island."),
                (8, "Optional day at Elafonisi beach."),
                (11, "Transfer to the airport and return charter flight."));

            NewTrip(atlas, plane, "Saint Petersburg", "St. Petersburg White Nights",
                "The city of palaces during the White Nights, when the sun barely sets: the Hermitage, Peterhof and an evening canal cruise.",
                Next(6, 17), 5, 30, 420, 20,
                (1, "Flight from Belgrade to St. Petersburg."),
                (2, "Hermitage Museum and Palace Square."),
                (3, "Peterhof palace and fountains."),
                (4, "Peter and Paul Fortress; evening canal cruise during the White Nights."),
                (6, "Transfer to the airport and return flight."));

            NewTrip(horizon, plane, "Moscow", "Moscow & the Kremlin",
                "Red Square, the Kremlin, the Tretyakov Gallery and the palatial stations of the Moscow Metro.",
                Next(9, 15), 5, 30, 390, 20,
                (1, "Flight from Belgrade to Moscow."),
                (2, "Red Square, St. Basil's Cathedral and the Kremlin."),
                (3, "Tretyakov Gallery and a ride through the Moscow Metro's most beautiful stations."),
                (4, "Optional trip to Sergiev Posad."),
                (6, "Transfer to the airport and return flight."));

            santorini.Prices.First().ValidFrom = today.AddDays(-25);
            santorini.Prices.Add(new TripPrice { Price = 340, ValidFrom = today.AddDays(-60) });
            santorini.CreatedAt = DateTime.Now.AddDays(-60);
            santorini.UpdatedBy = NextEmployee(blueLagoon);
            santorini.UpdatedAt = DateTime.UtcNow.AddDays(-25);

            void EarlyBooking(Trip trip, int percent, DateTime until)
            {
                trip.EarlyBookingPercent = percent;
                trip.EarlyBookingUntil = until;
            }

            void LastMinute(Trip trip, int percent, int days)
            {
                trip.LastMinutePercent = percent;
                trip.LastMinuteDays = days;
            }

            EarlyBooking(antalyaSummer, 15, new DateTime(antalyaSummer.StartDate.Year, 1, 31));
            EarlyBooking(crete, 10, new DateTime(crete.StartDate.Year, 2, 28));
            EarlyBooking(santorini, 10, new DateTime(santorini.StartDate.Year - 1, 12, 31));
            EarlyBooking(puntaCana, 10, today.AddDays(40));
            EarlyBooking(sharm, 12, today.AddDays(70));
            LastMinute(lastMinute, 20, 7);
            LastMinute(havana, 12, 14);
            LastMinute(cairo, 10, 21);
            LastMinute(paris, 15, 10);

            PromoCode Promo(Agency agency, string code, string description, int percent, DateTime from, DateTime to, int? maxUses)
            {
                var promo = new PromoCode
                {
                    Agency = agency,
                    Code = code,
                    Description = description,
                    DiscountPercent = percent,
                    ValidFrom = from,
                    ValidTo = to,
                    MaxUses = maxUses,
                    CreatedBy = staff[agency][0],
                    CreatedAt = from.AddDays(-3)
                };
                result.PromoCodes.Add(promo);
                return promo;
            }

            var horizon10 = Promo(horizon, "HORIZON10", "Autumn newsletter subscribers", 10, today.AddDays(-10), today.AddDays(80), 50);
            Promo(horizon, "WINTER5", "Last winter campaign", 5, today.AddDays(-120), today.AddDays(-10), null);
            Promo(blueLagoon, "BLUE2027", "Early 2027 campaign", 8, today.AddDays(-5), new DateTime(today.Year + 1, 3, 31), 20);
            var atlasFamily = Promo(atlas, "ATLASFAMILY", "Family travel fair", 7, today.AddDays(-30), today.AddDays(150), null);

            var pastSantorini = PastDeparture(santorini, 75, 320);
            var pastParis = PastDeparture(paris, 52, 300);
            var pastIstanbul = PastDeparture(istanbul, 33, 150);
            var pastCairo = PastDeparture(cairo, 110, 380);
            var pastAntalya = PastDeparture(antalyaSummer, 100, 280);
            var pastCappadocia = PastDeparture(cappadocia, 18, 330);

            var istanbulAutumn = Departure(istanbul, today.AddDays(48), 145, today.AddDays(-10));
            Departure(cairo, Next(3, 15), 400, today.AddDays(-5));
            Departure(santorini, santorini.StartDate.AddDays(21), 330, today.AddDays(-3));

            AddPlanning(result.Trips);

            Reservation Book(RegularUser customer, Trip trip, int hotelIndex, int adults, int children,
                ReservationStatus status, int daysAgo, params (string RoomType, int Count)[] rooms)
            {
                var hotel = trip.Accommodations
                    .Select(a => a.Accommodation)
                    .OrderByDescending(a => a.AccommodationStars)
                    .ThenBy(a => a.AccommodationName)
                    .ElementAt(hotelIndex);
                var nights = BookingPricing.Nights(trip.StartDate, trip.EndDate);
                var price = trip.Prices.First().Price;
                var createdAt = DateTime.Now.AddDays(-daysAgo);

                var reservation = new Reservation
                {
                    Code = ReservationCodes.New(),
                    User = customer,
                    Trip = trip,
                    Accommodation = hotel,
                    NumberOfAdults = adults,
                    NumberOfChildren = children,
                    Status = status,
                    StatusChangedAt = status == ReservationStatus.Pending ? null : createdAt.AddDays(1).ToUniversalTime(),
                    PricePerPerson = price,
                    ChildDiscountPercent = trip.ChildDiscountPercent,
                    CreatedAt = createdAt
                };

                decimal roomsTotal = 0;
                foreach (var (roomType, count) in rooms)
                {
                    var room = hotel.Rooms.First(r => r.RoomType.RoomTypeName == roomType);
                    var pricePerNight = room.Prices.First().Price;
                    reservation.Rooms.Add(new ReservationRoom { Room = room, NumberOfRooms = count, PricePerNight = pricePerNight });
                    roomsTotal += BookingPricing.RoomTotal(pricePerNight, count, nights);
                }

                reservation.Subtotal = BookingPricing.TravelersTotal(price, trip.ChildDiscountPercent, adults, children) + roomsTotal;
                (reservation.Offer, reservation.OfferPercent) = BookingPricing.OfferAt(trip, createdAt);
                Reprice(reservation);

                var bookedAt = createdAt.ToUniversalTime();
                reservation.StatusChanges.Add(new ReservationStatusChange
                {
                    Status = ReservationStatus.Pending,
                    ChangedAt = bookedAt,
                    ChangedBy = customer
                });

                if (status != ReservationStatus.Pending)
                {
                    reservation.StatusChanges.Add(new ReservationStatusChange
                    {
                        Status = status == ReservationStatus.Rejected ? ReservationStatus.Rejected : ReservationStatus.Confirmed,
                        ChangedAt = bookedAt.AddDays(1),
                        ChangedBy = NextEmployee(trip.Agency)
                    });
                }

                if (status == ReservationStatus.Cancelled)
                {
                    var feePercent = CancellationRules.FeePercent(ReservationStatus.Confirmed, trip.StartDate, createdAt.AddDays(3));
                    reservation.CancellationFee = CancellationRules.Fee(reservation.TotalPrice, feePercent);
                    reservation.StatusChangedAt = bookedAt.AddDays(3);
                    reservation.StatusChanges.Add(new ReservationStatusChange
                    {
                        Status = ReservationStatus.Cancelled,
                        ChangedAt = bookedAt.AddDays(3),
                        ChangedBy = customer,
                        Comment = feePercent > 0
                            ? $"Cancelled by the customer. Cancellation fee {feePercent}%: {BookingPricing.Euro(reservation.CancellationFee.Value)}."
                            : "Cancelled by the customer, free of charge."
                    });
                }

                AddTravelers(reservation, customer, trip, adults, children);
                AddPayments(reservation, trip, status, bookedAt);

                result.Reservations.Add(reservation);
                return reservation;
            }

            string[] companions = { "Milena", "Dragan", "Jelena", "Nemanja", "Tamara", "Vuk", "Teodora", "Lazar" };
            string[] kids = { "Mia", "Luka", "Sofija", "Pavle", "Nina", "Andrej" };
            var person = 0;

            void AddTravelers(Reservation reservation, RegularUser customer, Trip trip, int adults, int children)
            {
                person++;
                reservation.Travelers.Add(new ReservationTraveler
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    DateOfBirth = trip.StartDate.AddYears(-(31 + person % 12)).AddDays(-(person * 37 % 300))
                });

                for (var i = 1; i < adults; i++)
                {
                    reservation.Travelers.Add(new ReservationTraveler
                    {
                        FirstName = companions[(person + i) % companions.Length],
                        LastName = customer.LastName,
                        DateOfBirth = trip.StartDate.AddYears(-(29 + (person + i) % 15)).AddDays(-(i * 53 % 300))
                    });
                }

                for (var c = 0; c < children; c++)
                {
                    reservation.Travelers.Add(new ReservationTraveler
                    {
                        FirstName = kids[(person + c) % kids.Length],
                        LastName = customer.LastName,
                        IsChild = true,
                        DateOfBirth = trip.StartDate.AddYears(-(5 + (person + c) % 6)).AddDays(-40)
                    });
                }
            }

            void AddPayments(Reservation reservation, Trip trip, ReservationStatus status, DateTime bookedAt)
            {
                var employee = staff[trip.Agency][0];
                var depositAt = bookedAt.AddDays(3);
                if (status == ReservationStatus.Pending || status == ReservationStatus.Rejected || depositAt > DateTime.UtcNow)
                {
                    return;
                }

                var deposit = PaymentRules.Deposit(reservation.TotalPrice);
                reservation.Payments.Add(new Payment
                {
                    Amount = deposit,
                    Method = PaymentMethod.Card,
                    PaidAt = depositAt,
                    RecordedBy = employee,
                    Note = "Deposit"
                });

                if (status == ReservationStatus.Cancelled)
                {
                    var refund = deposit - (reservation.CancellationFee ?? 0);
                    if (refund > 0)
                    {
                        reservation.Payments.Add(new Payment
                        {
                            Amount = -refund,
                            Method = PaymentMethod.BankTransfer,
                            PaidAt = depositAt.AddDays(2),
                            RecordedBy = employee,
                            Note = "Refund after cancellation"
                        });
                    }
                }
                else if (trip.EndDate < today)
                {
                    reservation.Payments.Add(new Payment
                    {
                        Amount = reservation.TotalPrice - deposit,
                        Method = PaymentMethod.BankTransfer,
                        PaidAt = trip.StartDate.AddDays(-25).ToUniversalTime(),
                        RecordedBy = employee,
                        Note = "Balance"
                    });
                }
            }

            void UsePromo(Reservation reservation, PromoCode promo)
            {
                reservation.PromoCode = promo;
                reservation.PromoPercent = promo.DiscountPercent;
                Reprice(reservation);
                AdjustPayments(reservation);
            }

            Reservation WithExtras(Reservation reservation, params ExtraKind[] kinds)
            {
                var travelers = reservation.NumberOfAdults + reservation.NumberOfChildren;
                foreach (var extra in reservation.Trip.Extras.Where(e => kinds.Contains(e.Kind)))
                {
                    var quantity = extra.PerPerson ? travelers : 1;
                    reservation.Extras.Add(new ReservationExtra
                    {
                        TripExtra = extra,
                        Kind = extra.Kind,
                        Name = extra.Name,
                        UnitPrice = extra.Price,
                        Quantity = quantity,
                        Total = ExtraRules.Total(extra.Price, quantity)
                    });
                }

                Reprice(reservation);
                AdjustPayments(reservation);
                return reservation;
            }

            void AdjustPayments(Reservation reservation)
            {
                var deposit = PaymentRules.Deposit(reservation.TotalPrice);
                foreach (var payment in reservation.Payments)
                {
                    payment.Amount = payment.Note == "Deposit" ? deposit
                        : payment.Note == "Balance" ? reservation.TotalPrice - deposit
                        : payment.Amount;
                }
            }

            var petarSantorini = WithExtras(Book(petar, santorini, 0, 2, 0, ReservationStatus.Confirmed, 20, ("Double Room", 1)),
                ExtraKind.Insurance, ExtraKind.Excursion);
            UsePromo(Book(petar, paris, 1, 2, 1, ReservationStatus.Pending, 2, ("Triple Room", 1)), horizon10);
            Book(petar, cairo, 0, 2, 0, ReservationStatus.Cancelled, 30, ("Double Room", 1));
            WithExtras(Book(ana, antalyaSummer, 0, 2, 2, ReservationStatus.Confirmed, 15, ("Quadruple Room", 1)), ExtraKind.Insurance);
            var anaHavana = WithExtras(Book(ana, havana, 1, 2, 0, ReservationStatus.Pending, 1, ("Double Room", 1)),
                ExtraKind.Insurance, ExtraKind.Transfer);
            UsePromo(Book(ana, crete, 0, 2, 1, ReservationStatus.Pending, 2, ("Triple Room", 1)), atlasFamily);
            Book(ana, santoDomingo, 0, 4, 2, ReservationStatus.Confirmed, 25, ("Quadruple Room", 1), ("Double Room", 1));
            Book(marko, santoDomingo, 1, 4, 0, ReservationStatus.Pending, 2, ("Double Room", 2));
            Book(marko, lastMinute, 0, 1, 0, ReservationStatus.Confirmed, 6, ("Double Room", 1));
            Book(marko, santorini, 1, 2, 0, ReservationStatus.Pending, 0, ("Double Room", 1));

            Book(luka, istanbulAutumn, 1, 2, 0, ReservationStatus.Confirmed, 4, ("Double Room", 1));

            Book(jovana, santorini, 0, 2, 0, ReservationStatus.Pending, 4, ("Double Room", 1));
            var saraCappadocia = WithExtras(Book(sara, cappadocia, 0, 2, 0, ReservationStatus.Confirmed, 5, ("Double Room", 1)),
                ExtraKind.Insurance);
            saraCappadocia.Payments.Clear();

            var rejected = Book(marko, paris, 0, 2, 0, ReservationStatus.Rejected, 5, ("Master Bed Suite", 1));
            rejected.StatusChanges.Last().Comment =
                "The hotel's suites are reserved for a corporate group on these dates. We offered a double room instead.";

            Reservation Travelled(RegularUser customer, Trip trip, int hotelIndex, int adults, int children, params (string RoomType, int Count)[] rooms) =>
                Book(customer, trip, hotelIndex, adults, children, ReservationStatus.Confirmed,
                    (today - trip.StartDate).Days + 30 + result.Reservations.Count % 7, rooms);

            var santoriniAna = Travelled(ana, pastSantorini, 0, 2, 0, ("Double Room", 1));
            var santoriniLuka = Travelled(luka, pastSantorini, 1, 2, 0, ("Double Room", 1));
            var santoriniMilan = Travelled(milan, pastSantorini, 0, 2, 1, ("Triple Room", 1));
            var parisSara = Travelled(sara, pastParis, 1, 2, 0, ("Double Room", 1));
            var parisMarko = Travelled(marko, pastParis, 2, 1, 0, ("Double Room", 1));
            var parisJovana = Travelled(jovana, pastParis, 2, 2, 0, ("Double Room", 1));
            var istanbulPetar = Travelled(petar, pastIstanbul, 1, 2, 0, ("Double Room", 1));
            var istanbulJovana = Travelled(jovana, pastIstanbul, 1, 2, 2, ("Quadruple Room", 1));
            Travelled(ana, pastIstanbul, 0, 2, 0, ("Double Room", 1));
            var cairoLuka = Travelled(luka, pastCairo, 0, 2, 0, ("Double Room", 1));
            var cairoSara = Travelled(sara, pastCairo, 2, 1, 0, ("Double Room", 1));
            var antalyaMilan = Travelled(milan, pastAntalya, 0, 2, 2, ("Quadruple Room", 1));
            var antalyaJovana = Travelled(jovana, pastAntalya, 2, 2, 0, ("Double Room", 1));
            var cappadociaAna = Travelled(ana, pastCappadocia, 0, 2, 0, ("Double Room", 1));
            var cappadociaSara = Travelled(sara, pastCappadocia, 1, 2, 0, ("Double Room", 1));
            Travelled(petar, pastCappadocia, 1, 2, 0, ("Double Room", 1));

            Review Rate(Reservation reservation, int tripRating, int hotelRating, string comment, int daysAfterReturn)
            {
                int Near(int rating, int shift) => Math.Clamp(rating + shift, 1, 5);

                var review = new Review
                {
                    Reservation = reservation,
                    User = reservation.User,
                    Trip = reservation.Trip,
                    Accommodation = reservation.Accommodation,
                    TripRating = tripRating,
                    HotelRating = hotelRating,
                    GuideRating = Near(tripRating, comment.Length % 3 == 0 ? 1 : 0),
                    OrganisationRating = Near(tripRating, comment.Length % 4 == 1 ? -1 : 0),
                    ValueRating = Near((tripRating + hotelRating + 1) / 2, 0),
                    Comment = comment,
                    CreatedAt = reservation.Trip.EndDate.AddDays(daysAfterReturn).AddHours(19)
                };
                result.Reviews.Add(review);
                return review;
            }

            void Reply(Review review, AgencyEmployee employee, string text)
            {
                review.AgencyReply = text;
                review.AgencyReplyBy = employee;
                review.AgencyReplyAt = review.CreatedAt.AddDays(1).ToUniversalTime();
            }

            Reply(Rate(santoriniAna, 5, 5,
                    "Unforgettable week. The caldera walk from Fira to Imerovigli and the sunset in Oia were the highlights. Our guide knew every corner of the island and the transfers were always on time.", 2),
                staff[blueLagoon][1],
                "Thank you, Ana! We passed your compliments on to our guide. We hope to see you again in the Aegean.");
            Rate(santoriniLuka, 4, 3,
                "Beautiful island and a well-organised program. The hotel room was smaller than in the photos and the air conditioning was noisy, but the location was great.", 4);
            Rate(santoriniMilan, 5, 4,
                "We travelled with our eight-year-old and everything was child-friendly. The cruise to the volcano was the best day of the holiday.", 6);

            var parisSaraReview = Rate(parisSara, 4, 4,
                "Paris in four days is a rush, but the program covered all the essentials. The Louvre tour with the guide was excellent; I would add one free afternoon.", 3);
            parisSaraReview.GuideRating = parisSaraReview.OrganisationRating = parisSaraReview.ValueRating = null;
            Reply(Rate(parisMarko, 3, 2,
                    "The guided part was good, but the coach to Versailles was 40 minutes late and breakfast at the hotel was poor for the price.", 1),
                staff[horizon][1],
                "Dear Marko, we apologise for the delay. We have changed the coach company for the next departures and passed your comments on to the hotel.");
            var advert = Rate(parisJovana, 1, 1,
                "Call me on +381 64 000 0000 for much cheaper tickets to Paris, better than any agency!!!", 2);
            advert.IsActive = false;
            advert.ModerationNote = "Advertising and contact details are not allowed in reviews.";
            advert.ModeratedBy = admin;
            advert.ModeratedAt = advert.CreatedAt.AddDays(1).ToUniversalTime();

            Rate(istanbulPetar, 4, 4,
                "The coach ride is long, but the seats were comfortable and the breaks well planned. Hagia Sophia and the Bosphorus cruise were worth it, and the agency answered all our questions before the trip.", 2);
            Rate(istanbulJovana, 5, 4,
                "Great value for a family of four. The guide made history interesting even for the kids, and the Grand Bazaar was a fun adventure.", 5);

            Rate(cairoLuka, 5, 4,
                "Standing in front of the pyramids is something everyone should experience. The Egyptologist guide was brilliant and patient with all our questions.", 3);
            Rate(cairoSara, 4, 3,
                "Interesting program and a safe, well-organised trip. The hotel was fine but far from the centre, so the evenings were mostly spent there.", 8);

            Rate(antalyaMilan, 4, 5,
                "A relaxing family holiday. The hotel pool and the beach were excellent and the kids' club kept our children busy. The Düden Waterfalls trip was a nice break from the beach.", 2);
            Reply(Rate(antalyaJovana, 3, 3,
                    "A good beach holiday, but the transfer from the airport took almost two hours because the bus stopped at many other hotels.", 3),
                staff[blueLagoon][0],
                "Thank you for the feedback, Jovana. From next season our guests have direct transfers to their hotel.");

            Rate(cappadociaAna, 5, 5,
                "The balloon flight at sunrise was magical, and the cave hotel was the most unusual place we have ever stayed. Perfectly organised from start to finish.", 1);
            Rate(cappadociaSara, 4, 4,
                "A fascinating landscape, and the underground city was amazing. Hiking through the Rose Valley at sunset was my favourite moment.", 4);

            void Message(Reservation reservation, User sender, string text, double hoursAgo, bool read)
            {
                var sentAt = DateTime.UtcNow.AddHours(-hoursAgo);
                reservation.Messages.Add(new ReservationMessage
                {
                    Sender = sender,
                    Text = text,
                    SentAt = sentAt,
                    ReadAt = read ? sentAt.AddMinutes(40) : null
                });
            }

            Message(petarSantorini, petar, "Hello! Is the airport transfer included on the way back as well?", 24 * 18, true);
            Message(petarSantorini, staff[blueLagoon][1],
                "Yes, the transfer is included both ways. The bus leaves the hotel three hours before the flight.", 24 * 18 - 3, true);
            Message(anaHavana, ana,
                "Hi! We are celebrating our anniversary on this trip. Is it possible to get a room with a sea view?", 20, false);

            result.Waitlist.Add(new WaitlistEntry { Trip = santoDomingo, User = petar, Adults = 2, CreatedAt = DateTime.Now.AddDays(-2) });
            result.Waitlist.Add(new WaitlistEntry { Trip = santoDomingo, User = sara, Adults = 2, Children = 1, CreatedAt = DateTime.Now.AddDays(-1) });

            void Favorite(RegularUser customer, string destinationName) =>
                result.Favorites.Add(new UserFavorites { User = customer, Destination = destinations.Single(d => d.DestinationName == destinationName) });

            Favorite(petar, "Santorini");
            Favorite(petar, "Cappadocia");
            Favorite(petar, "Machu Picchu");
            Favorite(ana, "Paris");
            Favorite(ana, "Venice");
            Favorite(marko, "Istanbul");
            Favorite(marko, "Crete");

            void Alert(RegularUser customer, string destinationName, decimal maxPrice, int daysAgo) =>
                result.PriceAlerts.Add(new PriceAlert
                {
                    User = customer,
                    Destination = destinations.Single(d => d.DestinationName == destinationName),
                    MaxPrice = maxPrice,
                    CreatedAt = DateTime.UtcNow.AddDays(-daysAgo)
                });

            Alert(petar, "Santorini", 1500, 6);
            Alert(ana, "Paris", 300, 3);
            Alert(marko, "Crete", 450, 9);

            result.Attractions = AttractionSampleData.Create(destinations, result.Trips);

            return result;
        }

        private static void Reprice(Reservation reservation)
        {
            reservation.DiscountTotal = BookingPricing.Discount(reservation.Subtotal, reservation.OfferPercent)
                + BookingPricing.Discount(reservation.Subtotal, reservation.PromoPercent);
            reservation.ExtrasTotal = reservation.Extras.Sum(e => e.Total);
            reservation.TotalPrice = reservation.Subtotal - reservation.DiscountTotal + reservation.ExtrasTotal;
        }

        private static readonly Dictionary<string, Dictionary<int, string>> DayTitles = new()
        {
            ["Havana: Rhythm of the Old Town"] = new()
            {
                [1] = "Arrival in Havana",
                [2] = "Old Havana on foot",
                [3] = "Classic cars and the Malecón",
                [5] = "Viñales Valley (optional)",
                [7] = "Free day and live son",
                [10] = "Homeward"
            },
            ["Paris City Break"] = new()
            {
                [1] = "Bonjour Paris",
                [2] = "The Louvre and Montmartre",
                [3] = "Versailles and the Seine",
                [5] = "Au revoir"
            },
            ["Istanbul: Between Two Continents"] = new()
            {
                [1] = "On the road to Istanbul",
                [2] = "Sultanahmet",
                [3] = "Palace and bazaar",
                [4] = "Across the Bosphorus",
                [5] = "Back to Belgrade"
            },
            ["Cappadocia Balloon Adventure"] = new()
            {
                [1] = "Arrival in Göreme",
                [2] = "Balloons at sunrise",
                [3] = "Underground cities",
                [4] = "Rose and Red valleys",
                [6] = "Homeward"
            },
            ["Santorini Sunsets"] = new()
            {
                [1] = "Arrival on Santorini",
                [2] = "The caldera path",
                [3] = "Sunset in Oia",
                [4] = "Volcano cruise (optional)",
                [5] = "Akrotiri and the Red Beach",
                [8] = "Homeward"
            }
        };

        private static void AddPlanning(IEnumerable<Trip> trips)
        {
            foreach (var trip in trips)
            {
                var nights = BookingPricing.Nights(trip.StartDate, trip.EndDate);
                var allInclusive = trip.TripName.Contains("All Inclusive", StringComparison.OrdinalIgnoreCase);
                var titles = DayTitles.GetValueOrDefault(trip.TripName);
                var byBus = trip.Transport.TransportName == "Bus";

                foreach (var day in trip.Itinerary.ItineraryActivities)
                {
                    day.Title = titles?.GetValueOrDefault(day.Day);
                    day.Overnight = day.Day <= nights ? trip.Destination.DestinationName : null;
                    day.Meals = day.Day == 1
                        ? (allInclusive ? Meals.Dinner : Meals.None)
                        : allInclusive && day.Day <= nights ? Meals.Breakfast | Meals.Lunch | Meals.Dinner : Meals.Breakfast;
                }

                trip.Extras.Add(new TripExtra
                {
                    Kind = ExtraKind.Insurance,
                    Name = "Travel insurance",
                    Description = "Medical costs abroad up to €30,000, lost luggage and trip cancellation for illness.",
                    Price = byBus ? 12 : nights >= 7 ? 35 : 22,
                    PerPerson = true
                });

                if (!byBus)
                {
                    trip.Extras.Add(new TripExtra
                    {
                        Kind = ExtraKind.Transfer,
                        Name = "Private airport transfer",
                        Description = "A private car with a driver instead of the group bus, both ways.",
                        Price = 45,
                        PerPerson = false
                    });
                }

                foreach (var day in trip.Itinerary.ItineraryActivities.Where(a => a.Activity.StartsWith("Optional ")))
                {
                    var text = day.Activity["Optional ".Length..].Split(new[] { ". ", "; " }, StringSplitOptions.None)[0].TrimEnd('.');
                    var name = char.ToUpperInvariant(text[0]) + text[1..];
                    trip.Extras.Add(new TripExtra
                    {
                        Kind = ExtraKind.Excursion,
                        Name = name.Length <= 100 ? name : name[..100],
                        Description = $"Day {day.Day} of the program, with a guide and transport.",
                        Price = ExcursionPrice(name),
                        PerPerson = true
                    });
                }
            }
        }

        private static decimal ExcursionPrice(string name)
        {
            var text = name.ToLowerInvariant();
            return text.Contains("balloon") ? 190
                : text.Contains("catamaran") || text.Contains("boat") || text.Contains("cruise") ? 70
                : text.Contains("safari") || text.Contains("buggy") ? 65
                : text.Contains("day trip") || text.Contains("excursion") ? 55
                : 45;
        }

        public static List<Notification> Notifications(Result result)
        {
            var now = DateTime.UtcNow;
            var list = new List<Notification>();

            DateTime? ReadIfOlderThanTwoDays(DateTime at) => (now - at).TotalDays > 2 ? at.AddHours(2) : null;

            void Add(User user, NotificationType type, string title, string message, string link, DateTime at, DateTime? readAt) =>
                list.Add(new Notification
                {
                    UserId = user.UserId,
                    Type = type,
                    Title = title,
                    Message = message,
                    Link = link,
                    CreatedAt = at,
                    ReadAt = readAt
                });

            foreach (var r in result.Reservations)
            {
                var link = $"/reservations/{r.ReservationId}";
                var booked = r.StatusChanges.First(c => c.Status == ReservationStatus.Pending).ChangedAt;

                if ((now - booked).TotalDays <= 30)
                {
                    foreach (var employee in result.Staff[r.Trip.Agency])
                    {
                        Add(employee, NotificationType.ReservationCreated, $"New reservation {r.Code}",
                            $"{r.User.FirstName} {r.User.LastName} booked {r.Trip.TripName}.", link, booked,
                            r.Status == ReservationStatus.Pending ? null : booked.AddHours(3));
                    }
                }

                var decision = r.StatusChanges.FirstOrDefault(c => c.Status == ReservationStatus.Confirmed || c.Status == ReservationStatus.Rejected);
                if (decision != null && (now - decision.ChangedAt).TotalDays <= 60)
                {
                    var confirmed = decision.Status == ReservationStatus.Confirmed;
                    Add(r.User, confirmed ? NotificationType.ReservationConfirmed : NotificationType.ReservationRejected,
                        $"Reservation {r.Code} {(confirmed ? "confirmed" : "rejected")}",
                        confirmed
                            ? $"Your trip {r.Trip.TripName} is confirmed. Have a great journey!"
                            : $"The agency could not confirm your reservation for {r.Trip.TripName}: {decision.Comment}",
                        link, decision.ChangedAt, ReadIfOlderThanTwoDays(decision.ChangedAt));
                }

                var cancellation = r.StatusChanges.FirstOrDefault(c => c.Status == ReservationStatus.Cancelled);
                if (cancellation != null)
                {
                    foreach (var employee in result.Staff[r.Trip.Agency])
                    {
                        Add(employee, NotificationType.ReservationCancelled, $"Reservation {r.Code} cancelled",
                            $"{r.User.FirstName} {r.User.LastName} cancelled the reservation for {r.Trip.TripName}.",
                            link, cancellation.ChangedAt, cancellation.ChangedAt.AddHours(1));
                    }
                }
            }

            foreach (var r in result.Reservations)
            {
                foreach (var message in r.Messages.Where(m => m.ReadAt == null && m.Sender == r.User))
                {
                    foreach (var employee in result.Staff[r.Trip.Agency])
                    {
                        Add(employee, NotificationType.NewMessage, $"Message about {r.Code} from {r.User.FirstName} {r.User.LastName}",
                            message.Text.Length <= 140 ? message.Text : message.Text[..139] + "…",
                            $"/reservations/{r.ReservationId}", message.SentAt, null);
                    }
                }
            }

            foreach (var review in result.Reviews)
            {
                var written = review.CreatedAt.ToUniversalTime();

                if (review.IsActive && (now - written).TotalDays <= 30)
                {
                    foreach (var employee in result.Staff[review.Trip.Agency])
                    {
                        Add(employee, NotificationType.ReviewReceived,
                            $"New review: {review.TripRating}★ for {review.Trip.TripName}",
                            review.Comment.Length <= 140 ? review.Comment : review.Comment[..139] + "…",
                            "/agency?tab=reviews", written, review.AgencyReply == null ? null : written.AddHours(5));
                    }
                }

                if (review.AgencyReply != null)
                {
                    Add(review.User, NotificationType.ReviewReplied, $"{review.Trip.Agency.AgencyName} replied to your review",
                        review.AgencyReply, $"/reservations/{review.Reservation.ReservationId}",
                        review.AgencyReplyAt.Value, ReadIfOlderThanTwoDays(review.AgencyReplyAt.Value));
                }

                if (!review.IsActive)
                {
                    Add(review.User, NotificationType.ReviewHidden, "Your review was hidden",
                        $"Your review of {review.Trip.TripName} was hidden by a moderator: {review.ModerationNote}",
                        $"/reservations/{review.Reservation.ReservationId}", review.ModeratedAt.Value, review.ModeratedAt.Value.AddHours(4));
                }
            }

            return list;
        }

        private static void AddRoomPrices(IEnumerable<Accommodation> accommodations, DateTime today)
        {
            var validFrom = new DateTime(today.Year - 1, 1, 1);

            foreach (var accommodation in accommodations)
            {
                var stars = (int)Math.Round(accommodation.AccommodationStars);
                var basePrice = stars switch { <= 2 => 40m, 3 => 60m, 4 => 90m, _ => 150m };
                var countryFactor = CountryPriceLevel(accommodation.Destination.Country.CountryName);

                foreach (var room in accommodation.Rooms)
                {
                    var typeFactor = room.RoomType.RoomTypeName switch
                    {
                        "Triple Room" => 1.35m,
                        "Quadruple Room" => 1.65m,
                        "Master Bed Suite" => 2.2m,
                        _ => 1m
                    };
                    var price = Math.Round(basePrice * typeFactor * countryFactor / 5m, 0) * 5m;
                    room.Prices.Add(new RoomPrice { Price = price, ValidFrom = validFrom });
                }
            }
        }

        private static decimal CountryPriceLevel(string country) => country switch
        {
            "France" => 1.5m,
            "Greece" => 1.15m,
            "Dominican Republic" => 1.1m,
            "Mexico" => 1.05m,
            "Russia" => 1m,
            "Brazil" => 0.95m,
            "Cuba" => 0.9m,
            "Turkey" => 0.75m,
            "Egypt" => 0.7m,
            _ => 1m
        };
    }
}
