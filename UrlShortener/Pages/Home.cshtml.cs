using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Pages;

public class HomeModel : PageModel
{
    private readonly ILinkService _linkService;
    private readonly IUrlValidationService _urlValidationService;

    public HomeModel(ILinkService linkService, IUrlValidationService urlValidationService)
    {
        _linkService = linkService;
        _urlValidationService = urlValidationService;
    }

    [BindProperty]
    public string NewUrl { get; set; } = string.Empty;

    public List<ShortLink> ShortLinks { get; set; } = new();

    //для переключения на режим редактирования
    [FromQuery(Name = "editId")]
    public int? EditingId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalCount { get; set; }
    public int TotalPages => _linkService.CalculateTotalPages(TotalCount, PageSize);

    private async Task<IActionResult> ReloadPage()
    {
        await OnGetAsync();
        return Page();
    }
    public async Task OnGetAsync()
    {
        var result = await _linkService.GetPagedLinksAsync(PageNumber, PageSize);
        ShortLinks = result.Links;
        TotalCount = result.TotalCount;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return await ReloadPage();

        //чтобы не выдавать ошибку если пользователь введёт ссылку без http или https
        NewUrl = _urlValidationService.NormalizeUrl(NewUrl);

        if (!_urlValidationService.IsValidUrl(NewUrl))
        {
            ModelState.AddModelError("NewUrl", "Некорректный формат URL");
            return await ReloadPage();
        }

        await _linkService.CreateLinkAsync(NewUrl);
        return RedirectToPage(new { pageNumber = 1 });
    }

    public async Task<IActionResult> OnGetDeleteAsync(int id, [FromQuery] int pageNumber)
    {
        await _linkService.DeleteLinkAsync(id);
        //пересчёт страниц, ведь их количество изменится
        var linksTotalCount = await _linkService.GetTotalCountAsync();
        pageNumber = _linkService.CorrectPageNumber(pageNumber, linksTotalCount, PageSize);
        return RedirectToPage(new { pageNumber });
    }

    public async Task<IActionResult> OnPostUpdateAsync(int id, string newUrl, [FromQuery] int pageNumber)
    {
        newUrl = _urlValidationService.NormalizeUrl(newUrl);

        if (!_urlValidationService.IsValidUrl(newUrl))
        {
            return RedirectToPage(new { pageNumber, editId = id });
        }

        await _linkService.UpdateLinkAsync(id, newUrl);
        return RedirectToPage(new { pageNumber });
    }

    public async Task<IActionResult> OnGetClickCountAsync(int id)
    {
        var link = await _linkService.GetLinkByIdAsync(id);
        if (link == null) return NotFound();
        return new JsonResult(new { count = link.ClickCount });
    }

}