using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ShortLink> ShortLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // индексы созданы для ускорения поиска и выборки данных
            modelBuilder.Entity<ShortLink>()
                .HasIndex(l => l.ShortCode)
                .IsUnique();

            modelBuilder.Entity<ShortLink>()
                .HasIndex(l => l.CreatedAt);
        }
    
    }
}
        