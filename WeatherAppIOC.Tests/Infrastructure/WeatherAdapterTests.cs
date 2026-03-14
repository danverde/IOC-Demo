using With_IOC.Infrastructure;

namespace WithIOC.Tests.Infrastructure;

public class WeatherAdapterTests
{
    [Fact]
    public async Task GetWeatherDataAsync_ReturnsWeatherApiResponse()
    {
        // Arrange
        var adapter = new WeatherAdapter();
        var zipCode = "10001";

        // Act
        var result = await adapter.GetWeatherDataAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(zipCode, result.ZipCode);
        Assert.NotNull(result.Temperatures);
        Assert.NotNull(result.Conditions);
        Assert.Equal(5, result.Temperatures.Length);
        Assert.Equal(5, result.Conditions.Length);
    }

    [Fact]
    public async Task GetWeatherDataAsync_ReturnsExpectedTemperatureRange()
    {
        // Arrange
        var adapter = new WeatherAdapter();
        var zipCode = "90210";

        // Act
        var result = await adapter.GetWeatherDataAsync(zipCode);

        // Assert
        Assert.All(result.Temperatures, temp => Assert.InRange(temp, -20, 30));
    }

    [Fact]
    public async Task GetWeatherDataAsync_ReturnsExpectedConditions()
    {
        // Arrange
        var adapter = new WeatherAdapter();
        var zipCode = "60601";

        // Act
        var result = await adapter.GetWeatherDataAsync(zipCode);

        // Assert
        var expectedConditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" };
        Assert.Equal(expectedConditions, result.Conditions);
    }
}
