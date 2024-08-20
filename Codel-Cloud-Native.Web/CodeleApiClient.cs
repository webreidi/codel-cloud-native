namespace Codel_Cloud_Native.Web;

public class CodeleApiClient(HttpClient httpClient)
{
    public async Task<SampleData[]> GetSampleDataAsync(int maxItems = 100, CancellationToken cancellationToken = default)
    {
        List<SampleData> answers = null;

        await foreach (var answer in httpClient.GetFromJsonAsAsyncEnumerable<SampleData>("/sample-data", cancellationToken))
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

public record SampleData(string answer)
{
    public string toString => answer;

}