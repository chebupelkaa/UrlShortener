using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IUrlShortenerService _shortenerService;

    public IndexModel(AppDbContext context, IUrlShortenerService shortenerService)
    {
        _context = context;
        _shortenerService = shortenerService;
    }


    [BindProperty]
    public string NewUrl { get; set; } = string.Empty;

    public List<ShortLink> ShortLinks { get; set; } = new();

    [FromQuery(Name = "editId")]
    public int? EditingId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;


    public int PageSize { get; set; } = 5;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int ReturnPageNumber { get; set; }

    public async Task OnGetAsync()
    {
        await LoadLinksAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadLinksAsync();
            return Page();
        }

        if (!NewUrl.StartsWith("http://") && !NewUrl.StartsWith("https://"))
        {
            NewUrl = "https://" + NewUrl;
        }

        string shortCode;
        do
        {
            shortCode = _shortenerService.GenerateUniqueCode();
        }
        while (await _context.ShortLinks.AnyAsync(l => l.ShortCode == shortCode));

        var link = new ShortLink
        {
            OriginalUrl = NewUrl,
            ShortCode = shortCode,
            CreatedAt = DateTime.UtcNow,
            ClickCount = 0
        };

        _context.ShortLinks.Add(link);
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnGetDeleteAsync(int id, [FromQuery] int pageNumber)
    {
        var link = await _context.ShortLinks.FindAsync(id);
        if (link != null)
        {
            _context.ShortLinks.Remove(link);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { pageNumber });
    }

    public async Task<IActionResult> OnPostUpdateAsync(int id, string newUrl, [FromQuery] int pageNumber)
    {
        var link = await _context.ShortLinks.FindAsync(id);
        if (link != null)
        {
            link.OriginalUrl = newUrl;
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { pageNumber });
    }

    public string Truncate(string url, int length)
    {
        if (string.IsNullOrEmpty(url)) return url;
        return url.Length <= length ? url : url.Substring(0, length - 3) + "...";
    }

    //private async Task LoadLinksAsync()
    //{
    //    ShortLinks = await _context.ShortLinks.OrderByDescending(l => l.CreatedAt).ToListAsync();

    //}
    private async Task LoadLinksAsync()
    {
        var query = _context.ShortLinks.OrderByDescending(l => l.CreatedAt);

        TotalCount = await query.CountAsync();
        ShortLinks = await query
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

}