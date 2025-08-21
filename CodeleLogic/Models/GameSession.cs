using CodeleLogic.Services;

namespace CodeleLogic.Models;

/// <summary>
/// Represents a game session with state and attempts.
/// </summary>
public class GameSession
{
    public string GameId { get; private set; }
    public string TargetWord { get; private set; }
    public List<GuessResult> Attempts { get; private set; }
    public int MaxAttempts { get; private set; }
    public bool IsComplete { get; private set; }
    public bool IsWin { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public GameSession(string gameId, string targetWord, int maxAttempts = 5)
    {
        GameId = gameId ?? throw new ArgumentNullException(nameof(gameId));
        TargetWord = targetWord ?? throw new ArgumentNullException(nameof(targetWord));
        MaxAttempts = maxAttempts;
        Attempts = new List<GuessResult>();
        IsComplete = false;
        IsWin = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a guess attempt to the session.
    /// </summary>
    /// <param name="guessResult">The result of the guess</param>
    /// <exception cref="InvalidOperationException">Thrown when the game is already complete or max attempts reached</exception>
    public void AddAttempt(GuessResult guessResult)
    {
        if (IsComplete)
            throw new InvalidOperationException("Cannot add attempt to completed game");

        if (Attempts.Count >= MaxAttempts)
            throw new InvalidOperationException("Maximum attempts reached");

        Attempts.Add(guessResult);

        // Check if this attempt wins the game
        if (guessResult.IsWin)
        {
            IsWin = true;
            IsComplete = true;
        }
        // Check if we've reached max attempts
        else if (Attempts.Count >= MaxAttempts)
        {
            IsComplete = true;
        }
    }

    /// <summary>
    /// Gets the current attempt number (1-based).
    /// </summary>
    public int CurrentAttempt => Attempts.Count + 1;

    /// <summary>
    /// Gets the number of remaining attempts.
    /// </summary>
    public int RemainingAttempts => Math.Max(0, MaxAttempts - Attempts.Count);
}