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
    
    /// <summary>
    /// Dictionary tracking the status of letters that have been guessed
    /// Key: letter character (uppercase), Value: best status achieved for that letter
    /// </summary>
    public Dictionary<char, LetterStatus> GuessedLetters { get; }
    
    public GameSession(Guid gameId, string targetWord, int maxAttempts = 5)
    {
        GameId = gameId;
        TargetWord = targetWord ?? throw new ArgumentNullException(nameof(targetWord));
        MaxAttempts = maxAttempts;
        Attempts = new List<GuessResult>();
        GuessedLetters = new Dictionary<char, LetterStatus>();
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
}