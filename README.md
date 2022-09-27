# MinimalApiCrud

## Nuget package

You can use MinimalApiCrud installing the Nuget package from here [https://www.nuget.org/packages/MinimalApiCrud](https://www.nuget.org/packages/MinimalApiCrud)

## What is it?

It's a very simple library that permit to create CRUD endpoints for your entities.  
It works with .NET Minimal API and .NET 6

Keep in mind that this library try to offer a base set of CRUD endpoints. If you need to customize something you have to write your endpoint.

## Methods

*   **MapCrud**: initialize the library with the data model, the id data type and the data context.
*   **SetupMapping**: configure the mapping between data model and view model. (for details see [https://github.com/MapsterMapper/Mapster](https://github.com/MapsterMapper/Mapster))
*   **GetAll**: setup the Read endpoint. It's paginated.
*   **GetOneById**: setup the single Read by id endpoint.
*   **Filter**: setup the filters and the comparing logics to retrieve the results. (for details see [https://dynamic-linq.net/advanced-parse-lambda#dynamic-lambda)](https://dynamic-linq.net/advanced-parse-lambda#dynamic-lambda))
*   **Insert**: setup the Create endpoint.
*   **Update**: setup the Update endpoint.
*   **Delete**: setup the Delete enpoint.

## Example

In this example you can see how configurate endpoints and setup mappings.

For more details, please check the example here [https://github.com/matteoventuri7/MinimalApiCrud/tree/master/MinimalApi](https://github.com/matteoventuri7/MinimalApiCrud/tree/master/MinimalApi)

### Program.cs

```c#
using var minimalApiCrudBuilder =
    app.MapCrud<WeatherForecast, int, WeatherForecastContext>()
    .SetupMapping<WeatherForecast, WeatherForecastViewModel>(x =>
        x.Map(dest => dest.DisplayDate, src => src.Date.ToLongDateString())
        .Map(dest => dest.Temperature, src => src.TemperatureC + "°C"))
    .GetAll<WeatherForecastViewModel>(config: x => x.WithName("GetWeatherForecast"))
    .GetOneById<WeatherForecastViewModel>()
    .Filter<WeatherForecastViewModel>(new Dictionary<string, string> {
        {nameof(WeatherForecast.TemperatureC),$"{nameof(WeatherForecast.TemperatureC)} == @0" }, 
        {nameof(WeatherForecast.Summary), $"{nameof(WeatherForecast.Summary)} == @0" } }
        , FilterLogic.OR)
    .Insert<WeatherForecastDto>()
    .Update<WeatherForecastViewModel>()
    .Delete("/weatherforecast/{id:int}");
```
