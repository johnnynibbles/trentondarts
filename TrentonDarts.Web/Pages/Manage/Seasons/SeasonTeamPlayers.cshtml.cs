using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class SeasonTeamPlayersModel : PageModel
{
    private readonly AppDbContext _db;

    public SeasonTeamPlayersModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SeasonTeamId { get; set; }

    public string TeamName { get; private set; } = "";
    public List<WinterSeasonTeamPlayer> TeamPlayers { get; private set; } = new();
    public List<Player> AvailablePlayers { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var st = await _db.WinterSeasonTeams.Include(s => s.Team).FirstOrDefaultAsync(s => s.Id == SeasonTeamId);
        if (st == null) return NotFound();

        TeamName = st.Team.Name;

        TeamPlayers = await _db.WinterSeasonTeamPlayers
            .Where(p => p.SeasonTeamId == SeasonTeamId)
            .Include(p => p.Player)
            .OrderBy(p => p.Player.LastName)
            .ToListAsync();

        var assignedIds = TeamPlayers.Select(p => p.PlayerId).ToHashSet();
        AvailablePlayers = await _db.Players
            .Where(p => p.LeagueId == LeagueId && !assignedIds.Contains(p.Id))
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int playerId, string role)
    {
        _db.WinterSeasonTeamPlayers.Add(new WinterSeasonTeamPlayer
        {
            LeagueId = LeagueId,
            SeasonId = Id,
            SeasonTeamId = SeasonTeamId,
            PlayerId = playerId,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int seasonTeamPlayerId)
    {
        var tp = await _db.WinterSeasonTeamPlayers.FindAsync(seasonTeamPlayerId);
        if (tp != null)
        {
            _db.WinterSeasonTeamPlayers.Remove(tp);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
