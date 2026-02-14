namespace UrlShortener.Services
{
    public interface IUrlValidationService
    {
        string NormalizeUrl(string url);
        bool IsValidUrl(string url);
    }
}
