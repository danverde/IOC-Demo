using Ardalis.GuardClauses;
using WithoutIOC.Application;

namespace WithoutIOC.View;

public class WeatherController
{
    private readonly string _connectionString;
    private readonly string _apiKey;

    public WeatherController(string connectionString, string apiKey)
    {
        _connectionString = Guard.Against.NullOrWhiteSpace(connectionString);
        _apiKey = Guard.Against.NullOrWhiteSpace(apiKey);
    }

    public async Task<IResult> GetWeatherForecastAsync(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Results.BadRequest(new { error = "Required input zipCode was empty" });
        }

        var weatherService = new WeatherService(_connectionString, _apiKey);
        var forecast = await weatherService.GetWeatherForecastAsync(zipCode);

        if (forecast == null)
        {
            return Results.BadRequest(new { error = "Zip code is not supported." });
        }

        return Results.Ok(forecast);
    }
}