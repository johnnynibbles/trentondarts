using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Seasons;

public class LeaderboardModel : PageModel
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

    public LeaderboardModel(AppDbContext db, StatsService statsService)
    {
        _db = db;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public SeasonPart? SeasonPart { get; private set; }
    public string Division { get; private set; } = "";
    public string Leaderboard { get; private set; } = "overall";
    public Dictionary<string, string> Divisions { get; private set; } = new();
    public List<PlayerStat> PlayerStats { get; private set; } = new();

    public async Task OnGetAsync(int seasonId,
        [FromQuery] SeasonPart? seasonPart = null,
        [FromQuery] string division = "",
        [FromQuery] string leaderboard = "overall")
    {
        Season = await _db.WinterSeasons.FindAsync(seasonId)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
            ?? throw new InvalidOperationException("Season not found");

        SeasonPart = seasonPart;
        Division = division;
        Leaderboard = leaderboard;

        Divisions = await GetDivisionsAsync(seasonId);
        PlayerStats = await _statsService.GetPlayerStatsForSeasonPartAsync(seasonId, seasonPart, division);

        // Sort by leaderboard
        PlayerStats = leaderboard switch
        {
            "singles" => PlayerStats.OrderByDescending(p => p.SinglesWin).ThenByDescending(p => p.GamesPlayed).ToList(),
            "doubles" => PlayerStats.OrderByDescending(p => p.DoublesWin).ThenByDescending(p => p.GamesPlayed).ToList(),
            "801" => PlayerStats.OrderByDescending(p => p.Eight01Win).ThenByDescending(p => p.GamesPlayed).ToList(),
            "cricket" => PlayerStats.OrderByDescending(p => p.CricketWin).ThenByDescending(p => p.GamesPlayed).ToList(),
            "01" => PlayerStats.OrderByDescending(p => p.Oh1Win).ThenByDescending(p => p.GamesPlayed).ToList(),
            "singles-301" => PlayerStats.OrderByDescending(p => p.Singles301Win).ThenByDescending(p => p.Game301.Played).ToList(),
            "singles-cricket" => PlayerStats.OrderByDescending(p => p.SinglesCricketWin).ThenByDescending(p => p.GameCricket.Played).ToList(),
            "doubles-cricket" => PlayerStats.OrderByDescending(p => p.DoublesCricketWin).ThenByDescending(p => p.GameDoubleCricket.Played).ToList(),
            "doubles-501" => PlayerStats.OrderByDescending(p => p.Doubles501Win).ThenByDescending(p => p.Game501.Played).ToList(),
            "triples-801" => PlayerStats.OrderByDescending(p => p.Triples801Win).ThenByDescending(p => p.Game801.Played).ToList(),
            _ => PlayerStats.OrderByDescending(p => p.OverallWin).ThenByDescending(p => p.GamesPlayed).ToList()
        };
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
