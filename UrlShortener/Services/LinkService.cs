using UrlShortener.Data;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Services
{
    public class LinkService : ILinkService
    {
        private readonly AppDbContext _context;
        private readonly IUrlShortenerService _codeGenerator;

        public LinkService(AppDbContext context, IUrlShortenerService codeGenerator)
        {
            _context = context;
            _codeGenerator = codeGenerator;
        }

        public async Task<(List<ShortLink> Links, int TotalCount)> GetPagedLinksAsync(int page, int pageSize)
        {
            var query = _context.ShortLinks.OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();

            var totalPages = CalculateTotalPages(totalCount, pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;
            if (page < 1) page = 1;

            var links = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (links, totalCount);
        }

        public async Task<ShortLink> CreateLinkAsync(string originalUrl)
        {
            string shortCode;
            do
            {
                shortCode = _codeGenerator.GenerateUniqueCode();
            }
            while (await _context.ShortLinks.AnyAsync(l => l.ShortCode == shortCode));

            var link = new ShortLink
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            _context.ShortLinks.Add(link);
            await _context.SaveChangesAsync();

            return link;
        }

        public async Task<bool> DeleteLinkAsync(int id)
        {
            var link = await _context.ShortLinks.FindAsync(id);
            if (link == null) return false;

            _context.ShortLinks.Remove(link);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLinkAsync(int id, string newUrl)
        {
            var link = await _context.ShortLinks.FindAsync(id);
            if (link == null) return false;

            link.OriginalUrl = newUrl;
            await _context.SaveChangesAsync();
            return true;
        }

        public int CalculateTotalPages(int totalCount, int pageSize)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.ShortLinks.CountAsync();
        }
    }
}
