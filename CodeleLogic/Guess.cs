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
        private int attempts = 0; // Track the number of attempts

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

            GuessStatus = new List<(char, LetterStatus)>();

            for (int i = 0; i < 5; i++)
            {
                char letter = Word[i];
                bool isDuplicateInAnswer = answer.Count(x => x == letter) > 1;

                // Check for duplicate letters
                if ((GuessStatus.Contains((letter, LetterStatus.Correct)) || 
                     GuessStatus.Contains((letter, LetterStatus.IncorrectPosition))) && 
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

            // Increment attempts and check if the game should end
            attempts++;
            if (attempts >= 5 || IsWinningGuess(answer))
            {
                EndGame();
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

        /// <summary>
        /// Method <c>EndGame</c> handles logic when the game ends.
        /// </summary>
        private void EndGame()
        {
            Console.WriteLine("Game Over. You've reached the maximum attempts or won the game!");
            // Implement additional logic to reset or end the game
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
