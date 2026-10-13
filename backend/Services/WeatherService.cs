
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Operations_Platform_Assignment.Contracts;

namespace Operations_Platform_Assignment.Services;

public sealed class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetWeatherAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var lat = latitude.ToString(
            CultureInfo.InvariantCulture);

        var lon = longitude.ToString(
            CultureInfo.InvariantCulture);

        var url = $"v1/forecast?latitude={lat}" +
                  $"&longitude={lon}" +
                  "&current=temperature_2m," +
                  "relative_humidity_2m,wind_speed_10m";

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var weather = await response.Content
            .ReadFromJsonAsync<OpenMeteoResponse>(
                cancellationToken: cancellationToken);

        if (weather?.Current is null)
        {
            throw new InvalidOperationException(
                "Weather data is unavailable.");
        }

        return new WeatherData(
            weather.Current.Temperature,
            weather.Current.Humidity,
            weather.Current.WindSpeed,
            weather.Current.Time);
    }

    private sealed class OpenMeteoResponse
    {
        [JsonPropertyName("current")]
        public CurrentWeather? Current { get; set; }
    }

    private sealed class CurrentWeather
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int Humidity { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; } = "";
    }
}
