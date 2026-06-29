# codele-aspire-devkit
## Wordle: Developer's Edition. A .NET Aspire, Blazor WebAssembly app built for demoing C# Dev Kit.

To run this application:
1. run dotnet workload install aspire
2. ensure you have Docker running
3. set the start-up project to be the AppHost project.
  
This will launch all the needed projects and launch the [.NET Aspire dashboard](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview).

## Domain Architecture

The application follows clean architecture principles with explicit domain services and DTO boundaries:

### Domain Layer (`CodeleLogic` project)
- **IWordProvider**: Provides target words from database or in-memory sources
  - `DatabaseWordProvider`: Fetches words from SQL Server database
  - `InMemoryWordProvider`: Fallback provider for testing/development
- **IGuessEvaluator**: Pure function for evaluating guesses against target words
  - `GuessEvaluator`: Implements Wordle logic for letter status evaluation
- **IGameService**: Orchestrates game sessions and high-level operations
  - `GameService`: Manages session lifecycle and guess processing
- **Domain Models**:
  - `GameSession`: Aggregate containing game state, attempts, and completion status
  - `GuessResult` & `LetterResult`: Value objects for guess evaluation results

### API Layer (`Codel-Cloud-Native.ApiService`)
- **Clean DTOs**: All public endpoints return DTOs, no domain object leakage
  - `GameSessionDto`: Game state for API responses
  - `GuessResultDto` & `LetterResultDto`: Guess evaluation results
- **RESTful Endpoints**:
  - `GET /codele-words`: Get available words from database
  - `POST /game/create`: Create new game session
  - `POST /game/guess`: Submit guess with validation
  - `GET /game/{gameId}`: Get current game state
- **Error Handling**: Proper HTTP status codes and ProblemDetails responses

### Presentation Layer (`Codel-Cloud-Native.Web`)
- **Typed API Client**: `CodeleApiClient` consumes DTOs only
- **Clean Component State**: `PlayCodele.razor` works with DTOs, no direct domain dependencies
- **Error Handling**: Graceful handling of API failures with user feedback

This architecture enables:
- Easy testing of isolated business logic
- Future rule variants without UI/API changes
- Clear API evolution without breaking contracts
- Database-backed word list with fallback options

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
