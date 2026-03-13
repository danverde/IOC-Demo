using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using With_IOC.Application;
using With_IOC.Domain;
using With_IOC.View;

namespace WithIOC.Tests.View;

public class WeatherControllerTests
{
    [Fact]
    public async Task GetWeatherForecast_WithValidZipCode_ReturnsOkResult()
    {
        // Arrange
        var mockService = new Mock<IWeatherService>();
        var zipCode = "10001";
        var forecast = new List<WeatherForecast>
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, "Mild", zipCode)
        };

        mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync(forecast);

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsType<Ok<List<WeatherForecast>>>(result);
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<IWeatherService>();
        var zipCode = "99999";

        mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync((List<WeatherForecast>?)null);

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

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
        // Arrange
        var mockService = new Mock<IWeatherService>();
        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultTypeName = result.GetType().Name;
        Assert.StartsWith("BadRequest", resultTypeName);
        mockService.Verify(s => s.GetWeatherForecastAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsExpectedForecastCount()
    {
        // Arrange
        var mockService = new Mock<IWeatherService>();
        var zipCode = "90210";
        var forecast = new List<WeatherForecast>
        {
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 20, "Mild", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(1)), 22, "Warm", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(2)), 24, "Warm", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(3)), 26, "Hot", zipCode),
            new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(4)), 28, "Hot", zipCode)
        };

        mockService.Setup(s => s.GetWeatherForecastAsync(zipCode)).ReturnsAsync(forecast);

        var controller = new WeatherController(mockService.Object);

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

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
