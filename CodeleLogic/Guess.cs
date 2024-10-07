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
        public List<(char, LetterStatus)>? GuessStatus;

        public Guess(string? word)
        {
            this.Word = word;
        }

        /// <summary>
        /// Method <c>GetGuessStatuses</c> evaluates each letter in a guess and determines their status relative to the answer.
        /// </summary>
        public void GetGuessStatuses(string answer)
        {
            if (string.IsNullOrEmpty(Word))
                return;

            // Add this check to ensure the guess is exactly 5 characters long
            if (Word.Length != 5)
            {
                Console.WriteLine("Error: The guess must be exactly 5 characters long.");
                return;
            }

            // Count letters in the answer
            var letterCount = new Dictionary<char, int>();
            foreach (var letter in answer)
            {
                if (letterCount.ContainsKey(letter))
                    letterCount[letter]++;
                else
                    letterCount[letter] = 1;
            }

            GuessStatus = new List<(char, LetterStatus)>();

            // First pass to check for Correct guesses
            for (int i = 0; i < Word.Length; i++)
            {
                char letter = Word[i];

                // Check if the letter is correct
                if (Word[i] == answer[i])
                {
                    GuessStatus.Add((letter, LetterStatus.Correct));
                    // Decrement the count in the letterCount dictionary
                    letterCount[letter]--;
                }
                else
                {
                    GuessStatus.Add((letter, LetterStatus.Incorrect)); // Default to Incorrect
                }
            }

            // Second pass to check for IncorrectPosition
            for (int i = 0; i < Word.Length; i++)
            {
                char letter = Word[i];

                // Check for IncorrectPosition only if not already marked as Correct
                if (GuessStatus[i].Item2 != LetterStatus.Correct && letterCount.ContainsKey(letter) && letterCount[letter] > 0)
                {
                    GuessStatus[i] = (letter, LetterStatus.IncorrectPosition);
                    letterCount[letter]--;
                }
            }
        }

        /// <summary>
        /// Method <c>IsWinningGuess</c> checks if the guess is the correct answer, thus winning the game.
        /// </summary>
        public bool IsWinningGuess(string answer)
        {
            if (!string.IsNullOrEmpty(Word))
            {
                return Word.Equals(answer, StringComparison.OrdinalIgnoreCase);
            }
            return false;
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
