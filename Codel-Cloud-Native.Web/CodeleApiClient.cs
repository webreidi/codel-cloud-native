namespace Codel_Cloud_Native.Web;

public class CodeleApiClient(HttpClient httpClient)
{
    public async Task<CodeleWords[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default)
    {
        List<CodeleWords> answers = null;

        await foreach (var answer in httpClient.GetFromJsonAsAsyncEnumerable<CodeleWords>("/codele-words", cancellationToken))
        {
            if (answers?.Count >= maxItems)
            {
                break;
            }
            if (answer is not null)
            {
                answers ??= [];
                answers.Add(answer);
            }
        }

        return answers?.ToArray() ?? [];
    }
}

public record CodeleWords(string answer)
{
    public string toString => answer;

}