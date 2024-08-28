using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var password = builder.AddParameter("mysql-password", secret: true);

var mysql = builder.AddMySql("test-mysql", password)
            .AddDatabase("words");

var apiService = builder.AddProject<Projects.Codele_ApiService>("apiservice")
                        .WithReference(mysql);

builder.AddProject<Projects.Codel_Cloud_Native_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
