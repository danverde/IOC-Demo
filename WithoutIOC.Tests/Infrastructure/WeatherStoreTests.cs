using WithoutIOC.Infrastructure;

namespace WithoutIOC.Tests.Infrastructure;

public class WeatherStoreTests
{
    [Theory]
    [InlineData("10001", true)]
    [InlineData("10002", true)]
    [InlineData("90210", true)]
    [InlineData("60601", true)]
    [InlineData("94102", true)]
    [InlineData("02101", true)]
    [InlineData("75201", true)]
    [InlineData("33101", true)]
    [InlineData("98101", true)]
    [InlineData("85001", true)]
    public async Task IsZipCodeSupported_WithSupportedZipCode_ReturnsTrue(string zipCode, bool expected)
    {
        // Arrange
        var weatherStore = new WeatherStore("Server=localhost;Database=Test;");

        // Act
        var result = await weatherStore.IsZipCodeSupportedAsync(zipCode);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("99999")]
    [InlineData("00000")]
    [InlineData("12345")]
    [InlineData("11111")]
    public async Task IsZipCodeSupported_WithUnsupportedZipCode_ReturnsFalse(string zipCode)
    {
        // Arrange
        var weatherStore = new WeatherStore("Server=localhost;Database=Test;");

        // Act
        var result = await weatherStore.IsZipCodeSupportedAsync(zipCode);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherStore(null!));
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new WeatherStore(""));
    }
}
