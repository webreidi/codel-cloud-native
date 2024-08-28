// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore;
using Codele.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.Services.AddOpenTelemetry()
	.WithTracing(tracing => tracing.AddSource(ApiDbInitializer.ActivitySourceName));

builder.Services.AddDbContextPool<MyDb1Context>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("mysqldb"), new MySqlServerVersion(new Version()), sqlOptions =>
	{
		sqlOptions.MigrationsAssembly("Codele.MigrationService");
		// Workaround for https://github.com/dotnet/aspire/issues/1023
		sqlOptions.ExecutionStrategy(c => new RetryingSqlServerRetryingExecutionStrategy(c));
	}));
builder.EnrichMySqlDbContext<MyDb1Context>(settings =>
	// Disable Aspire default retries as we're using a custom execution strategy
	settings.DisableRetry = true);

var app = builder.Build();

app.Run();
