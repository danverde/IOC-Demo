using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WithoutIOC.View;
using Xunit.Abstractions;

namespace WithoutIOC.Tests.View;

public class WeatherControllerTests
{
    private const string TestConnectionString = "Server=localhost;Database=Test;";
    private const string TestApiKey = "asdf123";

    private readonly WeatherController _controller;
    
    public WeatherControllerTests()
    {
        _controller = new WeatherController(TestConnectionString, TestApiKey);
    }
    
    [Fact]
    public async Task GetWeatherForecast_WithValidZipCode_ReturnsOkResult()
    {
        // Arrange
        var zipCode = "10001";

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsType<Ok<List<WithoutIOC.Domain.WeatherForecast>>>(result);
    }

    [Fact]
    public async Task GetWeatherForecast_WithUnsupportedZipCode_ReturnsBadRequest()
    {
        // Arrange
        var zipCode = "99999";

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

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

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultTypeName = result.GetType().Name;
        Assert.StartsWith("BadRequest", resultTypeName);
    }

    [Fact]
    public async Task GetWeatherForecast_WithSupportedZipCode_ReturnsExpectedForecastCount()
    {
        // Arrange
        var zipCode = "90210";

        // Act
        var result = await _controller.GetWeatherForecastAsync(zipCode);

        // Assert
        var okResult = Assert.IsType<Ok<List<WithoutIOC.Domain.WeatherForecast>>>(result);
        Assert.Equal(5, okResult.Value?.Count);
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherController(null!, TestApiKey));
    }
    
    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new WeatherController(TestConnectionString, null!));
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherController("", TestApiKey));
    }
    
    [Fact]
    public void Constructor_WithEmptyApiKey_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherController(TestConnectionString, ""));
    }
}
