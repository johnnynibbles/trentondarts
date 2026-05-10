using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Web.Pages.Matches;

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly MatchRepository _matchRepo;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ShowModel(AppDbContext db, MatchRepository matchRepo)
    {
        _db = db;
        _matchRepo = matchRepo;
    }

    public WinterSeason Season { get; private set; } = null!;
    public WinterSeasonMatch Match { get; private set; } = null!;
    public Dictionary<string, string> MatchesThisWeek { get; private set; } = new();
    public string SelectedMatchRoute { get; private set; } = "";
    public string MatchResultsJson { get; private set; } = "{}";
    public string HomPlayersJson { get; private set; } = "[]";
    public string AwayPlayersJson { get; private set; } = "[]";
    public bool IsBoardMember { get; private set; }

    public async Task<IActionResult> OnGetAsync(int seasonId, int id)
    {
        Season = await _db.WinterSeasons.FindAsync(seasonId)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent);
        if (Season == null) return NotFound();

        Match = await _db.WinterSeasonMatches
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Week)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (Match == null) return NotFound();

        IsBoardMember = await IsBoardMemberAsync();

        // Build the "matches this week" dropdown
        var weekMatches = await _db.WinterSeasonMatches
            .Where(m => m.WeekId == Match.WeekId)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .Include(m => m.Week)
            .OrderBy(m => m.Division)
            .ToListAsync();

        foreach (var m in weekMatches)
        {
            var url = $"/season/{seasonId}/match/{m.Id}";
            var label = $"Div {m.Division} ({m.Week.Date:MM-dd-yyyy}) {m.AwayTeam.Name} @ {m.HomeTeam.Name}";
            MatchesThisWeek[url] = label;
        }
        SelectedMatchRoute = $"/season/{seasonId}/match/{id}";

        // Build match result snapshot JSON for the KO viewmodel
        var matchResult = await _matchRepo.GetMatchResultsFromMatchAsync(Match);
        var snapshot = matchResult.GetSnapshot();
        MatchResultsJson = JsonSerializer.Serialize(snapshot, JsonOpts);

        // Build player lists
        var homeSeasonTeam = await _db.WinterSeasonTeams
            .Where(st => st.TeamId == Match.HomeTeamId && st.SeasonId == Season.Id)
            .Include(st => st.TeamPlayers).ThenInclude(tp => tp.Player)
            .FirstOrDefaultAsync();

        var awaySeasonTeam = await _db.WinterSeasonTeams
            .Where(st => st.TeamId == Match.AwayTeamId && st.SeasonId == Season.Id)
            .Include(st => st.TeamPlayers).ThenInclude(tp => tp.Player)
            .FirstOrDefaultAsync();

        HomPlayersJson = BuildPlayerListJson(homeSeasonTeam);
        AwayPlayersJson = BuildPlayerListJson(awaySeasonTeam);

        return Page();
    }

    private string BuildPlayerListJson(WinterSeasonTeam? seasonTeam)
    {
        if (seasonTeam == null) return "[]";
        var players = seasonTeam.TeamPlayers
            .Where(tp => tp.Player != null)
            .Select(tp => new GamePlayer { Id = tp.Player.Id, Name = tp.Player.Name })
            .OrderBy(p => p.Name)
            .ToList();
        return JsonSerializer.Serialize(players, JsonOpts);
    }

    private async Task<bool> IsBoardMemberAsync()
    {
        if (User.IsInRole("Admin") || User.IsInRole("Owner")) return true;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
