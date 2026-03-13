using Moq;
using With_IOC.Application;
using With_IOC.Infrastructure;

namespace WithIOC.Tests.Application;

public class WeatherServiceTests
{
    private readonly Mock<IWeatherStore> _mockStore;
    private readonly Mock<IWeatherAdapter> _mockAdapter;
    
    private readonly IWeatherService _service;
    
    public WeatherServiceTests()
    {
        _mockStore = new Mock<IWeatherStore>();
        _mockAdapter = new Mock<IWeatherAdapter>();
        
        _service = new WeatherService(_mockStore.Object, _mockAdapter.Object);
    }
    
    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsForecastList()
    {
        // Arrange
        var zipCode = "10001";

        _mockStore.Setup(s => s.IsZipCodeSupportedAsync(zipCode)).ReturnsAsync(true);
        _mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        // Act
        var result = await _service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.All(result, forecast => Assert.Equal(zipCode, forecast.ZipCode));
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsNull()
    {
        // Arrange
        var zipCode = "99999";

        _mockStore.Setup(s => s.IsZipCodeSupportedAsync(zipCode)).ReturnsAsync(false);

        // Act
        var result = await _service.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.Null(result);
        _mockAdapter.Verify(a => a.GetWeatherDataAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsForecastsWithCorrectProperties()
    {
        // Arrange
        var zipCode = "90210";

        _mockStore.Setup(s => s.IsZipCodeSupportedAsync(zipCode)).ReturnsAsync(true);
        _mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        // Act
        var result = await _service.GetWeatherForecastAsync(zipCode);

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
        var zipCode = "60601";

        _mockStore.Setup(s => s.IsZipCodeSupportedAsync(zipCode)).ReturnsAsync(true);
        _mockAdapter.Setup(a => a.GetWeatherDataAsync(zipCode))
            .ReturnsAsync(new WeatherApiResponse
            {
                ZipCode = zipCode,
                Temperatures = new[] { -15, 2, 10, 18, 25 },
                Conditions = new[] { "Chilly", "Cool", "Mild", "Warm", "Hot" }
            });

        // Act
        var result = await _service.GetWeatherForecastAsync(zipCode);

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
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherService(null!, _mockAdapter.Object));
    }

    [Fact]
    public void Constructor_WithNullWeatherAdapter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherService(_mockStore.Object, null!));
    }
}
