using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeleLogic
{
    /// <summary>
    /// Class <c>Guess</c> handles the game logic for Codele.
    /// </summary>
    public class Guess
    {
        public string? Word { get; set; }
        public List<(char, LetterStatus)>? GuessStatus { get; private set; }

        public Guess(string? word)
        {
            Word = word;
        }

        /// <summary>
        /// Method <c>GetGuessStatuses</c> evaluates each letter in a guess and determines their status relative to the answer.
        /// </summary>
        public void GetGuessStatuses(string answer)
        {
            if (string.IsNullOrEmpty(Word))
                return;

            // Ensure the guess is exactly 5 characters long
            if (Word.Length != 5)
            {
                Console.WriteLine("Error: The guess must be exactly 5 characters long.");
                return;
            }

            GuessStatus = new List<(char, LetterStatus)>();

            for (int i = 0; i < 5; i++)
            {
                char letter = Word[i];
                bool isDuplicateInAnswer = answer.Count(x => x == letter) > 1;

                // Check for duplicate letters
                if ((GuessStatus?.Contains((letter, LetterStatus.Correct)) ?? false) || 
                    (GuessStatus?.Contains((letter, LetterStatus.IncorrectPosition)) ?? false) && 
                    isDuplicateInAnswer)
                {
                    GuessStatus.Add((letter, LetterStatus.Incorrect));
                }
                else
                {
                    if (Word[i] == answer[i])
                    {
                        GuessStatus.Add((letter, LetterStatus.Correct));
                    }
                    else if (answer.Contains(letter))
                    {
                        GuessStatus.Add((letter, LetterStatus.IncorrectPosition));
                    }
                    else
                    {
                        GuessStatus.Add((letter, LetterStatus.Incorrect));
                    }
                }
            }
        }

        /// <summary>
        /// Method <c>IsWinningGuess</c> checks if the guess is the correct answer, thus winning the game.
        /// </summary>
        public bool IsWinningGuess(string answer)
        {
            return !string.IsNullOrEmpty(Word) && Word.Equals(answer, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Enum <c>LetterStatus</c> defines the possible statuses of each letter in a guess.
    /// </summary>
    public enum LetterStatus
    {
        Correct,
        IncorrectPosition,
        Incorrect
    }
}
