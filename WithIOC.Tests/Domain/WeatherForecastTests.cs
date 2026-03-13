using With_IOC.Domain;

namespace WithIOC.Tests.Domain;

public class WeatherForecastTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(10, 49)]
    [InlineData(20, 67)]
    [InlineData(30, 85)]
    [InlineData(-10, 15)]
    public void TemperatureF_ConvertsFromCelsiusCorrectly(int temperatureC, int expectedF)
    {
        // Arrange
        var forecast = new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now),
            temperatureC,
            "Mild",
            "10001"
        );

        // Act
        var result = forecast.TemperatureF;

        // Assert
        Assert.Equal(expectedF, result);
    }

    [Fact]
    public void WeatherForecast_InitializesPropertiesCorrectly()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Now);
        var tempC = 25;
        var summary = "Warm";
        var zipCode = "90210";

        // Act
        var forecast = new WeatherForecast(date, tempC, summary, zipCode);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(tempC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
        Assert.Equal(zipCode, forecast.ZipCode);
    }
}
