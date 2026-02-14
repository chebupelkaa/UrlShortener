using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Services;

namespace UrlShortener.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddRazorPages();
            services.AddScoped<IUrlShortenerService, UrlShortenerService>();
            services.AddScoped<IUrlValidationService, UrlValidationService>();
            services.AddScoped<ILinkService, LinkService>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            return services;
        }
    }
}
