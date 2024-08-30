using Codele.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

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





