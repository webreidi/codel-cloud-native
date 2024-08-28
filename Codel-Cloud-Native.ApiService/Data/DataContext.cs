namespace Codele.ApiService.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    
    protected readonly IConfiguration Configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = Configuration.GetConnectionString("MySqConnection");    
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));    
    }

    public DbSet<Words> Answers { get; set; }


}
