using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.MatchTypes.Rules;

public class RulesIndexModel : PageModel
{
    private readonly AppDbContext _db;

    public RulesIndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int MatchTypeId { get; set; }

    public string MatchTypeName { get; set; } = string.Empty;
    public List<MatchTypeGameRule> Rules { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var mt = await _db.MatchTypes
            .Include(m => m.GameRules)
            .FirstOrDefaultAsync(m => m.Id == MatchTypeId);
        if (mt == null) return NotFound();

        MatchTypeName = mt.Name;
        Rules = mt.GameRules.OrderBy(r => r.OrderId).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var rule = await _db.MatchTypeGameRules.FindAsync(id);
        if (rule != null)
        {
            rule.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { leagueId = LeagueId, matchTypeId = MatchTypeId });
    }

    public async Task<IActionResult> OnPostMoveUpAsync(int id)
    {
        await MoveRule(id, up: true);
        return RedirectToPage(new { leagueId = LeagueId, matchTypeId = MatchTypeId });
    }

    public async Task<IActionResult> OnPostMoveDownAsync(int id)
    {
        await MoveRule(id, up: false);
        return RedirectToPage(new { leagueId = LeagueId, matchTypeId = MatchTypeId });
    }

    private async Task MoveRule(int id, bool up)
    {
        var rules = await _db.MatchTypeGameRules
            .Where(r => r.MatchTypeId == MatchTypeId && r.DeletedAt == null)
            .OrderBy(r => r.OrderId)
            .ToListAsync();

        var idx = rules.FindIndex(r => r.Id == id);
        if (idx < 0) return;
        var swapIdx = up ? idx - 1 : idx + 1;
        if (swapIdx < 0 || swapIdx >= rules.Count) return;

        (rules[idx].OrderId, rules[swapIdx].OrderId) = (rules[swapIdx].OrderId, rules[idx].OrderId);
        await _db.SaveChangesAsync();
    }
}
