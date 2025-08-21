using Codele.ApiService;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Text.Json;
using CodeleLogic.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddSqlServerClient("codele");

// Add services to the container.
builder.Services.AddProblemDetails();
// Health checks (liveness & readiness)
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

// Register domain services
// GameService stores in-memory game sessions and must be a singleton so sessions persist
// across multiple HTTP requests. GuessEvaluator and IWordProvider are stateless and
// safe to register as singletons here as well.
builder.Services.AddSingleton<IGuessEvaluator, GuessEvaluator>();
builder.Services.AddSingleton<IGameService, GameService>();

// Register word provider with database connection as a singleton so it can be used
// by the singleton GameService without lifetime conflicts.
builder.Services.AddSingleton<IWordProvider>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("codele");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Fallback to in-memory provider if no database connection
        return new InMemoryWordProvider();
    }
    return new DatabaseWordProvider(connectionString);
});

// If Redis is configured, register a ConnectionMultiplexer for use in readiness checks
var redisConn = builder.Configuration.GetSection("Redis").GetValue<string>("ConnectionString");
if (!string.IsNullOrEmpty(redisConn))
{
    builder.Services.AddSingleton(sp => ConnectionMultiplexer.Connect(redisConn));
}

var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.WeatherForecastApi();
app.CodeleGameApi();

// Liveness and readiness endpoints
// /healthz - simple liveness probe
app.MapHealthChecks("/healthz");

// /readyz - readiness probe with optional SQL / Redis checks
app.MapGet("/readyz", async (IServiceProvider services) =>
{
    var config = services.GetService<IConfiguration>();
    var results = new Dictionary<string, object>();
    var allHealthy = true;

    // Check SQL Server if a connection string is configured
    var connStr = config?.GetConnectionString("codele");
    if (!string.IsNullOrEmpty(connStr))
    {
        try
        {
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            results["sql"] = new { status = "healthy" };
        }
        catch (Exception ex)
        {
            results["sql"] = new { status = "unhealthy", detail = ex.Message };
            allHealthy = false;
        }
    }

    // Check Redis if a ConnectionMultiplexer is registered
    try
    {
        var mux = services.GetService<ConnectionMultiplexer>();
        if (mux is not null)
        {
            var db = mux.GetDatabase();
            var ping = await db.PingAsync();
            results["redis"] = new { status = "healthy", pingMs = ping.TotalMilliseconds };
        }
    }
    catch (Exception ex)
    {
        results["redis"] = new { status = "unhealthy", detail = ex.Message };
        allHealthy = false;
    }

    if (allHealthy)
    {
        return Results.Ok(results);
    }

    return Results.StatusCode(503);
});

app.MapDefaultEndpoints();

app.Run();





