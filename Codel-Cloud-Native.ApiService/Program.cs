using Codele.ApiService;
using CodeleLogic.Interfaces;
using CodeleLogic.Services;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Text.Json;

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
builder.Services.AddSingleton<IWordProvider, InMemoryWordProvider>();
builder.Services.AddSingleton<IGuessEvaluator, DefaultGuessEvaluator>();
builder.Services.AddSingleton<IGameService, DefaultGameService>();

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





