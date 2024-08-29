using System.Net.Sockets;
using System.Reflection.Metadata;
using Aspire.Pomelo.EntityFrameworkCore.MySql;
using MySqlConnector;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.AddMySqlDataSource("words");
		builder.AddMySqlDbContext<MyDb1Context>("mysqldb");

		// Add service defaults & Aspire components.
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddProblemDetails();

		//	builder.Services.AddDbContextPool<MyDb1Context>(options =>
		//options.UseMySql(builder.Configuration.GetConnectionString("mysqldb"), new MySqlServerVersion(new Version()), sqlOptions =>
		//{
		//	sqlOptions.MigrationsAssembly("Codele.MigrationService");
		//	// Workaround for https://github.com/dotnet/aspire/issues/1023
		//	sqlOptions.ExecutionStrategy(c => new RetryingSqlServerRetryingExecutionStrategy(c));
		//}));
		//	builder.EnrichMySqlDbContext<MyDb1Context>(settings =>
		//		// Disable Aspire default retries as we're using a custom execution strategy
		//		settings.DisableRetry = true);


		builder.Services.AddProblemDetails();
		builder.Services.AddEndpointsApiExplorer();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.UseExceptionHandler();

		var summaries = new[]
		{
	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
		string[] words = [];

		app.MapGet("/sample-data", async (MySqlConnection db) =>
		{
			using var command = db.CreateCommand();
			command.CommandText = "SELECT Id, word FROM words";
			using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				words.Append(reader.GetString(1));
			}
			//const string sql = """
   //             SELECT Id, word
   //             FROM words
   //             """;
			//var connection = await db.OpenAsync();
			//var answers = await db.QueryAsync<Words>(sql);
			//foreach (var word in answers)
			//{
			//	words.Append(word.Answer);
			//}

			var answer = Enumerable.Range(1, 14).Select(index =>
					new SampleData
					(
						words[Random.Shared.Next(words.Length)]
					))
					.ToArray();
			return answer;

		});

		//app.MapGet("/sample-data", async(MyDb1Context context) =>
		//{
		//	var answers = await context.Words.ToListAsync();
		//	foreach (var word in answers)
		//	{
		//		words.Append(word.Answer);
		//	}
		//	var answer = Enumerable.Range(1, 14).Select(index =>
		//			new SampleData
		//			(
		//				words[Random.Shared.Next(words.Length)]
		//			))
		//			.ToArray();
		//	return answer;
		//});

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
				var context = scope.ServiceProvider.GetRequiredService<MyDb1Context>();
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


