using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Seasons;

public class StatsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly StatsService _statsService;

    public static readonly Dictionary<string, string> Leaderboards = new()
    {
        ["overall"] = "Overall", ["singles"] = "Singles", ["doubles"] = "Doubles",
        ["801"] = "801", ["cricket"] = "Cricket", ["01"] = "01",
        ["singles-301"] = "Singles 301", ["singles-cricket"] = "Singles Cricket",
        ["doubles-501"] = "Doubles 501", ["doubles-cricket"] = "Doubles Cricket",
        ["triples-801"] = "Triples 801"
    };

    public StatsModel(AppDbContext db, StatsService statsService)
    {
        _db = db;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public SeasonPart? SeasonPart { get; private set; }
    public string Division { get; private set; } = "";
    public string Leaderboard { get; private set; } = "";
    public Dictionary<string, string> Divisions { get; private set; } = new();
    public List<TeamStat> TeamStats { get; private set; } = new();
    public List<PlayerStat> PlayerStats { get; private set; } = new();

    public async Task OnGetAsync(int seasonId,
        [FromQuery] SeasonPart? seasonPart = null,
        [FromQuery] string division = "",
        [FromQuery] string leaderboard = "")
    {
        Season = await _db.WinterSeasons.FindAsync(seasonId)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
            ?? throw new InvalidOperationException("Season not found");

        SeasonPart = seasonPart;
        Division = division;
        Leaderboard = leaderboard;

        Divisions = await GetDivisionsAsync(seasonId);
        TeamStats = await _statsService.GetTeamStatsForSeasonPartAsync(seasonId, seasonPart, division);
        PlayerStats = await _statsService.GetPlayerStatsForSeasonPartAsync(seasonId, seasonPart, "");
    }

    private async Task<Dictionary<string, string>> GetDivisionsAsync(int seasonId)
    {
        var pre = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == seasonId && st.PreSeasonDiv != null && st.PreSeasonDiv != "")
            .Select(st => st.PreSeasonDiv!)
            .Distinct().OrderBy(d => d).ToListAsync();

        var regular = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == seasonId && st.RegularSeasonDiv != null && st.RegularSeasonDiv != "")
            .Select(st => st.RegularSeasonDiv!)
            .Distinct().OrderBy(d => d).ToListAsync();

        return pre.Union(regular).Distinct().OrderBy(d => d)
            .ToDictionary(d => d, d => d);
    }
}
