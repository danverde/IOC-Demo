namespace With_IOC.Infrastructure;

public interface IWeatherAdapter
{
    Task<WeatherApiResponse> GetWeatherDataAsync(string zipCode);
}
