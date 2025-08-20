using CodeleLogic;
using CodeleLogic.Interfaces;
using CodeleLogic.Services;
using CodeleLogic.Models;

namespace Codel_Cloud_Native.Tests;

public class DomainServiceTests
{
    [Fact]
    public async Task InMemoryWordProvider_GetTargetWordAsync_ReturnsValidWord()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider();
        
        // Act
        var word = await wordProvider.GetTargetWordAsync();
        
        // Assert
        Assert.NotNull(word);
        Assert.NotEmpty(word);
        Assert.Equal(5, word.Length); // All predefined words are 5 letters
    }
    
    [Fact]
    public async Task InMemoryWordProvider_GetAllWordsAsync_ReturnsKnownWords()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider();
        
        // Act
        var words = await wordProvider.GetAllWordsAsync();
        
        // Assert
        Assert.NotNull(words);
        Assert.Contains("write", words);
        Assert.Contains("debug", words);
        Assert.All(words, word => Assert.Equal(5, word.Length));
    }
    
    [Fact]
    public void DefaultGuessEvaluator_EvaluateGuess_AllCorrect()
    {
        // Arrange
        var evaluator = new DefaultGuessEvaluator();
        
        // Act
        var result = evaluator.EvaluateGuess("apple", "apple");
        
        // Assert
        Assert.True(result.IsWin);
        Assert.True(result.IsValidLength);
        Assert.Equal("apple", result.Word);
        Assert.Equal(5, result.Letters.Count);
        Assert.All(result.Letters, letter => Assert.Equal(LetterStatus.Correct, letter.Status));
    }
    
    [Fact]
    public void DefaultGuessEvaluator_EvaluateGuess_AllIncorrect()
    {
        // Arrange
        var evaluator = new DefaultGuessEvaluator();
        
        // Act
        var result = evaluator.EvaluateGuess("zzzzz", "apple");
        
        // Assert
        Assert.False(result.IsWin);
        Assert.True(result.IsValidLength);
        Assert.Equal("zzzzz", result.Word);
        Assert.All(result.Letters, letter => Assert.Equal(LetterStatus.Incorrect, letter.Status));
    }
    
    [Fact]
    public void DefaultGuessEvaluator_EvaluateGuess_InvalidLength()
    {
        // Arrange
        var evaluator = new DefaultGuessEvaluator();
        
        // Act
        var result = evaluator.EvaluateGuess("app", "apple");
        
        // Assert
        Assert.False(result.IsWin);
        Assert.False(result.IsValidLength);
        Assert.Equal("app", result.Word);
    }
    
    [Fact]
    public async Task DefaultGameService_CreateGameSessionAsync_ReturnsValidSession()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider();
        var guessEvaluator = new DefaultGuessEvaluator();
        var gameService = new DefaultGameService(wordProvider, guessEvaluator);
        
        // Act
        var session = await gameService.CreateGameSessionAsync();
        
        // Assert
        Assert.NotEqual(Guid.Empty, session.GameId);
        Assert.NotEmpty(session.TargetWord);
        Assert.Equal(5, session.MaxAttempts);
        Assert.Empty(session.Attempts);
        Assert.False(session.IsComplete);
        Assert.False(session.IsWin);
    }
    
    [Fact]
    public async Task DefaultGameService_SubmitGuessAsync_WinningGuess()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider();
        var guessEvaluator = new DefaultGuessEvaluator();
        var gameService = new DefaultGameService(wordProvider, guessEvaluator);
        var session = await gameService.CreateGameSessionAsync();
        
        // Act
        var updatedSession = await gameService.SubmitGuessAsync(session.GameId, session.TargetWord);
        
        // Assert
        Assert.Single(updatedSession.Attempts);
        Assert.True(updatedSession.IsWin);
        Assert.True(updatedSession.IsComplete);
        Assert.True(updatedSession.Attempts.First().IsWin);
    }
    
    [Fact]
    public async Task DefaultGameService_SubmitGuessAsync_NonExistentGame_ThrowsException()
    {
        // Arrange
        var wordProvider = new InMemoryWordProvider();
        var guessEvaluator = new DefaultGuessEvaluator();
        var gameService = new DefaultGameService(wordProvider, guessEvaluator);
        var nonExistentGameId = Guid.NewGuid();
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            gameService.SubmitGuessAsync(nonExistentGameId, "guess"));
    }
    
    [Fact]
    public void GameSession_AddGuess_CompletedGame_ThrowsException()
    {
        // Arrange
        var session = new GameSession(Guid.NewGuid(), "apple", 1);
        var guessResult = new GuessResult("apple", new List<LetterResult>().AsReadOnly(), true, true);
        session.AddGuess(guessResult);
        
        // Act & Assert
        var anotherGuess = new GuessResult("wrong", new List<LetterResult>().AsReadOnly(), false, true);
        Assert.Throws<InvalidOperationException>(() => session.AddGuess(anotherGuess));
    }
}