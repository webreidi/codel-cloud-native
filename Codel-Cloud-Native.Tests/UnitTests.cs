using System.Net;
using System;
using CodeleLogic;
using CodeleLogic.Models;
using CodeleLogic.Services;

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

    [Fact]
    public void TestIsGuessWrongLength()
    {
        // Arrange
        var guess = new Guess("app");
        string answer = "apple";

        // Act
        guess.GetGuessStatuses(answer);

        // Assert
        Assert.False(guess.IsWinningGuess(answer));
    }

    [Fact]
    public void TestGameSession_TrackGuessedLetters_SingleGuess()
    {
        // Arrange
        var gameSession = new GameSession(Guid.NewGuid(), "APPLE", 5);
        var guessEvaluator = new DefaultGuessEvaluator();
        
        // Act
        var guessResult = guessEvaluator.EvaluateGuess("HELLO", "APPLE");
        gameSession.AddGuess(guessResult);
        
        // Assert - HELLO vs APPLE:
        // H=Incorrect, E=IncorrectPosition (E is in position 4 in APPLE), L=IncorrectPosition (L is in position 2), L=Incorrect, O=Incorrect
        // Since we track the best status per letter, L should be IncorrectPosition (not Incorrect)
        Assert.Equal(4, gameSession.GuessedLetters.Count); // H, E, L, O (duplicates consolidated)
        Assert.Equal(LetterStatus.Incorrect, gameSession.GuessedLetters['H']);
        Assert.Equal(LetterStatus.IncorrectPosition, gameSession.GuessedLetters['E']); // E is in APPLE but wrong position
        Assert.Equal(LetterStatus.IncorrectPosition, gameSession.GuessedLetters['L']); // L is in APPLE but wrong position (best status wins)
        Assert.Equal(LetterStatus.Incorrect, gameSession.GuessedLetters['O']);
    }

    [Fact]
    public void TestGameSession_TrackGuessedLetters_LetterStatusUpgrade()
    {
        // Arrange
        var gameSession = new GameSession(Guid.NewGuid(), "APPLE", 5);
        var guessEvaluator = new DefaultGuessEvaluator();
        
        // Act - first guess: HELLO vs APPLE (L will be IncorrectPosition)
        var guessResult1 = guessEvaluator.EvaluateGuess("HELLO", "APPLE");
        gameSession.AddGuess(guessResult1);
        
        // Act - second guess: APPLE vs APPLE (all letters correct, including L upgraded to Correct)
        var guessResult2 = guessEvaluator.EvaluateGuess("APPLE", "APPLE");
        gameSession.AddGuess(guessResult2);
        
        // Assert - L should be upgraded from IncorrectPosition to Correct
        Assert.Equal(LetterStatus.Correct, gameSession.GuessedLetters['A']);
        Assert.Equal(LetterStatus.Correct, gameSession.GuessedLetters['P']);
        Assert.Equal(LetterStatus.Correct, gameSession.GuessedLetters['L']); // Upgraded from IncorrectPosition to Correct
        Assert.Equal(LetterStatus.Correct, gameSession.GuessedLetters['E']); // Upgraded from IncorrectPosition to Correct
    }

    [Fact]
    public void TestGameSession_TrackGuessedLetters_NoDowngrade()
    {
        // Arrange
        var gameSession = new GameSession(Guid.NewGuid(), "APPLE", 5);
        var guessEvaluator = new DefaultGuessEvaluator();
        
        // Act - first guess: A is correct in APPLE
        var guessResult1 = guessEvaluator.EvaluateGuess("ALOFT", "APPLE");
        gameSession.AddGuess(guessResult1);
        
        // Act - second guess: A would be incorrect position if it appeared elsewhere, but shouldn't downgrade
        var guessResult2 = guessEvaluator.EvaluateGuess("BEAUT", "APPLE");  // A is not in APPLE at position 2
        gameSession.AddGuess(guessResult2);
        
        // Assert - A should remain Correct from first guess
        Assert.Equal(LetterStatus.Correct, gameSession.GuessedLetters['A']);
        Assert.Contains('B', gameSession.GuessedLetters.Keys);
        Assert.Contains('E', gameSession.GuessedLetters.Keys);
        Assert.Contains('U', gameSession.GuessedLetters.Keys);
        Assert.Contains('T', gameSession.GuessedLetters.Keys);
    }
}
