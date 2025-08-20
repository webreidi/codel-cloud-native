using System.Net.Sockets;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Dapper;
using CodeleLogic.Interfaces;
using Codele.ApiService.DTOs;
using Codele.ApiService.Mapping;

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
		// Legacy endpoint for backwards compatibility - still return words from database
		app.MapGet("/codele-words", async (SqlConnection db) =>
		{
			const string sql = """
			            SELECT Id, Answer
			            FROM Words
			            """;
			var answers = await db.QueryAsync<Words>(sql);
			return answers;
		});

		// New game API endpoints using domain services
		app.MapPost("/api/games", async (IGameService gameService) =>
		{
			try
			{
				var gameSession = await gameService.CreateGameSessionAsync();
				return Results.Created($"/api/games/{gameSession.GameId}", gameSession.ToCreateResponseDto());
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to create game: {ex.Message}", statusCode: 500);
			}
		});

		app.MapGet("/api/games/{gameId:guid}", async (Guid gameId, IGameService gameService) =>
		{
			try
			{
				var gameSession = await gameService.GetGameSessionAsync(gameId);
				if (gameSession == null)
				{
					return Results.NotFound($"Game with ID {gameId} not found");
				}
				return Results.Ok(gameSession.ToDto());
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to get game: {ex.Message}", statusCode: 500);
			}
		});

		app.MapPost("/api/games/{gameId:guid}/guesses", async (Guid gameId, SubmitGuessRequestDto request, IGameService gameService) =>
		{
			try
			{
				if (string.IsNullOrWhiteSpace(request.Guess))
				{
					return Results.BadRequest("Guess cannot be empty");
				}

				var gameSession = await gameService.SubmitGuessAsync(gameId, request.Guess);
				return Results.Ok(gameSession.ToDto());
			}
			catch (ArgumentException ex)
			{
				return Results.NotFound(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Results.Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to submit guess: {ex.Message}", statusCode: 500);
			}
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