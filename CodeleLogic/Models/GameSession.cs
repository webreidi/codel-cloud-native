using CodeleLogic.Interfaces;

namespace CodeleLogic.Models;

/// <summary>
/// Represents a game session with attempts, target word, and game state
/// </summary>
public class GameSession
{
    public Guid GameId { get; }
    public string TargetWord { get; }
    public List<GuessResult> Attempts { get; }
    public int MaxAttempts { get; }
    public bool IsComplete => IsWin || Attempts.Count >= MaxAttempts;
    public bool IsWin => Attempts.Any(a => a.IsWin);
    
    public GameSession(Guid gameId, string targetWord, int maxAttempts = 5)
    {
        GameId = gameId;
        TargetWord = targetWord ?? throw new ArgumentNullException(nameof(targetWord));
        MaxAttempts = maxAttempts;
        Attempts = new List<GuessResult>();
    }
    
    /// <summary>
    /// Adds a guess result to the game session
    /// </summary>
    /// <param name="guessResult">The result of evaluating a guess</param>
    /// <exception cref="InvalidOperationException">Thrown when game is already complete</exception>
    public void AddGuess(GuessResult guessResult)
    {
        if (IsComplete)
            throw new InvalidOperationException("Cannot add guess to completed game");
            
        Attempts.Add(guessResult);
    }
}