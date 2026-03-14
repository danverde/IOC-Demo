using With_IOC.Application;

namespace With_IOC.View;

public class WeatherController
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
    }

    public async Task<IResult> GetWeatherForecastAsync(string zipCode)
    {
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Results.BadRequest(new { error = "Zip code cannot be null or empty." });
        }

        var forecast = await _weatherService.GetWeatherForecastAsync(zipCode);

        if (forecast == null)
        {
            return Results.BadRequest(new { error = "Zip code is not supported." });
        }

        return Results.Ok(forecast);
    }
}
