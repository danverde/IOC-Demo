namespace WithoutIOC.Domain;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string ZipCode)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
