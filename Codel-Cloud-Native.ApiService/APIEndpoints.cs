using System.Net.Sockets;
using System.Reflection.Metadata;
using Microsoft.Data.SqlClient;
using Dapper;
using CodeleLogic.Services;
using Codele.ApiService.DTOs;

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
		// Get available words from database
		app.MapGet("/codele-words", async (IWordProvider wordProvider) =>
		{
			var words = await wordProvider.GetAllWordsAsync();
			return words.Select(w => w.ToDto()).ToArray();
		});

		// Create a new game session
		app.MapPost("/game/create", async (IGameService gameService, CreateGameSessionRequest? request) =>
		{
			try
			{
				var gameSession = await gameService.CreateGameSessionAsync();
				var dto = gameSession.ToDto();
				
				// Remove target word from response for security
				return Results.Ok(dto);
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to create game session: {ex.Message}");
			}
		});

		// Submit a guess
		app.MapPost("/game/guess", async (IGameService gameService, SubmitGuessRequest request) =>
		{
			try
			{
				if (string.IsNullOrWhiteSpace(request.GameId))
					return Results.BadRequest("GameId is required");

				if (string.IsNullOrWhiteSpace(request.Guess))
					return Results.BadRequest("Guess is required");

				var gameSession = await gameService.GetGameSessionAsync(request.GameId);
				if (gameSession == null)
					return Results.NotFound("Game session not found");

				if (gameSession.IsComplete)
					return Results.Conflict("Game is already complete");

				var guessResult = gameService.ApplyGuess(gameSession, request.Guess);
				var gameSessionDto = gameSession.ToDto();

				return Results.Ok(new { 
					GameSession = gameSessionDto, 
					GuessResult = guessResult.ToDto() 
				});
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return Results.Conflict(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to process guess: {ex.Message}");
			}
		});

		// Get game session state
		app.MapGet("/game/{gameId}", async (IGameService gameService, string gameId) =>
		{
			try
			{
				var gameSession = await gameService.GetGameSessionAsync(gameId);
				if (gameSession == null)
					return Results.NotFound("Game session not found");

				var dto = gameSession.ToDto();
				return Results.Ok(dto);
			}
			catch (Exception ex)
			{
				return Results.Problem($"Failed to get game session: {ex.Message}");
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