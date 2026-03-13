using Ardalis.GuardClauses;

namespace WithoutIOC.Infrastructure;

public class WeatherAdapter
{
    private string _apiKey;
    public WeatherAdapter(string apiKey)
    {
        _apiKey = Guard.Against.NullOrEmpty(apiKey);
    }
    
    public async Task<WeatherApiResponse> GetWeatherDataAsync(string zipCode)
    {
        // Simulate API call delay
        await Task.Delay(100);

        // Mock API response
        var response = new WeatherApiResponse
        {
            ZipCode = zipCode,
            Temperatures = new[] { -15, 2, 10, 18, 25 },
            Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
        };

        return response;
    }
}

public class WeatherApiResponse
{
    public string ZipCode { get; set; } = string.Empty;
    public int[] Temperatures { get; set; } = Array.Empty<int>();
    public string[] Conditions { get; set; } = Array.Empty<string>();
}
