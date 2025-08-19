namespace Codel_Cloud_Native.Web.DTOs;

/// <summary>
/// DTO representing the result of a single letter evaluation
/// </summary>
public record LetterResultDto(
    char Letter,
    string Status // "Correct", "IncorrectPosition", "Incorrect"
);

/// <summary>
/// DTO representing the result of evaluating a guess
/// </summary>
public record GuessResultDto(
    string Word,
    IReadOnlyList<LetterResultDto> Letters,
    bool IsWin,
    bool IsValidLength
);

/// <summary>
/// DTO representing a game session
/// </summary>
public record GameSessionDto(
    Guid GameId,
    int Attempts,
    int MaxAttempts,
    bool IsComplete,
    bool IsWin,
    IReadOnlyList<GuessResultDto> Guesses
);

/// <summary>
/// DTO for creating a new game session response
/// </summary>
public record CreateGameResponseDto(
    Guid GameId,
    int MaxAttempts
);

/// <summary>
/// DTO for submitting a guess request
/// </summary>
public record SubmitGuessRequestDto(
    string Guess
);