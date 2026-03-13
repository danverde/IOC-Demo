namespace WithoutIOC.Infrastructure;

public class WeatherStore
{
    private readonly string _connectionString;
    private readonly List<string> _supportedZipCodes = new()
    {
        "10001", "10002", "90210", "60601", "94102",
        "02101", "75201", "33101", "98101", "85001"
    };

    public WeatherStore(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<bool> IsZipCodeSupported(string zipCode)
    {
        // Simulate database call delay
        await Task.Delay(50);

        return _supportedZipCodes.Contains(zipCode);
    }
}