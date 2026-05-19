using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;
using SP = TrentonDarts.Web.Domain.SeasonPart;

namespace TrentonDarts.Web.Pages.Teams;

public class TeamScheduleRow
{
    public DateTime Date { get; set; }
    public string SeasonPart { get; set; } = "";
    public bool IsBye { get; set; }
    public int? MatchId { get; set; }
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = "";
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = "";
    public int AwayScore { get; set; }
    public int HomeScore { get; set; }
    public bool HasScorecard { get; set; }
    public bool IsAwayGame { get; set; }
}

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SeasonService _seasonService;
    private readonly StatsService _statsService;

    public ShowModel(AppDbContext db, SeasonService seasonService, StatsService statsService)
    {
        _db = db;
        _seasonService = seasonService;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public WinterSeasonTeam SeasonTeam { get; private set; } = null!;
    public List<WinterSeason> AllSeasons { get; private set; } = new();
    public SeasonPart? StatPart { get; private set; }
    public List<DivStandings> DivStandings { get; private set; } = new();
    public List<WinterStatAward> Awards { get; private set; } = new();
    public List<TeamStat> TeamStats { get; private set; } = new();
    public List<PlayerStat> PlayerStats { get; private set; } = new();
    public List<TeamScheduleRow> TeamSchedule { get; private set; } = new();
    public bool IsBoardMember { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id,
        [FromQuery] string? season, [FromQuery] SeasonPart? statpart)
    {
        if (season != null && int.TryParse(season, out var seasonId))
            Season = await _db.WinterSeasons.FindAsync(seasonId) ?? await GetCurrentSeasonAsync();
        else
            Season = await GetCurrentSeasonAsync();

        AllSeasons = await _db.WinterSeasons.OrderBy(s => s.EndYear).ToListAsync();

        SeasonTeam = await _db.WinterSeasonTeams
            .Where(st => st.TeamId == id && st.SeasonId == Season.Id)
            .Include(st => st.Team)
            .FirstOrDefaultAsync();
        if (SeasonTeam == null) return NotFound();

        IsBoardMember = await IsBoardMemberAsync();

        var today = DateTime.Today;
        var currentPart = await _seasonService.GetCurrentSeasonPartAsync(Season, DateTime.Today);
        StatPart = statpart ?? (Season.AccumulatePointsForAllParts ? (SeasonPart?)null : currentPart);

        // Division standings for the team's division
        var teamDiv = currentPart == SP.Regular ? SeasonTeam.RegularSeasonDiv : SeasonTeam.PreSeasonDiv;
        DivStandings = await _seasonService.GetDivisionStandingsAsync(Season, currentPart, today);
        DivStandings = DivStandings.Where(d => d.Name == teamDiv).ToList();

        // Awards for this team
        Awards = await _statsService.GetAwardsForSeasonAsync(Season.Id, null, null, null);
        Awards = Awards.Where(a => a.TeamId == SeasonTeam.TeamId).ToList();

        // Stats
        TeamStats = await _statsService.GetTeamStatsForSeasonPartAsync(Season.Id, StatPart, "");
        TeamStats = TeamStats.Where(t => t.TeamId == SeasonTeam.TeamId).ToList();

        PlayerStats = await _statsService.GetPlayerStatsForSeasonPartAsync(Season.Id, StatPart, "");
        PlayerStats = PlayerStats.Where(p => p.TeamId == SeasonTeam.TeamId).ToList();

        // Team schedule
        TeamSchedule = await BuildTeamScheduleAsync(Season, SeasonTeam.TeamId);

        return Page();
    }

    private async Task<List<TeamScheduleRow>> BuildTeamScheduleAsync(WinterSeason season, int teamId)
    {
        var weeks = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id)
            .OrderBy(w => w.Date)
            .ToListAsync();

        var matchResults = await _seasonService.GetMatchResultsLookupAsync(season.Id);

        var matches = await _db.WinterSeasonMatches
            .Where(m => m.SeasonId == season.Id && (m.HomeTeamId == teamId || m.AwayTeamId == teamId))
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .ToListAsync();

        var schedule = new List<TeamScheduleRow>();
        foreach (var week in weeks)
        {
            var match = matches.FirstOrDefault(m => m.WeekId == week.Id);
            if (match == null)
            {
                schedule.Add(new TeamScheduleRow
                {
                    Date = week.Date,
                    SeasonPart = week.WeekType == SP.Pre ? "Pre" : "Regular",
                    IsBye = true
                });
            }
            else
            {
                matchResults.TryGetValue(match.Id, out var result);
                schedule.Add(new TeamScheduleRow
                {
                    Date = week.Date,
                    SeasonPart = week.WeekType == SP.Pre ? "Pre" : "Regular",
                    MatchId = match.Id,
                    AwayTeamId = match.AwayTeamId,
                    AwayTeamName = match.AwayTeam.Name,
                    HomeTeamId = match.HomeTeamId,
                    HomeTeamName = match.HomeTeam.Name,
                    AwayScore = result?.AwayScore ?? 0,
                    HomeScore = result?.HomeScore ?? 0,
                    HasScorecard = result?.HasScorecard ?? false,
                    IsAwayGame = match.AwayTeamId == teamId
                });
            }
        }
        return schedule;
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
