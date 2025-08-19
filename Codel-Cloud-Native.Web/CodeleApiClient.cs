using Codel_Cloud_Native.Web.DTOs;

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

        await foreach (var item in _httpClient.GetFromJsonAsAsyncEnumerable<CodeleWords>("/codele-words", cancellationToken))
        {
            if (item is null) continue;
            list.Add(item);
            if (list.Count >= maxItems) break;
        }

        return list.ToArray();
    }

    public async Task<CreateGameResponseDto> CreateGameAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("/api/games", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var gameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponseDto>(cancellationToken);
        return gameResponse ?? throw new InvalidOperationException("Failed to deserialize game response");
    }

    public async Task<GameSessionDto> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var gameSession = await _httpClient.GetFromJsonAsync<GameSessionDto>($"/api/games/{gameId}", cancellationToken);
        return gameSession ?? throw new InvalidOperationException($"Game {gameId} not found");
    }

    public async Task<GameSessionDto> SubmitGuessAsync(Guid gameId, string guess, CancellationToken cancellationToken = default)
    {
        var request = new SubmitGuessRequestDto(guess);
        var response = await _httpClient.PostAsJsonAsync($"/api/games/{gameId}/guesses", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var gameSession = await response.Content.ReadFromJsonAsync<GameSessionDto>(cancellationToken);
        return gameSession ?? throw new InvalidOperationException("Failed to deserialize game session");
    }
}

public interface ICodeleApiClient
{
    Task<CodeleWords[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default);
    Task<CreateGameResponseDto> CreateGameAsync(CancellationToken cancellationToken = default);
    Task<GameSessionDto> GetGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<GameSessionDto> SubmitGuessAsync(Guid gameId, string guess, CancellationToken cancellationToken = default);
}

public record CodeleWords(string Answer)
{
    public override string ToString() => Answer;
    // Backwards-compatible property used by existing Razor components
    public string toString => Answer;
}