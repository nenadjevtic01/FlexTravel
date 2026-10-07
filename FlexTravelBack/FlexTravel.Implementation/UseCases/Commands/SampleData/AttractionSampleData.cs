using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Commands.SampleData
{
    public static class AttractionSampleData
    {
        public class Result
        {
            public List<AttractionCategory> Categories { get; } = new List<AttractionCategory>();
            public List<Attraction> Attractions { get; } = new List<Attraction>();
        }

        private record Sight(string Destination, string Name, string Category, string Description, string Keyword);

        private static readonly Sight[] Sights =
        {
            new("Havana", "Old Havana", "Historic site", "UNESCO-listed colonial quarter with Plaza de Armas, Plaza de la Catedral and Plaza Vieja.", "Old Havana"),
            new("Havana", "Malecón", "Landmark", "Eight-kilometre seafront promenade, the city's favourite place for an evening walk.", "Malecón"),
            new("Havana", "Revolution Square", "Landmark", "Vast square with the José Martí Memorial and the famous Che Guevara mural.", "Revolution Square"),
            new("Havana", "Viñales Valley", "Nature", "Limestone hills and tobacco farms, a UNESCO cultural landscape west of Havana.", "Viñales"),
            new("Varadero", "Varadero Beach", "Beach", "More than 20 km of white sand and shallow turquoise water on the Hicacos Peninsula.", "beach"),
            new("Varadero", "Cayo Blanco reef", "Nature", "Catamaran trips to the reefs off Cayo Blanco, with colourful fish and coral.", "catamaran"),
            new("Varadero", "Saturno Cave", "Nature", "Freshwater pool in a limestone cave, a favourite spot for a refreshing swim.", "Saturno"),
            new("Trinidad", "Plaza Mayor", "Historic site", "The colonial heart of Trinidad, surrounded by pastel mansions and the Holy Trinity church.", "Plaza Mayor"),
            new("Trinidad", "Valley of the Sugar Mills", "Historic site", "Valle de los Ingenios, with ruins of sugar plantations and the Manaca Iznaga tower.", "Sugar Mills"),
            new("Trinidad", "Playa Ancón", "Beach", "The best beach on the south coast of Cuba, a short drive from the town.", "Playa Ancón"),

            new("Punta Cana", "Bávaro Beach", "Beach", "Palm-lined beach with fine white sand along the resort strip.", "beach"),
            new("Punta Cana", "Saona Island", "Nature", "Island in the Cotubanamá National Park, reached by catamaran and famous for its natural pool.", "Saona"),
            new("Punta Cana", "Hoyo Azul", "Nature", "Turquoise cenote at the foot of a limestone cliff.", "cenote"),
            new("Santo Domingo", "Colonial Zone", "Historic site", "The first European city in the Americas: cobbled streets and early colonial buildings, a UNESCO site.", "Colonial Zone"),
            new("Santo Domingo", "Alcázar de Colón", "Museum", "Palace of Diego Columbus, son of Christopher Columbus, with period furniture and art.", "Alcázar"),
            new("Santo Domingo", "Cathedral of Santa María la Menor", "Religious site", "The oldest cathedral in the Americas, completed in 1541.", "Cathedral"),
            new("Santo Domingo", "Los Tres Ojos", "Nature", "Limestone caves with three blue lakes in the middle of the city.", "Tres Ojos"),
            new("Cancun", "Chichén Itzá", "Historic site", "Maya city with the El Castillo pyramid, one of the New Seven Wonders of the World.", "Chichén Itzá"),
            new("Cancun", "Isla Mujeres", "Beach", "Small island across the bay with Playa Norte and calm, clear water.", "Isla Mujeres"),
            new("Cancun", "Hotel Zone beaches", "Beach", "A 22 km strip of beaches between the Caribbean Sea and the Nichupté lagoon.", "hotel zone"),
            new("Mexico City", "Zócalo and Templo Mayor", "Historic site", "The main square with the Metropolitan Cathedral and the ruins of the Aztec main temple.", "Templo Mayor"),
            new("Mexico City", "Teotihuacan", "Historic site", "Ancient city with the Pyramids of the Sun and the Moon, 50 km from the capital.", "Teotihuacan"),
            new("Mexico City", "Frida Kahlo Museum", "Museum", "The Blue House in Coyoacán, where the painter was born and lived.", "Frida Kahlo"),
            new("Mexico City", "National Museum of Anthropology", "Museum", "The largest museum in Mexico, home of the Aztec Sun Stone.", "Anthropology"),
            new("Mexico City", "Xochimilco", "Culture", "Canals with colourful trajinera boats and floating mariachi bands.", "Xochimilco"),
            new("Rio de Janeiro", "Christ the Redeemer", "Landmark", "The 30-metre statue on Corcovado mountain, overlooking the whole city.", "Christ the Redeemer"),
            new("Rio de Janeiro", "Sugarloaf Mountain", "Nature", "Granite peak reached by cable car, with views over Guanabara Bay.", "Sugarloaf"),
            new("Rio de Janeiro", "Copacabana Beach", "Beach", "The most famous beach of Rio, with its wave-patterned promenade.", "Copacabana"),
            new("Rio de Janeiro", "Selarón Steps", "Landmark", "A stairway covered in more than 2,000 colourful tiles.", "Selarón"),

            new("Cairo", "Pyramids of Giza", "Historic site", "The last surviving wonder of the ancient world, guarded by the Great Sphinx.", "Pyramids of Giza"),
            new("Cairo", "Egyptian Museum", "Museum", "Treasures of the pharaohs, including objects from the tomb of Tutankhamun.", "Egyptian Museum"),
            new("Cairo", "Khan el-Khalili", "Market", "Medieval bazaar of spices, lamps and jewellery in Islamic Cairo.", "Khan el-Khalili"),
            new("Cairo", "Saqqara", "Historic site", "Necropolis with the Step Pyramid of Djoser, the oldest pyramid in Egypt.", "Saqqara"),
            new("Luxor", "Karnak Temple", "Religious site", "Huge temple complex with the Great Hypostyle Hall of 134 columns.", "Karnak"),
            new("Luxor", "Valley of the Kings", "Historic site", "Royal tombs of the New Kingdom cut into the hills of the West Bank.", "Valley of the Kings"),
            new("Luxor", "Temple of Hatshepsut", "Historic site", "Terraced mortuary temple of the female pharaoh at Deir el-Bahari.", "Hatshepsut"),
            new("Sharm El Sheikh", "Ras Mohammed National Park", "Nature", "Marine park at the tip of Sinai with some of the best coral reefs in the world.", "Ras Mohammed"),
            new("Sharm El Sheikh", "Mount Sinai", "Nature", "A night climb to the summit to watch the sunrise over the mountains.", "Mount Sinai"),
            new("Sharm El Sheikh", "Naama Bay", "Beach", "Lively bay with a sandy beach, restaurants and dive centres.", "beach"),

            new("Paris", "Eiffel Tower", "Landmark", "The iron symbol of Paris, 330 m tall, with views from three levels.", "Eiffel"),
            new("Paris", "Louvre Museum", "Museum", "The most visited museum in the world, home of the Mona Lisa.", "Louvre"),
            new("Paris", "Montmartre and Sacré-Cœur", "Religious site", "Hilltop quarter of artists, crowned by the white basilica.", "Montmartre"),
            new("Paris", "Palace of Versailles", "Historic site", "Royal palace with the Hall of Mirrors and vast formal gardens.", "Versailles"),
            new("Paris", "Seine river cruise", "Landmark", "Boat ride past Notre-Dame, the Louvre and the Eiffel Tower.", "Seine"),
            new("Nice", "Promenade des Anglais", "Landmark", "Seafront promenade along the Baie des Anges.", "Promenade"),
            new("Nice", "Old Nice", "Historic site", "Narrow lanes of Vieux Nice, the Cours Saleya flower market and Castle Hill.", "Old Town"),
            new("Nice", "Monaco", "Landmark", "The tiny principality with the Monte Carlo Casino and the Prince's Palace.", "Monaco"),
            new("Nice", "Èze", "Historic site", "Medieval hilltop village with an exotic garden high above the sea.", "Èze"),
            new("Avignon", "Palace of the Popes", "Historic site", "The largest Gothic palace in the world, seat of the popes in the 14th century.", "Palace of the Popes"),
            new("Avignon", "Pont d'Avignon", "Landmark", "The famous half bridge over the Rhône.", "Pont d'Avignon"),
            new("Avignon", "Sénanque Abbey", "Religious site", "Cistercian abbey surrounded by lavender fields.", "Sénanque"),
            new("Avignon", "Pont du Gard", "Historic site", "Three-tiered Roman aqueduct, a UNESCO World Heritage site.", "Pont du Gard"),

            new("Istanbul", "Hagia Sophia", "Religious site", "Sixth-century church, later a mosque, with a vast floating dome.", "Hagia Sophia"),
            new("Istanbul", "Blue Mosque", "Religious site", "The Sultan Ahmed Mosque with six minarets and blue Iznik tiles.", "Blue Mosque"),
            new("Istanbul", "Basilica Cistern", "Historic site", "Underground Byzantine water reservoir with 336 columns.", "Cistern"),
            new("Istanbul", "Topkapı Palace", "Museum", "Palace of the Ottoman sultans with the Treasury and the Harem.", "Topkapı"),
            new("Istanbul", "Grand Bazaar", "Market", "One of the oldest and largest covered markets in the world.", "Grand Bazaar"),
            new("Istanbul", "Bosphorus cruise", "Landmark", "Boat trip between Europe and Asia, past palaces and fortresses.", "Bosphorus"),
            new("Cappadocia", "Göreme Open-Air Museum", "Museum", "Rock-cut churches with Byzantine frescoes.", "Göreme Open-Air"),
            new("Cappadocia", "Hot-air balloon flight", "Nature", "A sunrise flight over fairy chimneys and valleys.", "balloon"),
            new("Cappadocia", "Derinkuyu Underground City", "Historic site", "An eight-level underground city that sheltered thousands of people.", "Derinkuyu"),
            new("Cappadocia", "Rose Valley", "Nature", "Hiking trail through pink rock formations, best at sunset.", "Rose"),
            new("Antalya", "Kaleiçi", "Historic site", "The old town of Antalya with Ottoman houses and the Roman Hadrian's Gate.", "Kaleiçi"),
            new("Antalya", "Düden Waterfalls", "Nature", "Waterfalls that drop straight into the Mediterranean.", "Düden"),
            new("Antalya", "Aspendos Theatre", "Historic site", "The best-preserved Roman theatre in the world, still used for concerts.", "Aspendos"),
            new("Antalya", "Perge", "Historic site", "Ruins of an ancient Greek city with a stadium and a colonnaded street.", "Perge"),
            new("Antalya", "Konyaaltı Beach", "Beach", "Long pebble beach with views of the Taurus Mountains.", "beach"),

            new("Athens", "Acropolis and the Parthenon", "Historic site", "The citadel of ancient Athens with the temple of Athena.", "Acropolis"),
            new("Athens", "Acropolis Museum", "Museum", "Modern museum with the sculptures of the Acropolis.", "Acropolis Museum"),
            new("Athens", "Plaka", "Historic site", "The oldest neighbourhood of Athens, full of tavernas and small shops.", "Plaka"),
            new("Athens", "Temple of Poseidon", "Historic site", "Marble temple on Cape Sounion, famous for its sunsets.", "Poseidon"),
            new("Santorini", "Oia", "Landmark", "Village of white houses and blue domes, famous for its sunset.", "Oia"),
            new("Santorini", "Caldera path", "Nature", "Cliff-top trail from Fira to Imerovigli above the flooded volcano.", "caldera"),
            new("Santorini", "Akrotiri", "Historic site", "Bronze Age town buried by a volcanic eruption, often called the Minoan Pompeii.", "Akrotiri"),
            new("Santorini", "Red Beach", "Beach", "A small beach under red volcanic cliffs.", "Red Beach"),
            new("Santorini", "Nea Kameni volcano", "Nature", "Volcanic island in the middle of the caldera, reached by boat.", "volcano"),
            new("Crete", "Palace of Knossos", "Historic site", "Centre of the Minoan civilisation and of the legend of the Minotaur.", "Knossos"),
            new("Crete", "Spinalonga", "Historic site", "Venetian fortress island and former leper colony.", "Spinalonga"),
            new("Crete", "Elafonisi Beach", "Beach", "Lagoon beach with pink sand on the south-west coast.", "Elafonisi"),

            new("Saint Petersburg", "Hermitage Museum", "Museum", "One of the largest art museums in the world, in the Winter Palace.", "Hermitage"),
            new("Saint Petersburg", "Peterhof", "Historic site", "Palace of Peter the Great with spectacular fountains.", "Peterhof"),
            new("Saint Petersburg", "Peter and Paul Fortress", "Historic site", "The original citadel of the city, with the tombs of the tsars.", "Peter and Paul"),
            new("Moscow", "Red Square and the Kremlin", "Historic site", "The heart of Moscow, with St. Basil's Cathedral and the Kremlin walls.", "Kremlin"),
            new("Moscow", "Tretyakov Gallery", "Museum", "The main collection of Russian art.", "Tretyakov"),
            new("Moscow", "Moscow Metro", "Landmark", "Underground stations decorated like palaces.", "Metro"),
            new("Moscow", "Sergiev Posad", "Religious site", "The Trinity Lavra of St. Sergius, the spiritual centre of Russian Orthodoxy.", "Sergiev Posad")
        };

        public static Result Create(IReadOnlyCollection<Destination> destinations, IEnumerable<Trip> trips)
        {
            var result = new Result();
            var categories = new Dictionary<string, AttractionCategory>();
            var keywords = new Dictionary<Attraction, string>();

            foreach (var sight in Sights)
            {
                if (!categories.TryGetValue(sight.Category, out var category))
                {
                    category = new AttractionCategory { AttractionCategoryName = sight.Category };
                    categories[sight.Category] = category;
                    result.Categories.Add(category);
                }

                var attraction = new Attraction
                {
                    AttractionName = sight.Name,
                    AttractionDescription = sight.Description,
                    AttractionCategory = category,
                    Destination = destinations.Single(d => d.DestinationName == sight.Destination)
                };

                keywords[attraction] = sight.Keyword;
                result.Attractions.Add(attraction);
            }

            foreach (var trip in trips)
            {
                var program = string.Join(" ", trip.Itinerary.ItineraryActivities.Select(a => a.Activity));

                foreach (var attraction in result.Attractions.Where(a => a.Destination == trip.Destination))
                {
                    if (program.Contains(keywords[attraction], StringComparison.OrdinalIgnoreCase))
                    {
                        trip.Attractions.Add(new TripAttraction { Attraction = attraction });
                    }
                }
            }

            return result;
        }
    }
}
