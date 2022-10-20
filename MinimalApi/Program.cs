using Microsoft.EntityFrameworkCore;
using MinimalApiCrud;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherForecastContext>(options =>
{
    options.UseInMemoryDatabase("WeatherForecasts");
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();

});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
      .GetRequiredService<WeatherForecastContext>();

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    db.Forecasts.AddRange(forecast);
    db.SaveChanges();
    db.Dispose();
}

using var minimalApiCrudBuilderWithViewModel =
    app.MapCrud<WeatherForecast, string?, WeatherForecastContext>()
    .SetupMapping<WeatherForecast, WeatherForecastViewModel>(x =>
        x.Map(dest => dest.DisplayDate,
            src => src.Date.ToLongDateString())
        .Map(dest => dest.Temperature,
                        src => src.TemperatureC + "°C"))
    .GetAll<WeatherForecastViewModel>("/weatherforecast/list/view", config: x => x.WithName("GetWeatherForecastViewModel"))
    .GetOneById<WeatherForecastViewModel>("/weatherforecast/one/view")
    .Filter<WeatherForecastViewModel>(new Dictionary<string, string> {
        {nameof(WeatherForecast.TemperatureC),$"{nameof(WeatherForecast.TemperatureC)} == @0" }, {nameof(WeatherForecast.Summary), $"{nameof(WeatherForecast.Summary)} == @0" } }
        , FilterLogic.OR, "/weatherforecast/filter/view", config: c=>c.WithName("Filter weather"))
    .Insert<WeatherForecastDto>("/weatherforecast/view")
    .Update<WeatherForecastViewModel>("/weatherforecast/view")
    .Delete("/weatherforecast/view/{id:int}");

using var minimalApiCrudBuilder =
    app.MapCrud<WeatherForecast, string?, WeatherForecastContext>()
    .GetAll("/weatherforecast/list", config: x => x.WithName("GetWeatherForecast"))
    .GetOneById("/weatherforecast/one")
    .Filter(new Dictionary<string, string> {
        {nameof(WeatherForecast.TemperatureC),$"{nameof(WeatherForecast.TemperatureC)} == @0" }, {nameof(WeatherForecast.Summary), $"{nameof(WeatherForecast.Summary)} == @0" } }
        , FilterLogic.OR, "/weatherforecast/filter")
    .Insert("/weatherforecast")
    .Update("/weatherforecast")
    .Delete("/weatherforecast/{id:int}");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

public class WeatherForecast : IEntity<string?>
{
    public WeatherForecast()
    {

    }

    public WeatherForecast(DateTime dateTime, int tempC, string summary)
    {
        Date = dateTime;
        TemperatureC = tempC;
        Summary = summary;
    }

    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Summary { get; set; } = null!;
}

public class WeatherForecastDto
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; } = null!;
}

public class WeatherForecastViewModel
{
    public string Id { get; set; } = null!;
    public string DisplayDate { get; set; } = null!;
    public string Temperature { get; set; } = null!;
    public string Summary { get; set; } = null!;
}

class WeatherForecastContext : DbContext, IDataContext<WeatherForecast>
{
    public WeatherForecastContext(DbContextOptions options) : base(options) { }
    public DbSet<WeatherForecast> Forecasts { get; set; } = null!;

    public async ValueTask<int> AddAsync(WeatherForecast model)
    {
        Forecasts.Add(model);
        return await SaveChangesAsync();
    }

    public async ValueTask<int> RemoveAsync(WeatherForecast model)
    {
        Forecasts.Remove(model);
        return await SaveChangesAsync();
    }

    public async ValueTask<int> UpdateAsync(WeatherForecast model)
    {
        // the tracking detected changes automatically.
        return await SaveChangesAsync();
    }

    IQueryable<WeatherForecast> IDataContext<WeatherForecast>.Set<T>() => Forecasts;
}