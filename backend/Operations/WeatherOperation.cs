
using System.Text.Json;
using Operations_Platform_Assignment.Contracts;
using Operations_Platform_Assignment.Services;

namespace Operations_Platform_Assignment.Operations;

public sealed class WeatherOperation : IOperation
{
    private readonly WeatherService _weatherService;

    private static readonly IReadOnlyList<OperationParameter>
        _parameters = Array.AsReadOnly(new[]
        {
            new OperationParameter
            {
                Name = "latitude",
                Label = "Latitude",
                Type = "number",
                Required = true
            },
            new OperationParameter
            {
                Name = "longitude",
                Label = "Longitude",
                Type = "number",
                Required = true
            }
        });

    public WeatherOperation(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public string Id => "weather";

    public string Name => "Current Weather";

    public IReadOnlyList<OperationParameter> Parameters
        => _parameters;

    public async Task<OperationResult> ExecuteAsync(
        JsonElement parameters,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        double latitude = parameters
            .GetProperty("latitude").GetDouble();

        double longitude = parameters
            .GetProperty("longitude").GetDouble();

        if (latitude < -90 || latitude > 90)
        {
            throw new OperationValidationException(
                new Dictionary<string, string[]>
                {
                    ["latitude"] =
                        ["Latitude must be between -90 and 90."]
                });
        }

        if (longitude < -180 || longitude > 180)
        {
            throw new OperationValidationException(
                new Dictionary<string, string[]>
                {
                    ["longitude"] =
                        ["Longitude must be between -180 and 180."]
                });
        }

        var weather = await _weatherService.GetWeatherAsync(
            latitude,
            longitude,
            cancellationToken);

        return new OperationResult(weather);
    }
}
