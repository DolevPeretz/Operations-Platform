
namespace Operations_Platform_Assignment.Contracts;

public sealed record WeatherData(
    double TemperatureCelsius,
    int HumidityPercent,
    double WindSpeedKmh,
    string Time);
