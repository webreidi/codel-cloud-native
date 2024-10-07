using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Codel_Cloud_Native.Web; // Ensure this matches your project namespace

using Aspire.Hosting;
//using Codele.ApiService;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var sqlserver = builder.AddSqlServer("sqlserver")
    // Mount the init scripts directory into the container.
    .WithBindMount("./sqlserverconfig", "/usr/config")
    // Mount the SQL scripts directory into the container so that the init scripts run.
    .WithBindMount("../Codel-Cloud-Native.ApiService/data/sqlserver", "/docker-entrypoint-initdb.d")
    // Run the custom entrypoint script on startup.
    .WithEntrypoint("/usr/config/entrypoint.sh")
    // Add the database to the application model so that it can be referenced by other resources.
    .AddDatabase("codele");

var apiService = builder.AddProject<Projects.Codele_ApiService>("apiservice")
                        .WithReference(sqlserver);

builder.AddProject<Projects.Codel_Cloud_Native_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
