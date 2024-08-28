using System.Net.Sockets;
using System.Reflection.Metadata;
using Aspire.Pomelo.EntityFrameworkCore.MySql;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.AddMySqlDbContext<DataContext>("mysqldb");

		// Add service defaults & Aspire components.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddProblemDetails();

		builder.AddMySqlDataSource("mysqldb");

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.UseExceptionHandler();

		var summaries = new[]
		{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

		List<Words> GetWords = [];

		string[] words = [];

		async Task OnInitializedAsync(DataContext dataContext) => GetWords = await dataContext.Answers.ToListAsync();
		foreach (var word in GetWords)
		{
			words.Append(word.Answer);
		}

		app.MapGet("/sample-data", () =>
		{
			var answer = Enumerable.Range(1, 14).Select(index =>
					new SampleData
					(
						words[Random.Shared.Next(words.Length)]
					))
					.ToArray();
			return answer;
		});

		app.MapGet("/weatherforecast", () =>
		{
			var forecast = Enumerable.Range(1, 5).Select(index =>
				new WeatherForecast
				(
					DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
					Random.Shared.Next(-20, 55),
					summaries[Random.Shared.Next(summaries.Length)]
				))
				.ToArray();
			return forecast;
		});

		app.MapDefaultEndpoints();

		if (app.Environment.IsDevelopment())
		{
			using (var scope = app.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<DataContext>();
				context.Database.EnsureCreated();
			}
		} else
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			// The default HSTS value is 30 days.
			// You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.Run();

		if (app.Environment.IsDevelopment())
		{
			using (var scope = app.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<DataContext>();
				context.Database.EnsureCreated();
			}
		} else
		{
			app.UseExceptionHandler("/Error", createScopeForErrors: true);
			// The default HSTS value is 30 days.
			// You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
	}
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record SampleData(string answer);


