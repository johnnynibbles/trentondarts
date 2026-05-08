using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<WinterSeason> Seasons { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Seasons = await _db.WinterSeasons
            .Where(s => s.LeagueId == LeagueId)
            .OrderByDescending(s => s.StartYear)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var season = await _db.WinterSeasons.FindAsync(id);
        if (season != null)
        {
            season.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
