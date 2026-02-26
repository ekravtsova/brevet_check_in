using Microsoft.EntityFrameworkCore;

namespace brevet_tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherForecastEntity> WeatherForecasts => Set<WeatherForecastEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecastEntity>(entity =>
            {
                entity.ToTable("weather_forecasts");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Date).IsRequired();
                entity.Property(x => x.TemperatureC).IsRequired();
                entity.Property(x => x.Summary).HasMaxLength(100).IsRequired();
            });
        }
    }
}
