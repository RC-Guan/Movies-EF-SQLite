using Microsoft.EntityFrameworkCore;
using MovieDatabase.Models;

namespace MovieDatabase.Infrastructure;

public class MovieDb : DbContext
{
    public MovieDb(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; } = null!;
}