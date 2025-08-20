namespace CodeleLogic.Interfaces;

/// <summary>
/// Provides target words for Codele games
/// </summary>
public interface IWordProvider
{
    /// <summary>
    /// Gets a target word for a new game session
    /// </summary>
    /// <returns>A target word for the game</returns>
    Task<string> GetTargetWordAsync();
    
    /// <summary>
    /// Gets all available words (for testing or word list management)
    /// </summary>
    /// <returns>Collection of available words</returns>
    Task<IEnumerable<string>> GetAllWordsAsync();
}