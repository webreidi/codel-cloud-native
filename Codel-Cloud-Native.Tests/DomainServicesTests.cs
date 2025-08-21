using CodeleLogic.Services;
using CodeleLogic.Models;
using CodeleLogic;

namespace Codel_Cloud_Native.Tests;

public class DomainServicesTests
{
    [Fact]
    public void GuessEvaluator_AllCorrect_ReturnsAllCorrectStatuses()
    {
        // Arrange
        var evaluator = new GuessEvaluator();
        var guess = "apple";
        var target = "apple";

        // Act
        var result = evaluator.EvaluateGuess(guess, target);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(5, resultList.Count);
        Assert.All(resultList, r => Assert.Equal(LetterStatus.Correct, r.Status));
    }

    [Fact]
    public void GuessEvaluator_AllIncorrect_ReturnsAllIncorrectStatuses()
    {
        // Arrange
        var evaluator = new GuessEvaluator();
        var guess = "apple";
        var target = "zzzzz";

        // Act
        var result = evaluator.EvaluateGuess(guess, target);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(5, resultList.Count);
        Assert.All(resultList, r => Assert.Equal(LetterStatus.Incorrect, r.Status));
    }

    [Fact]
    public void GuessEvaluator_IsWinningGuess_CorrectAnswer_ReturnsTrue()
    {
        // Arrange
        var evaluator = new GuessEvaluator();
        var guess = "apple";
        var target = "apple";

        // Act
        var result = evaluator.IsWinningGuess(guess, target);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void GuessEvaluator_IsWinningGuess_IncorrectAnswer_ReturnsFalse()
    {
        // Arrange
        var evaluator = new GuessEvaluator();
        var guess = "apple";
        var target = "grape";

        // Act
        var result = evaluator.IsWinningGuess(guess, target);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task InMemoryWordProvider_GetTargetWord_ReturnsValidWord()
    {
        // Arrange
        var words = new[] { "apple", "grape", "table" };
        var provider = new InMemoryWordProvider(words);

        // Act
        var result = await provider.GetTargetWordAsync();

        // Assert
        Assert.Contains(result, words);
    }

    [Fact]
    public async Task InMemoryWordProvider_GetAllWords_ReturnsAllWords()
    {
        // Arrange
        var words = new[] { "apple", "grape", "table" };
        var provider = new InMemoryWordProvider(words);

        // Act
        var result = await provider.GetAllWordsAsync();

        // Assert
        Assert.Equal(words, result);
    }

    [Fact]
    public async Task GameService_CreateGameSession_ReturnsValidSession()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider(new[] { "apple" });
        var guessEvaluator = new GuessEvaluator();
        var gameService = new GameService(wordProvider, guessEvaluator);

        // Act
        var result = await gameService.CreateGameSessionAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.GameId);
        Assert.Equal("apple", result.TargetWord);
        Assert.Equal(5, result.MaxAttempts);
        Assert.False(result.IsComplete);
        Assert.False(result.IsWin);
    }

    [Fact]
    public async Task GameService_ApplyGuess_WinningGuess_CompletesGame()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider(new[] { "apple" });
        var guessEvaluator = new GuessEvaluator();
        var gameService = new GameService(wordProvider, guessEvaluator);
        var gameSession = await gameService.CreateGameSessionAsync();

        // Act
        var result = gameService.ApplyGuess(gameSession, "apple");

        // Assert
        Assert.True(result.IsWin);
        Assert.True(gameSession.IsComplete);
        Assert.True(gameSession.IsWin);
        Assert.Equal(1, gameSession.Attempts.Count);
    }

    [Fact]
    public async Task GameService_ApplyGuess_IncorrectGuess_ContinuesGame()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider(new[] { "apple" });
        var guessEvaluator = new GuessEvaluator();
        var gameService = new GameService(wordProvider, guessEvaluator);
        var gameSession = await gameService.CreateGameSessionAsync();

        // Act
        var result = gameService.ApplyGuess(gameSession, "grape");

        // Assert
        Assert.False(result.IsWin);
        Assert.False(gameSession.IsComplete);
        Assert.False(gameSession.IsWin);
        Assert.Equal(1, gameSession.Attempts.Count);
    }

    [Fact]
    public async Task GameService_ApplyGuess_MaxAttempts_CompletesGame()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider(new[] { "apple" });
        var guessEvaluator = new GuessEvaluator();
        var gameService = new GameService(wordProvider, guessEvaluator);
        var gameSession = await gameService.CreateGameSessionAsync();

        // Act - Make 5 incorrect guesses
        for (int i = 0; i < 5; i++)
        {
            gameService.ApplyGuess(gameSession, "wrong");
        }

        // Assert
        Assert.True(gameSession.IsComplete);
        Assert.False(gameSession.IsWin);
        Assert.Equal(5, gameSession.Attempts.Count);
    }

    [Fact]
    public async Task GameService_ApplyGuess_CompletedGame_ThrowsException()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider(new[] { "apple" });
        var guessEvaluator = new GuessEvaluator();
        var gameService = new GameService(wordProvider, guessEvaluator);
        var gameSession = await gameService.CreateGameSessionAsync();
        
        // Complete the game
        gameService.ApplyGuess(gameSession, "apple");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            gameService.ApplyGuess(gameSession, "grape"));
    }
}