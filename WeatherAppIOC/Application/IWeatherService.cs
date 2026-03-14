using With_IOC.Domain;

namespace With_IOC.Application;

public interface IWeatherService
{
    Task<List<WeatherForecast>?> GetWeatherForecastAsync(string zipCode);
}
