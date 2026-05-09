using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Navigation;

public class CreateItemModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateItemModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int GroupId { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public NavItemType ItemType { get; set; }

    [BindProperty]
    public string? UrlTemplate { get; set; }

    [BindProperty]
    public int? BrowsableFileId { get; set; }

    [BindProperty]
    public bool IsHeader { get; set; }

    [BindProperty]
    public bool IsSeparator { get; set; }

    public List<SelectListItem> DocumentOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadDocumentOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDocumentOptionsAsync();
            return Page();
        }

        var maxOrder = _db.NavGroupItems.Any(i => i.NavGroupId == GroupId)
            ? _db.NavGroupItems.Where(i => i.NavGroupId == GroupId).Max(i => i.SortOrder)
            : 0;

        _db.NavGroupItems.Add(new NavGroupItem
        {
            NavGroupId = GroupId,
            Title = Title,
            ItemType = ItemType,
            UrlTemplate = ItemType != NavItemType.Document ? UrlTemplate : null,
            BrowsableFileId = ItemType == NavItemType.Document ? BrowsableFileId : null,
            IsHeader = IsHeader,
            IsSeparator = IsSeparator,
            SortOrder = maxOrder + 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }

    private async Task LoadDocumentOptionsAsync()
    {
        var files = await _db.BrowsableFiles
            .OrderBy(f => f.Title)
            .ToListAsync();
        DocumentOptions = files.Select(f => new SelectListItem(
            f.Title.Length > 0 ? f.Title : f.FileName,
            f.Id.ToString())).ToList();
    }
}
