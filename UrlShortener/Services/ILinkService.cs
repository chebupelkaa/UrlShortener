using UrlShortener.Models;

namespace UrlShortener.Services
{
    public interface ILinkService
    {
        Task<(List<ShortLink> Links, int TotalCount)> GetPagedLinksAsync(int page, int pageSize);
        Task<ShortLink> CreateLinkAsync(string originalUrl);
        Task<bool> DeleteLinkAsync(int id);
        Task<bool> UpdateLinkAsync(int id, string newUrl);
        int CalculateTotalPages(int totalCount, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<ShortLink?> GetLinkByIdAsync(int id);
        int CorrectPageNumber(int page, int totalCount, int pageSize);
    }
}
