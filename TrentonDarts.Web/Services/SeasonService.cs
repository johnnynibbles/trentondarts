using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Services;

// ─── Value objects ────────────────────────────────────────────────────────────

public record DivStanding(
    int TeamId, string Name, int MatchPoints,
    int WonPoints, int LossPoints, double Percentage, int Ppts);

public record DivStandings(string Name, List<DivStanding> Standings);

public record DivWeek(DateTime Date, List<MatchRow> Matches, List<string> ByeTeamNames);

public record DivisionSchedule(string Name, List<DivWeek> Weeks);

public record MatchRow(
    int MatchId, int WeekId, string? Division,
    int HomeTeamId, string HomeTeamName,
    int AwayTeamId, string AwayTeamName,
    int HomeScore, int AwayScore, bool HasScorecard);

// ─── SeasonService ────────────────────────────────────────────────────────────

public class SeasonService
{
    private readonly AppDbContext _db;

    public SeasonService(AppDbContext db) => _db = db;

    // ── Week navigation ───────────────────────────────────────────────────────

    public async Task<DateTime?> GetCurrentWeekDateAsync(WinterSeason season, DateTime asOf)
    {
        var lower = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id && w.Date <= asOf)
            .OrderByDescending(w => w.Date)
            .FirstOrDefaultAsync();

        if (lower != null) return lower.Date;

