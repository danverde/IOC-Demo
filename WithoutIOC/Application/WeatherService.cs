using WithoutIOC.Domain;
using WithoutIOC.Infrastructure;

namespace WithoutIOC.Application;

public class WeatherService
{
    private readonly string _connectionString;

    public WeatherService(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<List<WeatherForecast>?> GetWeatherForecast(string zipCode)
    {
        var weatherStore = new WeatherStore(_connectionString);
        bool isSupported = await weatherStore.IsZipCodeSupported(zipCode);

        if (!isSupported)
        {
            return null;
        }

        var weatherAdapter = new WeatherAdapter();
        var apiResponse = await weatherAdapter.GetWeatherDataAsync(zipCode);

        List<WeatherForecast> forecast = MapToWeatherForecast(apiResponse);

        return forecast;
    }

    private List<WeatherForecast> MapToWeatherForecast(WeatherApiResponse apiResponse)
    {
        return apiResponse.Temperatures
            .Select((temp, index) => new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index + 1)),
                temp,
                apiResponse.Conditions[index],
                apiResponse.ZipCode
            ))
            .ToList();
    }
}