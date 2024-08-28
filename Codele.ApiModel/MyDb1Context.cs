using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class MyDb1Context(DbContextOptions<MyDb1Context> options) : DbContext(options)
{
	public DbSet<Words> Words
	{
		get; set;
	}
}

public class Words
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Answer { get; set; }
}