using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using MatchType = TrentonDarts.Web.Data.Entities.MatchType;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class WeekMatchesModel : PageModel
{
    private readonly AppDbContext _db;

    public WeekMatchesModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int WeekId { get; set; }

    public DateTime WeekDate { get; private set; }
    public List<WinterSeasonMatch> Matches { get; private set; } = new();
    public List<WinterSeasonTeam> Teams { get; private set; } = new();
    public List<MatchType> MatchTypes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var week = await _db.WinterSeasonWeeks.FindAsync(WeekId);
        if (week == null) return NotFound();

        WeekDate = week.Date;

        Matches = await _db.WinterSeasonMatches
            .Where(m => m.WeekId == WeekId)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.MatchType)
            .ToListAsync();

        Teams = await _db.WinterSeasonTeams
            .Where(t => t.SeasonId == Id)
            .Include(t => t.Team)
            .OrderBy(t => t.Team.Name)
            .ToListAsync();

        MatchTypes = await _db.MatchTypes.OrderBy(m => m.Name).ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int awayTeamId, int homeTeamId, string? division, int matchTypeId)
    {
        _db.WinterSeasonMatches.Add(new WinterSeasonMatch
        {
            SeasonId = Id,
            WeekId = WeekId,
            MatchTypeId = matchTypeId,
            Division = division,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            LeagueId = LeagueId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int matchId)
    {
        var match = await _db.WinterSeasonMatches.FindAsync(matchId);
        if (match != null)
        {
            _db.WinterSeasonMatches.Remove(match);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
