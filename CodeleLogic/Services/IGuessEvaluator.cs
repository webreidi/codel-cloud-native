namespace CodeleLogic.Services;

/// <summary>
/// Evaluates guesses against target words.
/// </summary>
public interface IGuessEvaluator
{
    /// <summary>
    /// Evaluates a guess against the target word.
    /// </summary>
    /// <param name="guess">The guessed word</param>
    /// <param name="targetWord">The target word</param>
    /// <returns>List of letter statuses for each position</returns>
    IEnumerable<(char Letter, LetterStatus Status)> EvaluateGuess(string guess, string targetWord);

    /// <summary>
    /// Checks if the guess is a winning guess.
    /// </summary>
    /// <param name="guess">The guessed word</param>
    /// <param name="targetWord">The target word</param>
    /// <returns>True if the guess matches the target word exactly</returns>
    bool IsWinningGuess(string guess, string targetWord);
}