using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var password = builder.AddParameter("mysql-password", secret: true);

var mysql = builder.AddMySql("test-mysql")
		.WithEnvironment("MYSQL_DATABASE", "words")
		.WithBindMount("../DatabaseContainers.ApiService/data/mysql", "/docker-entrypoint-initdb.d")
		.AddDatabase("words");

builder.AddProject<Projects.Codele_MigrationService>("migration")
	   .WithReference(mysql);

var apiService = builder.AddProject<Projects.Codele_ApiService>("apiservice")
                        .WithReference(mysql);

builder.AddProject<Projects.Codel_Cloud_Native_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
