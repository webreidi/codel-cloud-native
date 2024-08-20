using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var mysql = builder.AddMySql("mysql");
var mysqldb = mysql.AddDatabase("mysqldb");

var apiService = builder.AddProject<Projects.Codel_Cloud_Native_ApiService>("apiservice")
                        .WithReference(mysqldb);

builder.AddProject<Projects.Codel_Cloud_Native_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.AddMySqlDataSource("MySqConnection");

builder.Build().Run();
