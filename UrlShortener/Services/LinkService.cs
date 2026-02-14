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

        //данная функция нужна для загрузки только необходимого кол-ва записей на странице (чтобы не загружать все записи сразу)
        public async Task<(List<ShortLink> Links, int TotalCount)> GetPagedLinksAsync(int page, int pageSize)
        {
            //OrderByDescending по дате чтобы записи было видно на первой странице
            var query = _context.ShortLinks.OrderByDescending(l => l.CreatedAt);

            var totalCount = await query.CountAsync();
            //проверки (если в url попадет неправильно число page)
            page = CorrectPageNumber(page, totalCount, pageSize);

            //только нужное кол-во записей на странице
            var links = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (links, totalCount);
        }

        public async Task<ShortLink> CreateLinkAsync(string originalUrl)
        {
            //проверка уникальности shortCode в бд, если есть повтор, то сгенерировать заново
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

        public int CorrectPageNumber(int page, int totalCount, int pageSize)
        {
            var totalPages = CalculateTotalPages(totalCount, pageSize);

            if (page > totalPages && totalPages > 0) return totalPages;
            if (page < 1) return 1;

            return page;
        }
    }
}
