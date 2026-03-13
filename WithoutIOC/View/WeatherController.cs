using WithoutIOC.Application;

namespace WithoutIOC.View;

public class WeatherController
{
    private readonly string _connectionString;

    public WeatherController(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<IResult> GetWeatherForecastAsync(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Results.BadRequest(new { error = "Zip code cannot be null or empty." });
        }

        var weatherService = new WeatherService(_connectionString);
        var forecast = await weatherService.GetWeatherForecastAsync(zipCode);

        if (forecast == null)
        {
            return Results.BadRequest(new { error = "Zip code is not supported." });
        }

        return Results.Ok(forecast);
    }
}