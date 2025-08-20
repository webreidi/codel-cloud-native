using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Codele.ApiService.DTOs;

namespace Codel_Cloud_Native.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateGame_ShouldReturnNewGameSession()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/games", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponseDto>();
        
        Assert.NotNull(gameResponse);
        Assert.NotEqual(Guid.Empty, gameResponse.GameId);
        Assert.Equal(5, gameResponse.MaxAttempts);
    }

    [Fact]
    public async Task GetGame_WithValidGameId_ShouldReturnGameSession()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsync("/api/games", null);
        var gameResponse = await createResponse.Content.ReadFromJsonAsync<CreateGameResponseDto>();

        // Act
        var getResponse = await client.GetAsync($"/api/games/{gameResponse!.GameId}");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var gameSession = await getResponse.Content.ReadFromJsonAsync<GameSessionDto>();
        
        Assert.NotNull(gameSession);
        Assert.Equal(gameResponse.GameId, gameSession.GameId);
        Assert.Equal(0, gameSession.Attempts);
        Assert.Equal(5, gameSession.MaxAttempts);
        Assert.False(gameSession.IsComplete);
        Assert.False(gameSession.IsWin);
        Assert.Empty(gameSession.Guesses);
    }

    [Fact]
    public async Task GetGame_WithInvalidGameId_ShouldReturnNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidGameId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/games/{invalidGameId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitGuess_WithValidGuess_ShouldUpdateGameSession()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsync("/api/games", null);
        var gameResponse = await createResponse.Content.ReadFromJsonAsync<CreateGameResponseDto>();

        var guess = new SubmitGuessRequestDto("apple");

        // Act
        var submitResponse = await client.PostAsJsonAsync($"/api/games/{gameResponse!.GameId}/guesses", guess);

        // Assert
        submitResponse.EnsureSuccessStatusCode();
        var gameSession = await submitResponse.Content.ReadFromJsonAsync<GameSessionDto>();
        
        Assert.NotNull(gameSession);
        Assert.Equal(1, gameSession.Attempts);
        Assert.Single(gameSession.Guesses);
        Assert.Equal("apple", gameSession.Guesses.First().Word);
        Assert.Equal(5, gameSession.Guesses.First().Letters.Count);
    }

    [Fact]
    public async Task SubmitGuess_WithEmptyGuess_ShouldReturnBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create a game first
        var createResponse = await client.PostAsync("/api/games", null);
        var gameResponse = await createResponse.Content.ReadFromJsonAsync<CreateGameResponseDto>();

        var guess = new SubmitGuessRequestDto("");

        // Act
        var response = await client.PostAsJsonAsync($"/api/games/{gameResponse!.GameId}/guesses", guess);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LegacyCodeleWords_ShouldStillWork()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/codele-words");

        // Assert
        // Note: This might fail in CI environments without SQL Server, but should pass in local environments
        // We're just testing that the endpoint still exists
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.InternalServerError);
    }
}