using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using With_IOC.Application;
using With_IOC.Domain;
using With_IOC.View;

namespace WithIOC.Tests.View;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _mockService;
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _mockService = new Mock<IWeatherService>();
        _controller = new WeatherController(_mockService.Object);
    }
    
    [Fact]
    public async Task GetWeatherForecast_WithValidZipCode_ReturnsOkResult()
    {
        // Arrange
        var zipCode = "10001";
        var forecast = new List<WeatherForecast>
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, "Mild", zipCode)
        };

        _mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync(forecast);

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsType<Ok<List<WeatherForecast>>>(result);
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsBadRequest()
    {
        // Arrange
        var zipCode = "99999";

        _mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync((List<WeatherForecast>?)null);

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultTypeName = result.GetType().Name;
        Assert.StartsWith("BadRequest", resultTypeName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetWeatherForecast_WithNullOrEmptyZipCode_ReturnsBadRequest(string zipCode)
    {
        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultTypeName = result.GetType().Name;
        Assert.StartsWith("BadRequest", resultTypeName);
        _mockService.Verify(s => s.GetWeatherForecastAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsExpectedForecastCount()
    {
        // Arrange
        var zipCode = "90210";
        var forecast = new List<WeatherForecast>
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, "Mild", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 22, "Warm", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(2)), 24, "Warm", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 26, "Hot", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(4)), 28, "Hot", zipCode)
        };

        _mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync(forecast);

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        var okResult = Assert.IsType<Ok<List<WeatherForecast>>>(result);
        Assert.Equal(5, okResult.Value?.Count);
    }

    [Fact]
    public void Constructor_WithNullWeatherService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherController(null!));
    }
}
