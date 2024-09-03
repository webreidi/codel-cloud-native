using System.Net.Sockets;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Codele.ApiService;

public static class ApiEndpoints
{
	
	public static WebApplication WeatherForecastApi(this WebApplication app)
	{
		var summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
		
		app.MapGet("/weatherforecast", () =>
		{
			var forecast = Enumerable.Range(1, 5).Select(index =>
				new WeatherForecast
				(
					DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
					Random.Shared.Next(-20, 55),
					summaries[Random.Shared.Next(summaries.Length)]
				))
				.ToArray();
			return forecast;
		});

		return app;
	}

	public static WebApplication CodeleGameApi(this WebApplication app)
	{

		string[] words = new string[14];

		app.MapGet("/sample-data", async (SqlConnection db) =>
		{
			const string sql = """
			            SELECT Id, Answer
			            FROM Words
			            """;
			var Answers = await db.QueryAsync<Words>(sql);
			int count = 0;

			foreach (var item in Answers)
			{
				words[count++] = item.Answer;
			}

			var answer = Enumerable.Range(1, Answers.Count()).Select(index =>
					new SampleData
					(
						words[Random.Shared.Next(words.Length)]
					))
					.ToArray();

			return Answers;
		});

		return app;
	}
}

	record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
	{
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}

	record SampleData(string answer);

	public record Words(int Id, string Answer);