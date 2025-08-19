namespace CodeleLogic.Interfaces;

/// <summary>
/// Result of evaluating a guess against a target word
/// </summary>
public record GuessResult(
    string Word,
    IReadOnlyList<LetterResult> Letters,
    bool IsWin,
    bool IsValidLength
);

/// <summary>
/// Result of evaluating a single letter in a guess
/// </summary>
public record LetterResult(
    char Letter,
    LetterStatus Status
);

/// <summary>
/// Service for evaluating guesses against target words
/// </summary>
public interface IGuessEvaluator
{
    /// <summary>
    /// Evaluates a guess against a target word
    /// </summary>
    /// <param name="guess">The guessed word</param>
    /// <param name="targetWord">The target word to compare against</param>
    /// <returns>Result of the guess evaluation</returns>
    GuessResult EvaluateGuess(string guess, string targetWord);
}