        var first = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id)
            .OrderBy(w => w.Date)
            .FirstOrDefaultAsync();

        return first?.Date;
    }

    public async Task<DateTime?> GetPreviousWeekDateAsync(WinterSeason season, DateTime weekDate)
    {
        var lower = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id && w.Date < weekDate)
            .OrderByDescending(w => w.Date)
            .FirstOrDefaultAsync();
        return lower?.Date;
    }

    public async Task<DateTime?> GetNextWeekDateAsync(WinterSeason season, DateTime weekDate)
    {
        var upper = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id && w.Date > weekDate)
            .OrderBy(w => w.Date)
            .FirstOrDefaultAsync();
        return upper?.Date;
    }

    public async Task<SeasonPart> GetCurrentSeasonPartAsync(WinterSeason season, DateTime? weekDate)
    {
        if (weekDate == null)
        {
            var hasPreWeeks = await _db.WinterSeasonWeeks
                .AnyAsync(w => w.SeasonId == season.Id && w.WeekType == SeasonPart.Pre);
            return hasPreWeeks ? SeasonPart.Pre : SeasonPart.Regular;
        }

        var lower = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id && w.WeekType != SeasonPart.Post && w.Date <= weekDate)
            .OrderByDescending(w => w.Date)
            .FirstOrDefaultAsync();

        return lower?.WeekType ?? SeasonPart.Pre;
    }

    // ── Standings ─────────────────────────────────────────────────────────────

    public async Task<List<DivStandings>> GetDivisionStandingsAsync(
        WinterSeason season, SeasonPart seasonPart, DateTime? asOfDate)
    {
        // Raw standings aggregation
        var standingsQuery = _db.WinterStatMatches
            .Where(s => s.SeasonId == season.Id);

        if (!season.AccumulatePointsForAllParts)
            standingsQuery = standingsQuery.Where(s => s.SeasonPart == seasonPart);

        if (asOfDate.HasValue)
            standingsQuery = standingsQuery.Where(s => s.Date <= asOfDate.Value);

        var rawStandings = await standingsQuery
            .GroupBy(s => s.TeamId)
            .Select(g => new
            {
                TeamId = g.Key,
                MatchPoints = g.Sum(s => s.MatchPoints),
                MatchesWon = g.Sum(s => s.PointsWon > s.PointsLost ? 1 : 0),
                MatchesLost = g.Sum(s => s.PointsLost > s.PointsWon ? 1 : 0),
                PointsWon = g.Sum(s => s.PointsWon),
                PointsLost = g.Sum(s => s.PointsLost),
            })
            .ToListAsync();

        // Get all season teams grouped by division
        var seasonTeams = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == season.Id)
            .Include(st => st.Team)
            .ToListAsync();

        var isRegular = seasonPart == SeasonPart.Regular;

        // Get unique divisions
        var divisions = seasonTeams
            .Select(st => isRegular ? st.RegularSeasonDiv : st.PreSeasonDiv)
            .Where(d => !string.IsNullOrEmpty(d))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var result = new List<DivStandings>();
        foreach (var div in divisions)
        {
            var teamsInDiv = seasonTeams
                .Where(st => (isRegular ? st.RegularSeasonDiv : st.PreSeasonDiv) == div)
                .ToList();

            var standings = new List<DivStanding>();
            foreach (var st in teamsInDiv)
            {
                var raw = rawStandings.FirstOrDefault(r => r.TeamId == st.TeamId);
                int wonPts = 0, lossPts = 0, matchPts = 0;
                double pct = 0;

                if (raw != null)
                {
                    matchPts = raw.MatchPoints;
                    if (!season.IsUsingMatchPoints)
                    {
                        wonPts = raw.PointsWon;
                        lossPts = raw.PointsLost;
                    }
                    else
                    {
                        wonPts = raw.MatchesWon;
                        lossPts = raw.MatchesLost;
                    }
                    if (raw.PointsWon + raw.PointsLost > 0)
                        pct = (double)raw.PointsWon / (raw.PointsWon + raw.PointsLost);
                }

                standings.Add(new DivStanding(st.TeamId, st.Team.Name, matchPts, wonPts, lossPts, pct, 0));
            }

            // Sort: match points first (if using), then wonPoints, then pct
            standings = season.IsUsingMatchPoints
                ? standings
                    .OrderByDescending(s => s.MatchPoints)
                    .ThenByDescending(s => s.WonPoints)
                    .ThenByDescending(s => s.Percentage)
                    .ToList()
                : standings
                    .OrderByDescending(s => s.WonPoints)
                    .ThenByDescending(s => s.Percentage)
                    .ToList();

            result.Add(new DivStandings(div!, standings));
        }

        return result;
    }

    // ── Schedule ──────────────────────────────────────────────────────────────

    public async Task<(List<DivisionSchedule> Pre, List<DivisionSchedule> Regular)>
        GetFullScheduleAsync(WinterSeason season)
    {
        var matchResults = await GetMatchResultsLookupAsync(season.Id);

        var allSeasonTeams = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == season.Id)
            .Include(st => st.Team)
            .ToListAsync();

        var preSchedules = await BuildDivSchedulesAsync(season, SeasonPart.Pre, allSeasonTeams, matchResults);
        var regularSchedules = await BuildDivSchedulesAsync(season, SeasonPart.Regular, allSeasonTeams, matchResults);

        return (preSchedules, regularSchedules.Count > 0 ? regularSchedules : new List<DivisionSchedule>());
    }

    public async Task<List<DivisionSchedule>> GetWeekScheduleAsync(
        WinterSeason season, DateTime weekDate)
    {
        var matchResults = await GetMatchResultsLookupAsync(season.Id);
        var week = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == season.Id && w.Date == weekDate.Date)
            .FirstOrDefaultAsync();

        var allTeamNames = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == season.Id)
            .Include(st => st.Team)
            .Select(st => st.Team.Name)
            .ToListAsync();

        var matchRows = new List<MatchRow>();
        var byeTeams = new List<string>(allTeamNames);

        if (week != null)
        {
            var weekMatches = await _db.WinterSeasonMatches
                .Where(m => m.WeekId == week.Id)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .OrderBy(m => m.Division)
                .ToListAsync();

            foreach (var m in weekMatches)
            {
                var r = matchResults.GetValueOrDefault(m.Id);
                matchRows.Add(new MatchRow(m.Id, m.WeekId, m.Division,
                    m.HomeTeamId, m.HomeTeam.Name,
                    m.AwayTeamId, m.AwayTeam.Name,
                    r?.HomeScore ?? 0, r?.AwayScore ?? 0, r?.HasScorecard ?? false));
                byeTeams.Remove(m.HomeTeam.Name);
                byeTeams.Remove(m.AwayTeam.Name);
            }
        }

        var divWeek = new DivWeek(weekDate, matchRows, byeTeams);
        return new List<DivisionSchedule> { new DivisionSchedule("", new List<DivWeek> { divWeek }) };
    }

    public async Task<List<DivisionSchedule>> GetNextWeekScheduleAsync(
        WinterSeason season, DateTime? afterDate)
    {
        var refDate = afterDate ?? DateTime.Today;
        var nextDate = await GetNextWeekDateAsync(season, refDate);
        if (nextDate == null) return new List<DivisionSchedule>();
        return await GetWeekScheduleAsync(season, nextDate.Value);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<List<DivisionSchedule>> BuildDivSchedulesAsync(
        WinterSeason season, SeasonPart weekType,
        List<WinterSeasonTeam> allSeasonTeams,
        Dictionary<int, MatchResultRow> matchResults)
    {
        var isRegular = weekType == SeasonPart.Regular;

        var divisions = allSeasonTeams
            .Select(st => isRegular ? st.RegularSeasonDiv : st.PreSeasonDiv)
            .Where(d => !string.IsNullOrEmpty(d))
            .Select(d => d![0].ToString())
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var schedules = new List<DivisionSchedule>();
        foreach (var divShort in divisions)
        {
            var divTeams = allSeasonTeams
                .Where(st => (isRegular ? st.RegularSeasonDiv : st.PreSeasonDiv)
                             ?.StartsWith(divShort) == true)
                .Select(st => st.Team.Name)
                .ToList();

            var weeks = await _db.WinterSeasonWeeks
                .Where(w => w.SeasonId == season.Id && w.WeekType == weekType)
                .OrderBy(w => w.Date)
                .ToListAsync();

            var divWeeks = new List<DivWeek>();
            foreach (var week in weeks)
            {
                var weekMatches = await _db.WinterSeasonMatches
                    .Where(m => m.WeekId == week.Id &&
                                (m.Division != null && m.Division.StartsWith(divShort)))
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .ToListAsync();

                var byeTeams = new List<string>(divTeams);
                var matchRows = new List<MatchRow>();

                foreach (var m in weekMatches)
                {
                    var r = matchResults.GetValueOrDefault(m.Id);
                    matchRows.Add(new MatchRow(m.Id, m.WeekId, m.Division,
                        m.HomeTeamId, m.HomeTeam.Name,
                        m.AwayTeamId, m.AwayTeam.Name,
                        r?.HomeScore ?? 0, r?.AwayScore ?? 0, r?.HasScorecard ?? false));
                    byeTeams.Remove(m.HomeTeam.Name);
                    byeTeams.Remove(m.AwayTeam.Name);
                }

                divWeeks.Add(new DivWeek(week.Date, matchRows, byeTeams));
            }

            schedules.Add(new DivisionSchedule(divShort, divWeeks));
        }

        return schedules;
    }

    public async Task<Dictionary<int, MatchResultRow>> GetMatchResultsLookupAsync(int seasonId)
    {
        var stats = await _db.WinterStatMatches
            .Where(s => s.SeasonId == seasonId)
            .ToListAsync();

        var lookup = new Dictionary<int, MatchResultRow>();
        foreach (var s in stats)
        {
            if (!lookup.TryGetValue(s.MatchId, out var row))
            {
                row = new MatchResultRow { MatchId = s.MatchId, HasScorecard = s.HasScorecard };
                lookup[s.MatchId] = row;
            }
            if (s.HomeMatch)
            {
                row.HomeTeamId = s.TeamId;
                row.HomeTeamName = s.TeamName ?? "";
                row.HomeScore = s.PointsWon;
            }
            else
            {
                row.AwayTeamId = s.TeamId;
                row.AwayTeamName = s.TeamName ?? "";
                row.AwayScore = s.PointsWon;
            }
        }
        return lookup;
    }
}

public class MatchResultRow
{
    public int MatchId { get; set; }
    public bool HasScorecard { get; set; }
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = "";
    public int HomeScore { get; set; }
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = "";
    public int AwayScore { get; set; }
}
