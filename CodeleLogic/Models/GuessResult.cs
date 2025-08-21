namespace CodeleLogic.Models;

/// <summary>
/// Represents the result of evaluating a guess.
/// </summary>
public class GuessResult
{
    public string GuessedWord { get; private set; }
    public IEnumerable<LetterResult> Letters { get; private set; }
    public bool IsWin { get; private set; }
    public DateTime AttemptedAt { get; private set; }

    public GuessResult(string guessedWord, IEnumerable<LetterResult> letters, bool isWin)
    {
        GuessedWord = guessedWord ?? throw new ArgumentNullException(nameof(guessedWord));
        Letters = letters ?? throw new ArgumentNullException(nameof(letters));
        IsWin = isWin;
        AttemptedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Represents the status of a single letter in a guess.
/// </summary>
public class LetterResult
{
    public char Letter { get; private set; }
    public LetterStatus Status { get; private set; }
    public int Position { get; private set; }

    public LetterResult(char letter, LetterStatus status, int position)
    {
        Letter = letter;
        Status = status;
        Position = position;
    }
}