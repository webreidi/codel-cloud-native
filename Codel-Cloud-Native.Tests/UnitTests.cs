using System.Net;
using System;
using CodeleLogic;

namespace Codel_Cloud_Native.Tests;


public class UnitTests
{

    [Fact]
    public void TestGameLogic()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Fact]
    public void TestGetGuessStatuses_AllCorrect()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "apple";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.NotNull(guess.GuessStatus);
        Assert.Equal(5, guess.GuessStatus.Count);
        foreach (var status in guess.GuessStatus)
        {
            Assert.Equal(LetterStatus.Correct, status.Item2);
        }
    }

    [Fact]
    public void TestGetGuessStatuses_AllIncorrect()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "zzzzz";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.NotNull(guess.GuessStatus);
        Assert.Equal(5, guess.GuessStatus.Count);
        foreach (var status in guess.GuessStatus)
        {
            Assert.Equal(LetterStatus.Incorrect, status.Item2);
        }
    }

    [Fact]
    public void TestGetGuessStatuses_SomeCorrectSomeIncorrectPosition()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "pleap";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.NotNull(guess.GuessStatus);
        Assert.Equal(5, guess.GuessStatus.Count);
        Assert.Equal(LetterStatus.IncorrectPosition, guess.GuessStatus[0].Item2);
        Assert.Equal(LetterStatus.IncorrectPosition, guess.GuessStatus[1].Item2);
        Assert.Equal(LetterStatus.IncorrectPosition, guess.GuessStatus[2].Item2);
        Assert.Equal(LetterStatus.IncorrectPosition, guess.GuessStatus[3].Item2);
        Assert.Equal(LetterStatus.IncorrectPosition, guess.GuessStatus[4].Item2);
    }

    [Fact]
    public void TestGetGuessStatuses_DuplicateLetters()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "appla";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.NotNull(guess.GuessStatus);
        Assert.Equal(5, guess.GuessStatus.Count);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[0].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[1].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[2].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[3].Item2);
        Assert.Equal(LetterStatus.Incorrect, guess.GuessStatus[4].Item2);
    }

    [Fact]
    public void TestGetGuessNotDuplicateLetters()
    {
        var guess = new Guess("coder");
        string answer = "cader";

        guess.GetGuessStatuses(answer);

        Assert.NotNull(guess.GuessStatus);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[0].Item2);
        Assert.Equal(LetterStatus.Incorrect, guess.GuessStatus[1].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[2].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[3].Item2);
        Assert.Equal(LetterStatus.Correct, guess.GuessStatus[4].Item2);
    }

    [Fact]
    public void TestIsWinningGuess()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "apple";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.True(guess.IsWinningGuess(answer));
    }

    [Fact]
    public void TestIsNotWinningGuess()
    {
        // Arrange
        var guess = new Guess("apple");
        string answer = "zzzzz";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.False(guess.IsWinningGuess(answer));
    }

    [Fact]
    public void TestIsGuessNullorEmpty()
    {
        // Arrange
        var guess = new Guess(null);
        string answer = "apple";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.False(guess.IsWinningGuess(answer));
    }
}
