using FlexTravel.Application.Dto;
using FlexTravel.Application.Weather;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text.Json;

namespace FlexTravel.Api.Core
{
    public class OpenMeteoWeatherProvider : IWeatherProvider
    {
        public const string ClientName = "open-meteo";
        private const string Daily = "weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum";

        private readonly IHttpClientFactory _clients;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OpenMeteoWeatherProvider> _logger;

        public OpenMeteoWeatherProvider(IHttpClientFactory clients, IMemoryCache cache, ILogger<OpenMeteoWeatherProvider> logger)
        {
            _clients = clients;
            _cache = cache;
            _logger = logger;
        }

        public int ForecastDays => 16;

        public IReadOnlyList<WeatherDayDto> Forecast(double latitude, double longitude, DateTime from, DateTime to) =>
            Get("https://api.open-meteo.com/v1/forecast", latitude, longitude, from, to, TimeSpan.FromHours(1));

        public IReadOnlyList<WeatherDayDto> History(double latitude, double longitude, DateTime from, DateTime to) =>
            Get("https://archive-api.open-meteo.com/v1/archive", latitude, longitude, from, to, TimeSpan.FromDays(7));

        private IReadOnlyList<WeatherDayDto> Get(string endpoint, double latitude, double longitude, DateTime from, DateTime to, TimeSpan keep)
        {
            var url = string.Create(CultureInfo.InvariantCulture,
                $"{endpoint}?latitude={latitude:0.####}&longitude={longitude:0.####}&daily={Daily}&timezone=auto"
                + $"&start_date={from:yyyy-MM-dd}&end_date={to:yyyy-MM-dd}");

            if (_cache.TryGetValue(url, out IReadOnlyList<WeatherDayDto> cached))
            {
                return Copy(cached);
            }

            IReadOnlyList<WeatherDayDto> days;
            try
            {
                var json = _clients.CreateClient(ClientName).GetStringAsync(url).GetAwaiter().GetResult();
                days = Parse(json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Open-Meteo request failed: {Url}", url);
                _cache.Set(url, (IReadOnlyList<WeatherDayDto>)null, TimeSpan.FromMinutes(3));
                return null;
            }

            _cache.Set(url, days, keep);
            return Copy(days);
        }

        private static IReadOnlyList<WeatherDayDto> Copy(IReadOnlyList<WeatherDayDto> days) => days?
            .Select(d => new WeatherDayDto { Date = d.Date, High = d.High, Low = d.Low, Precipitation = d.Precipitation, Code = d.Code })
            .ToList();

        private static IReadOnlyList<WeatherDayDto> Parse(string json)
        {
            using var document = JsonDocument.Parse(json);
            var daily = document.RootElement.GetProperty("daily");
            var dates = daily.GetProperty("time").EnumerateArray().Select(x => x.GetString()).ToList();

            double? Number(string name, int i)
            {
                var value = daily.GetProperty(name)[i];
                return value.ValueKind == JsonValueKind.Number ? Math.Round(value.GetDouble(), 1) : null;
            }

            return dates
                .Select((date, i) => new WeatherDayDto
                {
                    Date = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    High = Number("temperature_2m_max", i),
                    Low = Number("temperature_2m_min", i),
                    Precipitation = Number("precipitation_sum", i),
                    Code = Number("weather_code", i) is double code ? (int)code : null
                })
                .Where(d => d.High.HasValue || d.Low.HasValue)
                .ToList();
        }
    }
}
