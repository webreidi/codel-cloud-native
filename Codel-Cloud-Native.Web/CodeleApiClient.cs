namespace Codel_Cloud_Native.Web;

public class CodeleApiClient : ICodeleApiClient
{
    private readonly HttpClient _httpClient;

    public CodeleApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CodeleWords[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default)
    {
        var list = new List<CodeleWords>();

        // Use the new DTO-based endpoint
        var words = await _httpClient.GetFromJsonAsync<WordDto[]>("/codele-words", cancellationToken);
        if (words != null)
        {
            var limitedWords = words.Take(maxItems);
            list.AddRange(limitedWords.Select(w => new CodeleWords(w.Word)));
        }

        return list.ToArray();
    }

    public async Task<GameSessionDto> CreateGameSessionAsync(CancellationToken cancellationToken = default)
    {
        var request = new CreateGameSessionRequest { MaxAttempts = 5 };
        var response = await _httpClient.PostAsJsonAsync("/game/create", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var gameSession = await response.Content.ReadFromJsonAsync<GameSessionDto>(cancellationToken);
        return gameSession ?? throw new InvalidOperationException("Failed to create game session");
    }

    public async Task<GameGuessResponse> SubmitGuessAsync(string gameId, string guess, CancellationToken cancellationToken = default)
    {
        var request = new SubmitGuessRequest { GameId = gameId, Guess = guess };
        var response = await _httpClient.PostAsJsonAsync("/game/guess", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<GameGuessResponse>(cancellationToken);
        return result ?? throw new InvalidOperationException("Failed to submit guess");
    }

    public async Task<GameSessionDto> GetGameSessionAsync(string gameId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<GameSessionDto>($"/game/{gameId}", cancellationToken);
        return response ?? throw new InvalidOperationException("Game session not found");
    }
}

public interface ICodeleApiClient
{
    Task<CodeleWords[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default);
    Task<GameSessionDto> CreateGameSessionAsync(CancellationToken cancellationToken = default);
    Task<GameGuessResponse> SubmitGuessAsync(string gameId, string guess, CancellationToken cancellationToken = default);
    Task<GameSessionDto> GetGameSessionAsync(string gameId, CancellationToken cancellationToken = default);
}

// Client-side DTOs (mirror of API DTOs)
public record WordDto(string Word);

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

public record GuessResultDto
{
    public string GuessedWord { get; init; } = string.Empty;
    public List<LetterResultDto> Letters { get; init; } = new();
    public bool IsWin { get; init; }
    public DateTime AttemptedAt { get; init; }
}

public record LetterResultDto
{
    public char Letter { get; init; }
    public string Status { get; init; } = string.Empty; // "Correct", "IncorrectPosition", "Incorrect"
    public int Position { get; init; }
}

public record CreateGameSessionRequest
{
    public int MaxAttempts { get; init; } = 5;
}

public record SubmitGuessRequest
{
    public string GameId { get; init; } = string.Empty;
    public string Guess { get; init; } = string.Empty;
}

public record GameGuessResponse
{
    public GameSessionDto GameSession { get; init; } = new();
    public GuessResultDto GuessResult { get; init; } = new();
}

public record CodeleWords(string Answer)
{
    public override string ToString() => Answer;
    // Backwards-compatible property used by existing Razor components
    public string toString => Answer;
}