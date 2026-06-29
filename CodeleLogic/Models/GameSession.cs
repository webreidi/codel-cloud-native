using CodeleLogic;
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
    
    /// <summary>
    /// Dictionary tracking the status of letters that have been guessed
    /// Key: letter character (uppercase), Value: best status achieved for that letter
    /// </summary>
    public Dictionary<char, LetterStatus> GuessedLetters { get; private set; }

    public GameSession(string gameId, string targetWord, int maxAttempts = 5)
    {
        GameId = gameId ?? throw new ArgumentNullException(nameof(gameId));
        TargetWord = targetWord ?? throw new ArgumentNullException(nameof(targetWord));
        MaxAttempts = maxAttempts;
        Attempts = new List<GuessResult>();
        GuessedLetters = new Dictionary<char, LetterStatus>();
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

        // Update guessed letters tracking
        foreach (var letterResult in guessResult.Letters)
        {
            char upperLetter = char.ToUpper(letterResult.Letter);

            // Only update if we don't have this letter or if the new status is better
            if (!GuessedLetters.ContainsKey(upperLetter) ||
                GetStatusPriority(letterResult.Status) > GetStatusPriority(GuessedLetters[upperLetter]))
            {
                GuessedLetters[upperLetter] = letterResult.Status;
            }
        }

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
    /// Gets the priority of a letter status for tracking purposes
    /// Higher priority statuses take precedence over lower ones
    /// </summary>
    private static int GetStatusPriority(LetterStatus status)
    {
        return status switch
        {
            LetterStatus.Correct => 3,
            LetterStatus.IncorrectPosition => 2,
            LetterStatus.Incorrect => 1,
            _ => 0
        };
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