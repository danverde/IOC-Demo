using With_IOC.Domain;

namespace With_IOC.Application;

public interface IWeatherService
{
    Task<List<WeatherForecast>?> GetWeatherForecast(string zipCode);
}
