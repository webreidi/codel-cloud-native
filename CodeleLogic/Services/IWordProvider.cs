namespace CodeleLogic.Services;

/// <summary>
/// Provides target words for the game.
/// </summary>
public interface IWordProvider
{
    /// <summary>
    /// Gets a random target word for the game.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A target word for the game</returns>
    Task<string> GetTargetWordAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available words.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all available words</returns>
    Task<IEnumerable<string>> GetAllWordsAsync(CancellationToken cancellationToken = default);
}