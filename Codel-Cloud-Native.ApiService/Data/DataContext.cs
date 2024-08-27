using System;

namespace Codele.ApiService.Data;

public class DataContext : DbContext
{
    
    protected readonly IConfiguration Configuration;

    public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = Configuration.GetConnectionString("MySqConnection");    
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));    
    }

    public DbSet<Words> Answers { get; set; }


}
