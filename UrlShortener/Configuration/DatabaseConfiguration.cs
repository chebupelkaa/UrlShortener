using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

namespace UrlShortener.Configuration
{
    public static class DatabaseConfiguration
    {
        public static async Task MigrateDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
    }
}
