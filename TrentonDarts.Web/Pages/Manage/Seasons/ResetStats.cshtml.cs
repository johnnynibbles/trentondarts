using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class ResetStatsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UpdateMatchStatsService _statsService;

    public ResetStatsModel(AppDbContext db, UpdateMatchStatsService statsService)
    {
        _db = db;
        _statsService = statsService;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var matchIds = await _db.WinterSeasonMatches
            .Where(m => m.SeasonId == Id)
            .Select(m => m.Id)
            .ToListAsync();

        foreach (var matchId in matchIds)
        {
            await _statsService.UpdateAsync(matchId);
        }

        return RedirectToPage("Show", new { leagueId = LeagueId, id = Id });
    }
}
