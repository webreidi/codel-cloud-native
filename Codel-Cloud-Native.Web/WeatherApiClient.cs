using System.Text.Json.Serialization;

namespace Codel_Cloud_Native.Web;

public class WeatherApiClient(HttpClient httpClient)
{
    private static readonly string ApiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ?? "demo"; // Using demo mode - set WEATHER_API_KEY for production
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get current weather for a few major cities
            var cities = new[] { "London", "New York", "Tokyo", "Sydney", "Paris" };
            var forecasts = new List<WeatherForecast>();

            foreach (var city in cities.Take(maxItems))
            {
                var currentWeather = await GetCurrentWeatherAsync(city, cancellationToken);
                if (currentWeather != null)
                {
                    forecasts.Add(currentWeather);
                }
            }

            return forecasts.ToArray();
        }
        catch
        {
            // Fallback to mock data if API fails
            return GetMockWeatherData(maxItems);
        }
    }

    private async Task<WeatherForecast?> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"{BaseUrl}/weather?q={city}&appid={ApiKey}&units=metric";
            var response = await httpClient.GetFromJsonAsync<OpenWeatherResponse>(url, cancellationToken);

            if (response != null)
            {
                return new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now),
                    (int)Math.Round(response.Main.Temp),
                    $"{response.Weather[0].Description} in {city}"
                );
            }
        }
        catch
        {
            // Return null if this city fails, continue with others
        catch (Exception ex)
        {
            // Return null if this city fails, continue with others
            Debug.WriteLine($"Error fetching weather for {city}: {ex}");
        }

        return null;
    }

    private WeatherForecast[] GetMockWeatherData(int maxItems)
    {
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        return Enumerable.Range(1, Math.Min(maxItems, 5)).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Models for OpenWeatherMap API response
public class OpenWeatherResponse
{
    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public WeatherData[] Weather { get; set; } = Array.Empty<WeatherData>();
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
}

public class WeatherData
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
