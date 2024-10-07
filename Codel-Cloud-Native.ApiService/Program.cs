using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Codel_Cloud_Native.Web; // Ensure this matches your project namespace
using Codele.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & components.
builder.AddServiceDefaults();

// Configure SQL Server client
builder.AddSqlServerClient("codele");

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.WeatherForecastApi();
app.CodeleGameApi();

app.MapDefaultEndpoints();

app.Run();
