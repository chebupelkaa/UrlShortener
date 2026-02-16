using UrlShortener.Configuration;
using UrlShortener.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureApplication(builder.Configuration);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRedirectEndpoints();

app.MapRazorPages();

app.Run();
