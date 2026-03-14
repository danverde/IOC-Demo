using Ardalis.GuardClauses;
using WithoutIOC.Domain;
using WithoutIOC.Infrastructure;

namespace WithoutIOC.Application;

public class WeatherService
{
    private readonly string _connectionString;
    private readonly string _apiKey;

    public WeatherService(string connectionString, string apiKey)
    {
        _connectionString = Guard.Against.NullOrEmpty(connectionString);
        _apiKey = Guard.Against.NullOrEmpty(apiKey);
    }

    public async Task<List<WeatherForecast>?> GetWeatherForecastAsync(string zipCode)
    {
        var weatherStore = new WeatherStore(_connectionString);
        bool isSupported = await weatherStore.IsZipCodeSupportedAsync(zipCode);

        if (!isSupported)
        {
            return null;
        }

        var weatherAdapter = new WeatherAdapter(_apiKey);
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