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
            if (!string.IsNullOrEmpty(Word))
            {
                GuessStatus = new();

                // Iterate over the overlapping length of the guess and answer to avoid index exceptions.
                var length = Math.Min(Word.Length, answer.Length);
                var statuses = new LetterStatus[length];
                var isExactMatch = new bool[length];
                var unmatchedAnswerLetterCounts = new Dictionary<char, int>();

                // First pass: mark exact matches and count remaining answer letters.
                for (int i = 0; i < length; i++)
                {
                    if (Word[i] == answer[i])
                    {
                        isExactMatch[i] = true;
                        statuses[i] = LetterStatus.Correct;
                    }
                    else
                    {
                        char answerLetter = answer[i];
                        unmatchedAnswerLetterCounts[answerLetter] = unmatchedAnswerLetterCounts.GetValueOrDefault(answerLetter) + 1;
                    }
                }

                // Second pass: consume unmatched answer letters for incorrect-position matches.
                for (int i = 0; i < length; i++)
                {
                    if (isExactMatch[i])
                    {
                        continue;
                    }

                    char guessLetter = Word[i];
                    if (unmatchedAnswerLetterCounts.TryGetValue(guessLetter, out var count) && count > 0)
                    {
                        statuses[i] = LetterStatus.IncorrectPosition;
                        unmatchedAnswerLetterCounts[guessLetter] = count - 1;
                    }
                    else
                    {
                        statuses[i] = LetterStatus.Incorrect;
                    }
                }

                for (int i = 0; i < length; i++)
                {
                    GuessStatus.Add((Word[i], statuses[i]));
                }

                // If guess is longer than answer, mark remaining letters as incorrect
                if (Word.Length > answer.Length)
                {
                    for (int i = length; i < Word.Length; i++)
                    {
                        GuessStatus.Add((Word[i], LetterStatus.Incorrect));
                    }
                }
            }
        }

        /// <summary>
        /// Method <c>IsWinningGuess</c> Checks if the guess is the correct answer, thus winning the game.
        /// </summary>
        public bool IsWinningGuess(string answer)
        {
            if (!string.IsNullOrEmpty(Word))
            {
                if (Word.Equals(answer)) return true;
                return false;
            }
            return false;
        }

    }
}