using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class SeasonTeamInput
{
    public int TeamId { get; set; }
    public string? PreSeasonDiv { get; set; }
    public string? RegularSeasonDiv { get; set; }
}

public class SeasonTeamCreateModel : PageModel
{
    private readonly AppDbContext _db;

    public SeasonTeamCreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public SeasonTeamInput Input { get; set; } = new();

    public List<Team> Teams { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Teams = await _db.Teams
            .Where(t => t.LeagueId == LeagueId)
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _db.WinterSeasonTeams.Add(new WinterSeasonTeam
        {
            LeagueId = LeagueId,
            SeasonId = Id,
            TeamId = Input.TeamId,
            PreSeasonDiv = Input.PreSeasonDiv,
            RegularSeasonDiv = Input.RegularSeasonDiv,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Show", new { leagueId = LeagueId, id = Id });
    }
}
