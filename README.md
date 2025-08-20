# codele-aspire-devkit
## Wordle: Developer's Edition. A .NET Aspire, Blazor WebAssembly app built for demoing C# Dev Kit.

To run this application:
1. run dotnet workload install aspire
2. ensure you have Docker running
3. set the start-up project to be the AppHost project.
  
This will launch all the needed projects and launch the [.NET Aspire dashboard](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview).

## Architecture Overview

### Domain Layer (`CodeleLogic`)
The game logic is now cleanly separated into domain services with clear interfaces:

- **`IWordProvider`**: Provides target words for games (current implementation uses in-memory coding-related words)
- **`IGuessEvaluator`**: Evaluates guesses against target words using the existing Wordle logic
- **`GameSession`**: Represents a game session with attempts, completion status, and win state
- **`IGameService`**: Orchestrates game creation and guess submission

This architecture enables:
- Easy testing of game logic in isolation
- Future extensibility for variants (hard mode, timed games)
- Simple service replacement via dependency injection
- Clear separation between game rules and UI/API concerns

### API Layer (`Codel-Cloud-Native.ApiService`)
Clean REST endpoints that return only DTOs:

- `POST /api/games` - Create a new game session
- `GET /api/games/{gameId}` - Get current game state
- `POST /api/games/{gameId}/guesses` - Submit a guess

All endpoints return structured DTOs (`GameSessionDto`, `GuessResultDto`, `LetterResultDto`) with proper HTTP status codes and error handling.

### Blazor Client (`Codel-Cloud-Native.Web`)
The UI layer now:
- Uses only DTOs from the API (no direct domain model dependencies)
- Implements async game operations with error handling
- Maintains the same user experience while being decoupled from game logic

## Quick Wins

This branch includes a few small improvements for developer experience and observability:

- Health endpoints
	- `/healthz` - liveness probe (returns 200 when the process is up)
	- `/readyz` - readiness probe (verifies SQL Server connectivity if `ConnectionStrings:codele` is configured and Redis ping if `Redis:ConnectionString` is provided). Returns 200 with JSON details when healthy or 503 when a dependency is unhealthy.

- Typed HttpClient for the Web project
	- `CodeleApiClient` is registered as a typed HttpClient and configured with a simple retry policy to improve resilience against transient 5xx responses.

Example: configure the Web project to talk to the ApiService (service discovery is used by Aspire):

```json
{
	"Services": {
		"ApiBaseUrl": "https+http://apiservice"
	}
}
```

Then the `CodeleApiClient` can be injected into Blazor components and called as an async typed client.

If you want these quick wins adjusted (different retry policy, additional checks in `/readyz`, or documentation tweaks), open an issue or request a follow-up PR.
