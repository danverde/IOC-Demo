namespace With_IOC.Infrastructure;

public interface IWeatherStore
{
    Task<bool> IsZipCodeSupported(string zipCode);
}
