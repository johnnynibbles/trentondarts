using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using MatchType = TrentonDarts.Web.Data.Entities.MatchType;

namespace TrentonDarts.Web.Pages.Manage.MatchTypes;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<MatchType> MatchTypes { get; set; } = new();

    public async Task OnGetAsync()
    {
        MatchTypes = await _db.MatchTypes
            .Include(m => m.GameRules)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var mt = await _db.MatchTypes.Include(m => m.GameRules).FirstOrDefaultAsync(m => m.Id == id);
        if (mt != null)
        {
            var now = DateTime.UtcNow;
            mt.DeletedAt = now;
            foreach (var rule in mt.GameRules)
                rule.DeletedAt = now;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { leagueId = LeagueId });
    }
}
