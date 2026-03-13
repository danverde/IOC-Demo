namespace With_IOC.Infrastructure;

public interface IWeatherStore
{
    Task<bool> IsZipCodeSupportedAsync(string zipCode);
}
