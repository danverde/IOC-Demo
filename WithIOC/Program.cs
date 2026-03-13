using With_IOC.Application;
using With_IOC.Infrastructure;
using With_IOC.View;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("WeatherDatabase")
                       ?? "Server=localhost;Database=Weather;";

// Register dependencies for dependency injection
builder.Services.AddScoped<IWeatherAdapter, WeatherAdapter>();
builder.Services.AddScoped<IWeatherStore>(sp => new WeatherStore(connectionString));
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<WeatherController>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (string zipCode, WeatherController controller) => await controller.GetWeatherForecastAsync(zipCode))
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
