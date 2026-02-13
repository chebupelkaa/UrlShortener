using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.MapGet("/{shortCode}", async (string shortCode, AppDbContext db) =>
{
    var link = await db.ShortLinks.FirstOrDefaultAsync(l => l.ShortCode == shortCode);

    if (link == null)
    {
        return Results.NotFound("Ссылка не найдена");
    }

    await db.ShortLinks
        .Where(l => l.Id == link.Id)
        .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.ClickCount, p => p.ClickCount + 1));

    return Results.Redirect(link.OriginalUrl);
});

app.Run();
