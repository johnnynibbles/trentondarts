using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Players;

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PlayerHistoryService _historyService;
    private readonly StatsService _statsService;

    public ShowModel(AppDbContext db, PlayerHistoryService historyService, StatsService statsService)
    {
        _db = db;
        _historyService = historyService;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public Player Player { get; private set; } = null!;
    public Team? PlayerTeam { get; private set; }
    public int PlayerTeamId { get; private set; }
    public List<PlayerGameHistory> GameHistories { get; private set; } = new();
    public List<PlayerStat> TeamPlayerStats { get; private set; } = new();
    public bool IsBoardMember { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, [FromQuery] string? season)
    {
        if (season != null && int.TryParse(season, out var seasonId))
            Season = await _db.WinterSeasons.FindAsync(seasonId) ?? await GetCurrentSeasonAsync();
        else
            Season = await GetCurrentSeasonAsync();

        Player = await _db.Players.FindAsync(id);
        if (Player == null) return NotFound();

        IsBoardMember = await IsBoardMemberAsync();

        // Find which team this player is on this season
        var seasonTeam = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == Season.Id)
            .Include(st => st.Team)
            .Include(st => st.TeamPlayers)
            .FirstOrDefaultAsync(st => st.TeamPlayers.Any(tp => tp.PlayerId == id));

        PlayerTeam = seasonTeam?.Team;
        PlayerTeamId = seasonTeam?.TeamId ?? 0;

        GameHistories = await _historyService.GetPlayerGameHistoryAsync(Season.Id, id);

        if (PlayerTeamId > 0)
        {
            var allStats = await _statsService.GetPlayerStatsForSeasonPartAsync(Season.Id, "whole", "");
            TeamPlayerStats = allStats.Where(p => p.TeamId == PlayerTeamId).ToList();
        }

        return Page();
    }

    private async Task<WinterSeason> GetCurrentSeasonAsync() =>
        await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
        ?? await _db.WinterSeasons.OrderByDescending(s => s.EndYear).FirstAsync();

    private async Task<bool> IsBoardMemberAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
