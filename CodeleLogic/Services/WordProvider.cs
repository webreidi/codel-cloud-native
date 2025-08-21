using Microsoft.Data.SqlClient;
using Dapper;

namespace CodeleLogic.Services;

/// <summary>
/// Word provider that fetches words from SQL Server database.
/// </summary>
public class DatabaseWordProvider : IWordProvider
{
    private readonly string _connectionString;

    public DatabaseWordProvider(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<string> GetTargetWordAsync(CancellationToken cancellationToken = default)
    {
        var words = await GetAllWordsAsync(cancellationToken);
        var wordArray = words.ToArray();
        
        if (wordArray.Length == 0)
            throw new InvalidOperationException("No words available in database");

        // Return a random word
        return wordArray[Random.Shared.Next(wordArray.Length)];
    }

    public async Task<IEnumerable<string>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Answer
            FROM Words
            """;

        await using var connection = new SqlConnection(_connectionString);
        var words = await connection.QueryAsync<string>(sql);
        return words;
    }
}

/// <summary>
/// In-memory word provider for testing or fallback scenarios.
/// </summary>
public class InMemoryWordProvider : IWordProvider
{
    private readonly string[] _words;

    public InMemoryWordProvider(string[]? words = null)
    {
        _words = words ?? new[]
        {
            "write", "cobol", "coder", "array", "false", 
            "build", "table", "techy", "razor", "azure", 
            "agile", "cloud", "serve", "debug"
        };
    }

    public Task<string> GetTargetWordAsync(CancellationToken cancellationToken = default)
    {
        if (_words.Length == 0)
            throw new InvalidOperationException("No words available");

        var word = _words[Random.Shared.Next(_words.Length)];
        return Task.FromResult(word);
    }

    public Task<IEnumerable<string>> GetAllWordsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<string>>(_words);
    }
}