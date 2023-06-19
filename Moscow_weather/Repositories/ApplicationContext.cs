using Microsoft.EntityFrameworkCore;
using Moscow_weather.Models;

namespace Moscow_weather.Repositories;

public class ApplicationContext : DbContext
{
    public DbSet<WeatherData> Weathers { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherData>()
            .HasIndex(x => x.Date).IsUnique();
    }
}