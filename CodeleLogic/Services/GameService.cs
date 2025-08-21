using CodeleLogic.Models;

namespace CodeleLogic.Services;

/// <summary>
/// Default implementation of game service orchestrating game sessions.
/// </summary>
public class GameService : IGameService
{
    private readonly IWordProvider _wordProvider;
    private readonly IGuessEvaluator _guessEvaluator;
    private readonly Dictionary<string, GameSession> _activeSessions; // In-memory storage for now

    public GameService(IWordProvider wordProvider, IGuessEvaluator guessEvaluator)
    {
        _wordProvider = wordProvider ?? throw new ArgumentNullException(nameof(wordProvider));
        _guessEvaluator = guessEvaluator ?? throw new ArgumentNullException(nameof(guessEvaluator));
        _activeSessions = new Dictionary<string, GameSession>();
    }

    public async Task<GameSession> CreateGameSessionAsync(CancellationToken cancellationToken = default)
    {
        var targetWord = await _wordProvider.GetTargetWordAsync(cancellationToken);
        var gameId = Guid.NewGuid().ToString();
        
        var gameSession = new GameSession(gameId, targetWord);
        
        // Store session in memory (in a real app, this would be in a database/cache)
        _activeSessions[gameId] = gameSession;
        
        return gameSession;
    }

    public GuessResult ApplyGuess(GameSession gameSession, string guess)
    {
        if (gameSession == null)
            throw new ArgumentNullException(nameof(gameSession));
        
        if (string.IsNullOrWhiteSpace(guess))
            throw new ArgumentException("Guess cannot be null or whitespace", nameof(guess));

        if (gameSession.IsComplete)
            throw new InvalidOperationException("Cannot apply guess to completed game");

        // Evaluate the guess
        var letterStatuses = _guessEvaluator.EvaluateGuess(guess, gameSession.TargetWord);
        var isWin = _guessEvaluator.IsWinningGuess(guess, gameSession.TargetWord);

        // Convert to LetterResult objects
        var letterResults = letterStatuses.Select((ls, index) => 
            new LetterResult(ls.Letter, ls.Status, index)).ToList();

        var guessResult = new GuessResult(guess, letterResults, isWin);
        
        // Add attempt to session (this updates game state)
        gameSession.AddAttempt(guessResult);
        
        return guessResult;
    }

    public Task<GameSession?> GetGameSessionAsync(string gameId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(gameId))
            return Task.FromResult<GameSession?>(null);

        _activeSessions.TryGetValue(gameId, out var session);
        return Task.FromResult(session);
    }
}