namespace Codele.ApiService.DTOs;

/// <summary>
/// DTO representing a game session state for API responses.
/// </summary>
public record GameSessionDto
{
    public string GameId { get; init; } = string.Empty;
    public int Attempts { get; init; }
    public int MaxAttempts { get; init; }
    public bool IsComplete { get; init; }
    public bool IsWin { get; init; }
    public DateTime CreatedAt { get; init; }
    public int RemainingAttempts { get; init; }
    public List<GuessResultDto> GuessHistory { get; init; } = new();
}

/// <summary>
/// DTO representing the result of a guess for API responses.
/// </summary>
public record GuessResultDto
{
    public string GuessedWord { get; init; } = string.Empty;
    public List<LetterResultDto> Letters { get; init; } = new();
    public bool IsWin { get; init; }
    public DateTime AttemptedAt { get; init; }
}

/// <summary>
/// DTO representing a single letter result for API responses.
/// </summary>
public record LetterResultDto
{
    public char Letter { get; init; }
    public string Status { get; init; } = string.Empty; // "Correct", "IncorrectPosition", "Incorrect"
    public int Position { get; init; }
}

/// <summary>
/// DTO for creating a new game session.
/// </summary>
public record CreateGameSessionRequest
{
    public int MaxAttempts { get; init; } = 5;
}

/// <summary>
/// DTO for submitting a guess.
/// </summary>
public record SubmitGuessRequest
{
    public string GameId { get; init; } = string.Empty;
    public string Guess { get; init; } = string.Empty;
}

/// <summary>
/// DTO for word list responses.
/// </summary>
public record WordDto
{
    public string Word { get; init; } = string.Empty;
}