using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WithoutIOC.View;

namespace WithoutIOC.Tests.View;

public class WeatherControllerTests
{
    private const string TestConnectionString = "Server=localhost;Database=Test;";

    [Fact]
    public async Task GetWeatherForecast_WithValidZipCode_ReturnsOkResult()
    {
        // Arrange
        var controller = new WeatherController(TestConnectionString);
        var zipCode = "10001";

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsType<Ok<List<WithoutIOC.Domain.WeatherForecast>>>(result);
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsBadRequest()
    {
        // Arrange
        var controller = new WeatherController(TestConnectionString);
        var zipCode = "99999";

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var badRequestResult = result.GetType().Name;
        Assert.StartsWith("BadRequest", badRequestResult);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetWeatherForecast_WithNullOrEmptyZipCode_ReturnsBadRequest(string zipCode)
    {
        // Arrange
        var controller = new WeatherController(TestConnectionString);

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultTypeName = result.GetType().Name;
        Assert.StartsWith("BadRequest", resultTypeName);
    }

    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsExpectedForecastCount()
    {
        // Arrange
        var controller = new WeatherController(TestConnectionString);
        var zipCode = "90210";

        // Act
        var result = await controller.GetWeatherForecastAsync(zipCode);

        // Assert
        var okResult = Assert.IsType<Ok<List<WithoutIOC.Domain.WeatherForecast>>>(result);
        Assert.Equal(5, okResult.Value?.Count);
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherController(null!));
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherController(""));
    }
}
