using CodeleLogic.Models;
using CodeleLogic;

namespace Codele.ApiService.DTOs;

/// <summary>
/// Extension methods for mapping between domain objects and DTOs.
/// </summary>
public static class DtoMappingExtensions
{
    public static GameSessionDto ToDto(this GameSession gameSession)
    {
        return new GameSessionDto
        {
            GameId = gameSession.GameId,
            Attempts = gameSession.Attempts.Count,
            MaxAttempts = gameSession.MaxAttempts,
            IsComplete = gameSession.IsComplete,
            IsWin = gameSession.IsWin,
            CreatedAt = gameSession.CreatedAt,
            RemainingAttempts = gameSession.RemainingAttempts,
            GuessHistory = gameSession.Attempts.Select(a => a.ToDto()).ToList()
        };
    }

    public static GuessResultDto ToDto(this GuessResult guessResult)
    {
        return new GuessResultDto
        {
            GuessedWord = guessResult.GuessedWord,
            Letters = guessResult.Letters.Select(l => l.ToDto()).ToList(),
            IsWin = guessResult.IsWin,
            AttemptedAt = guessResult.AttemptedAt
        };
    }

    public static LetterResultDto ToDto(this LetterResult letterResult)
    {
        return new LetterResultDto
        {
            Letter = letterResult.Letter,
            Status = letterResult.Status.ToString(),
            Position = letterResult.Position
        };
    }

    public static WordDto ToDto(this string word)
    {
        return new WordDto { Word = word };
    }
}