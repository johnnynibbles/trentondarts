using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage.Documents;

public class UploadModel : PageModel
{
    private readonly BrowsableFileService _files;

    public UploadModel(BrowsableFileService files) => _files = files;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public IFormFile? UploadFile { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await _files.UploadDocumentAsync(UploadFile!, Title);
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
