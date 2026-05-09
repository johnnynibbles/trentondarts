using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage.Documents;

public class UploadModel : PageModel
{
    private readonly BrowsableFileService _files;
    private readonly AppDbContext _db;

    public UploadModel(BrowsableFileService files, AppDbContext db)
    {
        _files = files;
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase letters, numbers, and hyphens only (e.g. league-rules).")]
    public string? Slug { get; set; }

    [BindProperty]
    [Required]
    public IFormFile? UploadFile { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(Slug) &&
            await _db.BrowsableFiles.AnyAsync(f => f.Slug == Slug))
        {
            ModelState.AddModelError(nameof(Slug), "This slug is already in use.");
        }

        if (!ModelState.IsValid) return Page();

        await _files.UploadDocumentAsync(UploadFile!, Title, Slug);
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
