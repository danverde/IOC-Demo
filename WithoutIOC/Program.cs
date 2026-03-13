using WithoutIOC.View;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var connectionString = builder.Configuration.GetConnectionString("WeatherDatabase")
                       ?? "Server=localhost;Database=Weather;";

var weatherController = new WeatherController(connectionString);

app.MapGet("/weatherforecast", (string zipCode) => weatherController.GetWeatherForecast(zipCode))
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();