using FlexTravel.Domain;

namespace FlexTravel.Implementation.UseCases.Commands.SampleData
{
    public static class DestinationCoordinates
    {
        private static readonly Dictionary<string, (double Lat, double Lng)> Positions = new()
        {
            ["Havana"] = (23.1136, -82.3666),
            ["Varadero"] = (23.1544, -81.2448),
            ["Trinidad"] = (21.8019, -79.9847),
            ["Punta Cana"] = (18.5601, -68.3725),
            ["Cairo"] = (30.0444, 31.2357),
            ["Santo Domingo"] = (18.4861, -69.9312),
            ["Saona Island"] = (18.1550, -68.6870),
            ["Luxor"] = (25.6872, 32.6396),
            ["Sharm El Sheikh"] = (27.9158, 34.3300),
            ["Paris"] = (48.8566, 2.3522),
            ["Nice"] = (43.7102, 7.2620),
            ["Avignon"] = (43.9493, 4.8055),
            ["Istanbul"] = (41.0082, 28.9784),
            ["Cappadocia"] = (38.6431, 34.8289),
            ["Antalya"] = (36.8969, 30.7133),
            ["Rio de Janeiro"] = (-22.9068, -43.1729),
            ["Florianópolis"] = (-27.5954, -48.5480),
            ["Salvador"] = (-12.9777, -38.5016),
            ["Athens"] = (37.9838, 23.7275),
            ["Santorini"] = (36.3932, 25.4615),
            ["Crete"] = (35.2401, 24.8093),
            ["Mexico City"] = (19.4326, -99.1332),
            ["Tulum"] = (20.2114, -87.4654),
            ["Cancun"] = (21.1619, -86.8515),
            ["Saint Petersburg"] = (59.9311, 30.3609),
            ["Moscow"] = (55.7558, 37.6173),
            ["Lake Baikal"] = (53.5587, 108.1650),
            ["New Delhi"] = (28.6139, 77.2090),
            ["Agra"] = (27.1767, 78.0081),
            ["Varanasi"] = (25.3176, 82.9739),
            ["Lima"] = (-12.0464, -77.0428),
            ["Machu Picchu"] = (-13.1631, -72.5450),
            ["Cusco"] = (-13.5320, -71.9675),
            ["Seoul"] = (37.5665, 126.9780),
            ["Jeju Island"] = (33.4996, 126.5312),
            ["Busan"] = (35.1796, 129.0756),
            ["Marrakech"] = (31.6295, -7.9811),
            ["Fes"] = (34.0181, -5.0078),
            ["Casablanca"] = (33.5731, -7.5898),
            ["Buenos Aires"] = (-34.6037, -58.3816),
            ["Mendoza"] = (-32.8895, -68.8458),
            ["Iguazu Falls"] = (-25.6953, -54.4367),
            ["Kilimanjaro"] = (-3.0674, 37.3556),
            ["Zanzibar"] = (-6.1659, 39.2026),
            ["Serengeti National Park"] = (-2.3333, 34.8333),
            ["Sydney"] = (-33.8688, 151.2093),
            ["Melbourne"] = (-37.8136, 144.9631),
            ["Tasmania"] = (-42.0409, 146.8087),
            ["Antarctica"] = (-77.8460, 166.6760),
            ["Barcelona"] = (41.3874, 2.1686),
            ["Madrid"] = (40.4168, -3.7038),
            ["Seville"] = (37.3891, -5.9845),
            ["Rome"] = (41.9028, 12.4964),
            ["Venice"] = (45.4408, 12.3155),
            ["Florence"] = (43.7696, 11.2558),
            ["Lucerne"] = (47.0502, 8.3093),
            ["Zermatt"] = (46.0207, 7.7491),
            ["Interlaken"] = (46.6863, 7.8632)
        };

        public static void Apply(IEnumerable<Destination> destinations)
        {
            foreach (var destination in destinations)
            {
                if (Positions.TryGetValue(destination.DestinationName, out var position))
                {
                    destination.Latitude = position.Lat;
                    destination.Longitude = position.Lng;
                }
            }
        }
    }
}
