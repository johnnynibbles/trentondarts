using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Navigation;

public class EditItemModel : PageModel
{
    private readonly AppDbContext _db;

    public EditItemModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int GroupId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int ItemId { get; set; }

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

    public async Task<IActionResult> OnGetAsync()
    {
        var item = await _db.NavGroupItems.FindAsync(ItemId);
        if (item == null) return NotFound();

        Title = item.Title;
        ItemType = item.ItemType;
        UrlTemplate = item.UrlTemplate;
        BrowsableFileId = item.BrowsableFileId;
        IsHeader = item.IsHeader;
        IsSeparator = item.IsSeparator;

        await LoadDocumentOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDocumentOptionsAsync();
            return Page();
        }

        var item = await _db.NavGroupItems.FindAsync(ItemId);
        if (item == null) return NotFound();

        item.Title = Title;
        item.ItemType = ItemType;
        item.UrlTemplate = ItemType != NavItemType.Document ? UrlTemplate : null;
        item.BrowsableFileId = ItemType == NavItemType.Document ? BrowsableFileId : null;
        item.IsHeader = IsHeader;
        item.IsSeparator = IsSeparator;
        item.UpdatedAt = DateTime.UtcNow;

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
