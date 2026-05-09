using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage.Documents;

public class DocumentsIndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly BrowsableFileService _files;

    public DocumentsIndexModel(AppDbContext db, BrowsableFileService files)
    {
        _db = db;
        _files = files;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<BrowsableFile> Documents { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Documents = await _db.BrowsableFiles
            .Where(f => f.Category == "document")
            .OrderBy(f => f.Title)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _files.DeleteAsync(id);
        return RedirectToPage(new { leagueId = LeagueId });
    }
}
