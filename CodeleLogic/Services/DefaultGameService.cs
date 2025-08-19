using CodeleLogic.Interfaces;
using CodeleLogic.Models;
using System.Collections.Concurrent;

namespace CodeleLogic.Services;

/// <summary>
/// Default game service implementation using in-memory storage
/// </summary>
public class DefaultGameService : IGameService
{
    private readonly IWordProvider _wordProvider;
    private readonly IGuessEvaluator _guessEvaluator;
    private readonly ConcurrentDictionary<Guid, GameSession> _gameSessions = new();

    public DefaultGameService(IWordProvider wordProvider, IGuessEvaluator guessEvaluator)
    {
        _wordProvider = wordProvider ?? throw new ArgumentNullException(nameof(wordProvider));
        _guessEvaluator = guessEvaluator ?? throw new ArgumentNullException(nameof(guessEvaluator));
    }

    public async Task<GameSession> CreateGameSessionAsync()
    {
        var gameId = Guid.NewGuid();
        var targetWord = await _wordProvider.GetTargetWordAsync();
        var gameSession = new GameSession(gameId, targetWord);
        
        _gameSessions[gameId] = gameSession;
        return gameSession;
    }

    public Task<GameSession?> GetGameSessionAsync(Guid gameId)
    {
        _gameSessions.TryGetValue(gameId, out var gameSession);
        return Task.FromResult(gameSession);
    }

    public async Task<GameSession> SubmitGuessAsync(Guid gameId, string guess)
    {
        var gameSession = await GetGameSessionAsync(gameId);
        if (gameSession == null)
        {
            throw new ArgumentException($"Game session with ID {gameId} not found", nameof(gameId));
        }

        if (gameSession.IsComplete)
        {
            throw new InvalidOperationException("Cannot submit guess to completed game");
        }

        var guessResult = _guessEvaluator.EvaluateGuess(guess, gameSession.TargetWord);
        gameSession.AddGuess(guessResult);
        
        return gameSession;
    }
}