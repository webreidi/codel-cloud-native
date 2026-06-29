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

        var results = new List<(char, LetterStatus)>();
        
        // Use the existing logic from Guess.cs with improvements
        var length = Math.Min(guess.Length, targetWord.Length);
        
        for (int i = 0; i < length; i++)
        {
            char letter = guess[i];
            bool isDuplicateInAnswer = targetWord.Count(x => x == letter) > 1;

            // Check for duplicate letters - use existing logic
            if ((results.Contains((letter, LetterStatus.Correct)) || results.Contains((letter, LetterStatus.IncorrectPosition))) && !isDuplicateInAnswer)
            {
                results.Add((letter, LetterStatus.Incorrect));
            }
            else // regular Wordle logic
            {
                if (guess[i] == targetWord[i]) 
                    results.Add((letter, LetterStatus.Correct));
                else if (targetWord.Contains(letter)) 
                    results.Add((letter, LetterStatus.IncorrectPosition));
                else 
                    results.Add((letter, LetterStatus.Incorrect));
            }
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