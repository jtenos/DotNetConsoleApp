using Microsoft.EntityFrameworkCore;

namespace DotNetConsoleApp;

internal class MyDatabaseContext(DbContextOptions<MyDatabaseContext> options)
	: DbContext(options)
{
	public DbSet<Foo> Foos { get; set; }
	public DbSet<Bar> Bars { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Bar>()
			.HasOne(bar => bar.Foo)
			.WithMany(foo => foo.Bars)
			.HasForeignKey(bar => bar.FooID);
	}
}

public class Foo
{
	public int ID { get; set; }
	public string Name { get; set; } = default!;
	public List<Bar> Bars { get; set; } = [];
}

public class  Bar
{
    public int ID { get; set; }
	public int FooID { get; set; }
	public string Name { get; set; } = default!;
	public Foo Foo { get; set; } = default!;
}
