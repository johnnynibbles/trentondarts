using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Sponsors;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<Sponsor> Sponsors { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Sponsors = await _db.Sponsors
            .Where(s => s.LeagueId == LeagueId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var sponsor = await _db.Sponsors.FindAsync(id);
        if (sponsor != null)
        {
            sponsor.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
