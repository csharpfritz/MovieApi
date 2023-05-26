using Microsoft.EntityFrameworkCore;

namespace MovieApi.Models;

public class MovieContext : DbContext
{
	public DbSet<Movie> Movies { get; set; }
	public DbSet<Genre> Genres { get; internal set; }

	public MovieContext(DbContextOptions<MovieContext> options)
			: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Movie>();
		modelBuilder.Entity<Movie>().HasKey(m => m.Id);

		modelBuilder.Entity<Genre>();
		modelBuilder.Entity<Genre>().HasKey(g => g.Id);

		// suggest a many-to-many relationship between Movie and Genre
		// https://docs.microsoft.com/en-us/ef/core/modeling/relationships#many-to-many
		modelBuilder.Entity<Movie>()
		.HasMany(m => m.Genre)
		.WithMany(g => g.Movies);


		base.OnModelCreating(modelBuilder);
	}
}
