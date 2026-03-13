using WithoutIOC.Application;
using WithoutIOC.Domain;

namespace WithoutIOC.Tests.Application;

public class WeatherServiceTests
{
    private const string TestConnectionString = "Server=localhost;Database=Test;";

    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsForecastList()
    {
        // Arrange
        var service = new WeatherService(TestConnectionString);
        var zipCode = "10001";

        // Act
        var result = await service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.All(result, forecast => Assert.Equal(zipCode, forecast.ZipCode));
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsNull()
    {
        // Arrange
        var service = new WeatherService(TestConnectionString);
        var zipCode = "99999";

        // Act
        var result = await service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsForecastsWithCorrectProperties()
    {
        // Arrange
        var service = new WeatherService(TestConnectionString);
        var zipCode = "90210";

        // Act
        var result = await service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.All(result, forecast =>
        {
            Assert.NotEqual(default(DateOnly), forecast.Date);
            Assert.NotNull(forecast.Summary);
            Assert.Equal(zipCode, forecast.ZipCode);
            Assert.InRange(forecast.TemperatureC, -20, 30);
        });
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsForecastsWithSequentialDates()
    {
        // Arrange
        var service = new WeatherService(TestConnectionString);
        var zipCode = "60601";

        // Act
        var result = await service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i + 1].Date > result[i].Date);
        }
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherService(null!));
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherService(""));
    }
}
