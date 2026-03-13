using Moq;
using With_IOC.Application;
using With_IOC.Infrastructure;

namespace WithIOC.Tests.Application;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsForecastList()
    {
        // Arrange
        var mockStore = new Mock<IWeatherStore>();
        var mockAdapter = new Mock<IWeatherAdapter>();
        var zipCode = "10001";

        mockStore.Setup(s => s.IsZipCodeSupported(zipCode)).ReturnsAsync(true);
        mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        var service = new WeatherService(mockStore.Object, mockAdapter.Object);

        // Act
        var result = await service.GetWeatherForecast(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.All(result, forecast => Assert.Equal(zipCode, forecast.ZipCode));
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsNull()
    {
        // Arrange
        var mockStore = new Mock<IWeatherStore>();
        var mockAdapter = new Mock<IWeatherAdapter>();
        var zipCode = "99999";

        mockStore.Setup(s => s.IsZipCodeSupported(zipCode)).ReturnsAsync(false);

        var service = new WeatherService(mockStore.Object, mockAdapter.Object);

        // Act
        var result = await service.GetWeatherForecast(zipCode);

        // Assert
        Assert.Null(result);
        mockAdapter.Verify(a => a.GetWeatherDataAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsForecastsWithCorrectProperties()
    {
        // Arrange
        var mockStore = new Mock<IWeatherStore>();
        var mockAdapter = new Mock<IWeatherAdapter>();
        var zipCode = "90210";

        mockStore.Setup(s => s.IsZipCodeSupported(zipCode)).ReturnsAsync(true);
        mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        var service = new WeatherService(mockStore.Object, mockAdapter.Object);

        // Act
        var result = await service.GetWeatherForecast(zipCode);

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
        var mockStore = new Mock<IWeatherStore>();
        var mockAdapter = new Mock<IWeatherAdapter>();
        var zipCode = "60601";

        mockStore.Setup(s => s.IsZipCodeSupported(zipCode)).ReturnsAsync(true);
        mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        var service = new WeatherService(mockStore.Object, mockAdapter.Object);

        // Act
        var result = await service.GetWeatherForecast(zipCode);

        // Assert
        Assert.NotNull(result);
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i + 1].Date > result[i].Date);
        }
    }

    [Fact]
    public void Constructor_WithNullWeatherStore_ThrowsArgumentNullException()
    {
        // Arrange
        var mockAdapter = new Mock<IWeatherAdapter>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherService(null!, mockAdapter.Object));
    }

    [Fact]
    public void Constructor_WithNullWeatherAdapter_ThrowsArgumentNullException()
    {
        // Arrange
        var mockStore = new Mock<IWeatherStore>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherService(mockStore.Object, null!));
    }
}
