using CodeleLogic.Models;

namespace CodeleLogic.Services;

/// <summary>
/// Orchestrates game sessions and provides high-level game operations.
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Creates a new game session.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A new game session</returns>
    Task<GameSession> CreateGameSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a guess to a game session.
    /// </summary>
    /// <param name="gameSession">The current game session</param>
    /// <param name="guess">The player's guess</param>
    /// <returns>The result of the guess</returns>
    GuessResult ApplyGuess(GameSession gameSession, string guess);

    /// <summary>
    /// Gets a game session by ID (for future use when sessions are persisted).
    /// </summary>
    /// <param name="gameId">The game session ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The game session if found</returns>
    Task<GameSession?> GetGameSessionAsync(string gameId, CancellationToken cancellationToken = default);
}