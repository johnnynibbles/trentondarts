using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;

    public ShowModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public WinterSeason? Season { get; private set; }
    public List<WinterSeasonTeam> SeasonTeams { get; private set; } = new();
    public List<WinterSeasonWeek> Weeks { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Season = await _db.WinterSeasons.FindAsync(Id);
        if (Season == null) return NotFound();

        SeasonTeams = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == Id)
            .Include(st => st.Team)
            .OrderBy(st => st.Team.Name)
            .ToListAsync();

        Weeks = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == Id)
            .Include(w => w.Matches)
            .OrderBy(w => w.Date)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteTeamAsync(int seasonTeamId)
    {
        var st = await _db.WinterSeasonTeams.FindAsync(seasonTeamId);
        if (st != null)
        {
            _db.WinterSeasonTeams.Remove(st);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteWeekAsync(int weekId)
    {
        var week = await _db.WinterSeasonWeeks.FindAsync(weekId);
        if (week != null)
        {
            _db.WinterSeasonWeeks.Remove(week);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
