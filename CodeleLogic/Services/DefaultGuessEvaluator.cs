using CodeleLogic.Interfaces;

namespace CodeleLogic.Services;

/// <summary>
/// Default guess evaluator that uses the existing Guess class logic
/// </summary>
public class DefaultGuessEvaluator : IGuessEvaluator
{
    public GuessResult EvaluateGuess(string guess, string targetWord)
    {
        if (string.IsNullOrEmpty(guess))
        {
            return new GuessResult(
                guess ?? string.Empty,
                Array.Empty<LetterResult>(),
                false,
                false
            );
        }
        
        // Use existing Guess class to leverage current game logic
        var guessObj = new Guess(guess);
        guessObj.GetGuessStatuses(targetWord);
        
        var letterResults = guessObj.GuessStatus?.Select(status => 
            new LetterResult(status.Item1, status.Item2)).ToList() ?? new List<LetterResult>();
        
        bool isWin = guessObj.IsWinningGuess(targetWord);
        bool isValidLength = guess.Length == targetWord.Length;
        
        return new GuessResult(
            guess,
            letterResults.AsReadOnly(),
            isWin,
            isValidLength
        );
    }
}