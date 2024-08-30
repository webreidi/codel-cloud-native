using System.Net.Sockets;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Codele.ApiService;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.AddSqlServerClient("sqlserver");

		// Add service defaults & Aspire components.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddProblemDetails();

		builder.Services.AddProblemDetails();
		builder.Services.AddEndpointsApiExplorer();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.UseExceptionHandler();

		var summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		string[] words = [];

		app.MapGet("/sample-data", async(SqlConnection db) =>
		{
			const string sql = """
			            SELECT Id, Answer
			            FROM Words
			            """;
			var Answers = await db.QueryAsync<Words>(sql);

			var answer = Enumerable.Range(1, 14).Select(index =>
					new SampleData
					(
						words[Random.Shared.Next(words.Length)]
					))
					.ToArray();
			return answer;

		});

		//app.MapGet("/sample-data", async(MyDb1Context context) =>
		//{
		//	var answers = await context.Words.ToListAsync();
		//	foreach (var word in answers)
		//	{
		//		words.Append(word.Answer);
		//	}
		//	var answer = Enumerable.Range(1, 14).Select(index =>
		//			new SampleData
		//			(
		//				words[Random.Shared.Next(words.Length)]
		//			))
		//			.ToArray();
		//	return answer;
		//});

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

		app.MapDefaultEndpoints();

		app.Run();
	}
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record SampleData(string answer);

public record Words(int Id, string Answer);


