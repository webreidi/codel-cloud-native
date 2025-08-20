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
}

public interface ICodeleApiClient
{
    Task<CodeleWords[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default);
}

public record CodeleWords(string Answer)
{
    public override string ToString() => Answer;
    // Backwards-compatible property used by existing Razor components
    public string toString => Answer;
}