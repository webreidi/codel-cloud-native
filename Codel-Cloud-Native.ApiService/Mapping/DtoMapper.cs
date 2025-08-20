using CodeleLogic;
using CodeleLogic.Interfaces;
using CodeleLogic.Models;
using Codele.ApiService.DTOs;

namespace Codele.ApiService.Mapping;

/// <summary>
/// Maps between domain models and DTOs
/// </summary>
public static class DtoMapper
{
    public static LetterResultDto ToDto(this LetterResult letterResult)
    {
        return new LetterResultDto(
            letterResult.Letter,
            letterResult.Status.ToString()
        );
    }

    public static GuessResultDto ToDto(this GuessResult guessResult)
    {
        return new GuessResultDto(
            guessResult.Word,
            guessResult.Letters.Select(l => l.ToDto()).ToList().AsReadOnly(),
            guessResult.IsWin,
            guessResult.IsValidLength
        );
    }

    public static GameSessionDto ToDto(this GameSession gameSession)
    {
        return new GameSessionDto(
            gameSession.GameId,
            gameSession.IsComplete ? gameSession.TargetWord : null,
            gameSession.Attempts.Count,
            gameSession.MaxAttempts,
            gameSession.IsComplete,
            gameSession.IsWin,
            gameSession.Attempts.Select(a => a.ToDto()).ToList().AsReadOnly()
        );
    }

    public static CreateGameResponseDto ToCreateResponseDto(this GameSession gameSession)
    {
        return new CreateGameResponseDto(
            gameSession.GameId,
            gameSession.MaxAttempts
        );
    }
}