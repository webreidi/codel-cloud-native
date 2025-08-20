using CodeleLogic.Interfaces;

namespace CodeleLogic.Services;

/// <summary>
/// In-memory word provider using a predefined list of coding-related words
/// </summary>
public class InMemoryWordProvider : IWordProvider
{
    private readonly string[] _words = {
        "write", "cobol", "coder", "array", "false", 
        "build", "table", "techy", "razor", "azure", 
        "agile", "cloud", "serve", "debug"
    };
    
    private readonly Random _random = new();

    public Task<string> GetTargetWordAsync()
    {
        var word = _words[_random.Next(_words.Length)];
        return Task.FromResult(word);
    }

    public Task<IEnumerable<string>> GetAllWordsAsync()
    {
        return Task.FromResult<IEnumerable<string>>(_words);
    }
}