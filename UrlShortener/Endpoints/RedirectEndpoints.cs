using UrlShortener.Data;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Endpoints
{
    public static class RedirectEndpoints
    {
        public static void MapRedirectEndpoints(this WebApplication app)
        {
            app.MapGet("/{shortCode}", async (string shortCode, AppDbContext db) =>
            {
                // использование AsNoTracking() чтобы данные не помещались в кэш, ускоряет чтение
                var link = await db.ShortLinks.AsNoTracking().FirstOrDefaultAsync(l => l.ShortCode == shortCode);

                if (link == null) return Results.NotFound();

                // обновление на уровне бд для оптимизации запросов
                // используется метод ExecuteSqlInterpolatedAsync, используемый для выполнения сырых SQL-команд (INSERT, UPDATE, DELETE) с безопасной интерполяцией строк
                await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE ShortLinks SET ClickCount = ClickCount + 1 WHERE Id = {link.Id}");

                return Results.Redirect(link.OriginalUrl);
            });
        }
    }
}
