using CodeleLogic.Interfaces;
using CodeleLogic.Models;

namespace CodeleLogic.Interfaces;

/// <summary>
/// Main game service that orchestrates game sessions and guess evaluation
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Creates a new game session
    /// </summary>
    /// <returns>A new game session with a target word</returns>
    Task<GameSession> CreateGameSessionAsync();
    
    /// <summary>
    /// Gets an existing game session by ID
    /// </summary>
    /// <param name="gameId">The game session ID</param>
    /// <returns>The game session if found, null otherwise</returns>
    Task<GameSession?> GetGameSessionAsync(Guid gameId);
    
    /// <summary>
    /// Submits a guess for a game session
    /// </summary>
    /// <param name="gameId">The game session ID</param>
    /// <param name="guess">The guessed word</param>
    /// <returns>The updated game session</returns>
    /// <exception cref="ArgumentException">Thrown when game ID is invalid</exception>
    /// <exception cref="InvalidOperationException">Thrown when game is already complete</exception>
    Task<GameSession> SubmitGuessAsync(Guid gameId, string guess);
}