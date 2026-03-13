using With_IOC.Domain;
using With_IOC.Infrastructure;

namespace With_IOC.Application;

public class WeatherService : IWeatherService
{
    private readonly IWeatherStore _weatherStore;
    private readonly IWeatherAdapter _weatherAdapter;

    public WeatherService(IWeatherStore weatherStore, IWeatherAdapter weatherAdapter)
    {
        _weatherStore = weatherStore ?? throw new ArgumentNullException(nameof(weatherStore));
        _weatherAdapter = weatherAdapter ?? throw new ArgumentNullException(nameof(weatherAdapter));
    }

    public async Task<List<WeatherForecast>?> GetWeatherForecastAsync(string zipCode)
    {
        bool isSupported = await _weatherStore.IsZipCodeSupportedAsync(zipCode);

        if (!isSupported)
        {
            return null;
        }

        var apiResponse = await _weatherAdapter.GetWeatherDataAsync(zipCode);

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
