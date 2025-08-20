# codele-aspire-devkit
## Wordle: Developer's Edition. A .NET Aspire, Blazor WebAssembly app built for demoing C# Dev Kit.

To run this application:
1. run dotnet workload install aspire
2. ensure you have Docker running
3. set the start-up project to be the AppHost project.
  
This will launch all the needed projects and launch the [.NET Aspire dashboard](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview).

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
