namespace CodeleLogic.Services;

/// <summary>
/// Default implementation of guess evaluation using the existing Wordle logic.
/// </summary>
public class GuessEvaluator : IGuessEvaluator
{
    public IEnumerable<(char Letter, LetterStatus Status)> EvaluateGuess(string guess, string targetWord)
    {
        if (string.IsNullOrEmpty(guess) || string.IsNullOrEmpty(targetWord))
            return Enumerable.Empty<(char, LetterStatus)>();

        var length = Math.Min(guess.Length, targetWord.Length);
        var statuses = new LetterStatus[length];
        var isExactMatch = new bool[length];
        var unmatchedTargetLetterCounts = new Dictionary<char, int>();

        // First pass: mark exact-position matches and count the remaining target letters.
        for (int i = 0; i < length; i++)
        {
            if (guess[i] == targetWord[i])
            {
                isExactMatch[i] = true;
                statuses[i] = LetterStatus.Correct;
            }
            else
            {
                var targetLetter = targetWord[i];
                unmatchedTargetLetterCounts[targetLetter] = unmatchedTargetLetterCounts.GetValueOrDefault(targetLetter) + 1;
            }
        }

        // Second pass: only assign IncorrectPosition if an unmatched target letter is still available.
        for (int i = 0; i < length; i++)
        {
            if (isExactMatch[i])
            {
                continue;
            }

            var guessLetter = guess[i];
            if (unmatchedTargetLetterCounts.TryGetValue(guessLetter, out var count) && count > 0)
            {
                statuses[i] = LetterStatus.IncorrectPosition;
                unmatchedTargetLetterCounts[guessLetter] = count - 1;
            }
            else
            {
                statuses[i] = LetterStatus.Incorrect;
            }
        }

        var results = new List<(char, LetterStatus)>(length);
        for (int i = 0; i < length; i++)
        {
            results.Add((guess[i], statuses[i]));
        }

        // If guess is longer than answer, mark remaining letters as incorrect
        if (guess.Length > targetWord.Length)
        {
            for (int i = length; i < guess.Length; i++)
            {
                results.Add((guess[i], LetterStatus.Incorrect));
            }
        }

        return results;
    }

    public bool IsWinningGuess(string guess, string targetWord)
    {
        if (string.IsNullOrEmpty(guess) || string.IsNullOrEmpty(targetWord))
            return false;

        return guess.Equals(targetWord, StringComparison.OrdinalIgnoreCase);
    }
}