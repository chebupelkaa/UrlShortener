using UrlShortener.Data;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Endpoints
{
    public static class RedirectEndpoints
    {
        public static void MapRedirectEndpoints(this WebApplication app)
        {
            app.MapGet("/{code}", async (string code, AppDbContext db) =>
            {
                // AsNoTracking() ускоряет чтение, так как нам не нужно отслеживать изменения сущности в памяти
                var link = await db.ShortLinks.AsNoTracking().FirstOrDefaultAsync(l => l.ShortCode == code);

                if (link == null) return Results.NotFound();

                // обновление на уровне бд для оптимизации запросов
                // используется метод ExecuteSqlInterpolatedAsync, используемый для выполнения сырых SQL-команд (INSERT, UPDATE, DELETE) с безопасной интерполяцией строк
                await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE ShortLinks SET ClickCount = ClickCount + 1 WHERE Id = {link.Id}");

                return Results.Redirect(link.OriginalUrl);
            });
        }
    }
}
