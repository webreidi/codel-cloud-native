# Codel Cloud Native - GitHub Copilot Instructions

**Codel Cloud Native** is a .NET 9.0 Aspire application featuring a Blazor WebAssembly frontend that implements "Codele" - a developer-themed Wordle game. The application uses SQL Server and Redis containers orchestrated through .NET Aspire's Developer Control Plane.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Prerequisites - CRITICAL SETUP REQUIRED
**You MUST install .NET 9.0 SDK and Aspire workload before any development work:**

1. **Install .NET 9.0 SDK** (takes ~3 minutes, NEVER CANCEL):
   ```bash
   curl -sSL https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh | bash -s -- --channel 9.0 --install-dir ~/.dotnet
   export PATH=$HOME/.dotnet:$PATH
   ```
   
2. **Verify .NET 9.0 installation**:
   ```bash
   dotnet --version  # Should show 9.0.x
   ```

**Install Aspire templates**
   ```bash
dotnet new install Aspire.ProjectTemplates::9.4.0 --force
   ```

4. **Verify Aspire installation**:
   ```bash
   dotnet new list aspire # Should show 'aspire' in the list
   ```

### Build and Test Process
**Set PATH and run from repository root:**
```bash
export PATH=$HOME/.dotnet:$PATH
cd /path/to/codel-cloud-native
```

1. **Restore packages** (takes ~51 seconds first time, NEVER CANCEL, set timeout to 120+ seconds):
   ```bash
   dotnet restore
   ```

2. **Build solution** (takes ~17 seconds, NEVER CANCEL, set timeout to 60+ seconds):
   ```bash
   dotnet build
   ```

3. **Run unit tests** (takes ~7 seconds for specific tests, NEVER CANCEL, set timeout to 30+ seconds):
   ```bash
   # Run specific working unit tests:
   dotnet test --filter "TestGameLogic"
   
   # Or run all unit tests (note: integration tests may fail in CI environments):
   dotnet test  # Takes ~25+ seconds, 1 integration test may timeout
   ```

### Known Limitations - READ CAREFULLY

**Integration Tests**: The WebTests integration test will FAIL in CI/container environments due to Aspire DCP timeout issues. This is expected behavior and NOT a code problem. The error message will be:
```
Polly.Timeout.TimeoutRejectedException : The operation didn't complete within the allowed timeout of '00:00:20'
```

**Application Startup**: Running `dotnet run` on the AppHost project may fail in CI environments due to DCP orchestration issues. This is environment-specific, not a code issue.

**Unit Tests Work**: All game logic unit tests in the CodeleLogic project work correctly and should be used for validation.

## Validation

### Always validate changes with these WORKING commands:
1. **Build validation**:
   ```bash
   export PATH=$HOME/.dotnet:$PATH
   dotnet build  # Must succeed
   ```

2. **Unit test validation**:
   ```bash
   export PATH=$HOME/.dotnet:$PATH
   dotnet test --filter "TestGameLogic"  # Must pass
   ```

3. **Game logic tests** (all these should pass):
   ```bash
   dotnet test --filter "TestGetGuessStatuses_AllCorrect"
   dotnet test --filter "TestGetGuessStatuses_AllIncorrect" 
   dotnet test --filter "TestIsWinningGuess"
   ```

### DO NOT rely on these commands (they fail in CI environments):
- `dotnet run` on AppHost project
- Full integration test suite
- Application UI testing

## Project Structure

### Key Projects (6 total):
```
Codel-Cloud-Native.sln
├── Codel-Cloud-Native.AppHost/          # Aspire orchestrator (startup project)
├── Codel-Cloud-Native.Web/              # Blazor WebAssembly frontend  
├── Codel-Cloud-Native.ApiService/       # Backend API service
├── Codel-Cloud-Native.Tests/            # Unit and integration tests
├── CodeleLogic/                         # Core game logic library
└── Codel-Cloud-Native.ServiceDefaults/  # Shared Aspire services
```

### Important Files:
- `Program.cs` files: Entry points for each service
- `Codel-Cloud-Native.AppHost/Program.cs`: Defines SQL Server + Redis + service orchestration
- `CodeleLogic/`: Contains `Guess.cs`, `LetterStatus.cs` - core Wordle game logic
- `Codel-Cloud-Native.Web/Components/Pages/PlayCodele.razor`: Main game UI component

## Common Development Tasks

### Making Code Changes:
1. **Always validate prerequisites first** (PATH set, .NET 9.0, Aspire workload)
2. **Build after changes**: `dotnet build` (17 seconds)
3. **Test game logic**: `dotnet test --filter "TestGameLogic"` (7 seconds)
4. **DO NOT** attempt full application startup in CI environments

### Working with Game Logic:
- Core game logic is in `CodeleLogic/Guess.cs`
- UI logic is in `Codel-Cloud-Native.Web/Components/Pages/PlayCodele.razor`
- API endpoints are in `Codel-Cloud-Native.ApiService/Program.cs`
- Always test changes with: `dotnet test --filter "TestGetGuessStatuses"`

### Debugging Integration Issues:
- If integration tests fail with timeout errors, this is expected in CI
- If build fails with ".NET 9.0 not supported", install .NET 9.0 SDK first
- If Aspire commands fail, install Aspire workload first

## Technology Stack
- **.NET 9.0** - Required, not backward compatible with .NET 8.0
- **Aspire 9.3.0** - Container orchestration and service discovery
- **Blazor WebAssembly** - Frontend framework
- **SQL Server** - Database (via Docker container)
- **Redis** - Caching (via Docker container)
- **xUnit** - Testing framework
- **Docker** - Container runtime

## Timing Expectations - CRITICAL FOR CI/CD

**NEVER CANCEL these operations - they take time but will complete:**

| Operation | Duration | Timeout Setting | Notes |
|-----------|----------|----------------|-------|
| .NET 9.0 SDK Install | ~3 minutes | 600+ seconds | One-time setup |
| Aspire Workload Install | ~2-3 minutes | 600+ seconds | One-time setup |
| First `dotnet restore` | ~51 seconds | 120+ seconds | Downloads packages |
| `dotnet build` | ~17 seconds | 60+ seconds | Compiles code |
| Unit tests (filtered) | ~7 seconds | 30+ seconds | Fast validation |
| Full test suite | ~25+ seconds | 60+ seconds | 1 test will fail in CI |

**Always set appropriate timeouts in CI/CD pipelines and NEVER CANCEL long-running operations.**