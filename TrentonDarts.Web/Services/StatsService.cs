using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Services;

// ─── Stat result types ────────────────────────────────────────────────────────

public class GameTypeStat
{
    public int Played { get; set; }
    public int Won { get; set; }
    public int Lost { get; set; }
}

public class PlayerStat
{
    public int SeasonId { get; set; }
    public SeasonPart? SeasonPart { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
    public int GamesPlayed { get; set; }
    public int WeeksPlayed { get; set; }
    public int PointsWon { get; set; }
    public int PointsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public double OverallWin { get; set; }
    public double SinglesWin { get; set; }
    public double DoublesWin { get; set; }
    public double Eight01Win { get; set; }
    public double CricketWin { get; set; }
    public double Oh1Win { get; set; }
    public double Singles301Win { get; set; }
    public double SinglesCricketWin { get; set; }
    public double DoublesCricketWin { get; set; }
    public double Doubles501Win { get; set; }
    public double Triples801Win { get; set; }
    public GameTypeStat Game301 { get; set; } = new();
    public GameTypeStat GameCricket { get; set; } = new();
    public GameTypeStat Game501 { get; set; } = new();
    public GameTypeStat GameDoubleCricket { get; set; } = new();
    public GameTypeStat Game801 { get; set; } = new();
}

public class TeamStat
{
    public int SeasonId { get; set; }
    public SeasonPart? SeasonPart { get; set; }
    public string? PreSeasonDiv { get; set; }
    public string? RegularSeasonDiv { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int PointsWon { get; set; }
    public int PointsLost { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public double OverallWin { get; set; }
    public double SinglesWin { get; set; }
    public double DoublesWin { get; set; }
    public double Eight01Win { get; set; }
    public double CricketWin { get; set; }
    public double Oh1Win { get; set; }
    public double Singles301Win { get; set; }
    public double SinglesCricketWin { get; set; }
    public double DoublesCricketWin { get; set; }
    public double Doubles501Win { get; set; }
    public double Triples801Win { get; set; }
    public GameTypeStat Game301 { get; set; } = new();
    public GameTypeStat GameCricket { get; set; } = new();
    public GameTypeStat Game501 { get; set; } = new();
    public GameTypeStat GameDoubleCricket { get; set; } = new();
    public GameTypeStat Game801 { get; set; } = new();
}

// ─── StatsService ─────────────────────────────────────────────────────────────

public class StatsService
{
    private readonly AppDbContext _db;

    public StatsService(AppDbContext db) => _db = db;

    public async Task<List<PlayerStat>> GetPlayerStatsForSeasonPartAsync(
        int seasonId, SeasonPart? seasonPart, string? division)
    {
        var partFilter = seasonPart == null ? "" :
            $"AND (winter_stats_player_games.\"seasonPart\" IS NULL OR winter_stats_player_games.\"seasonPart\" = '{seasonPart.Value.ToString().ToLower()}')";

        var divFilter = string.IsNullOrEmpty(division) ? "" :
            $"AND (winter_season_teams.\"preSeasonDiv\" = '{division}' OR winter_season_teams.\"regularSeasonDiv\" = '{division}')";

        var sql = $"""
            SELECT
              teams."Id" AS "TeamId",
              teams."Name" AS "TeamName",
              players."Id" AS "PlayerId",
              (players."firstName" || ' ' || players."lastName") AS "PlayerName",
              SUM(CASE WHEN winter_stats_player_games."playerId" IS NOT NULL AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GamesPlayed",
              SUM(CASE WHEN winter_stats_player_games."playerId" IS NOT NULL AND "isForfeit" = false AND "isWon" = true  THEN "numberOfPoints" ELSE 0 END) AS "PointsWon",
              SUM(CASE WHEN winter_stats_player_games."playerId" IS NOT NULL AND "isWon" = false THEN "numberOfPoints" ELSE 0 END) AS "PointsLost",
              SUM(CASE WHEN winter_stats_player_games."playerId" IS NOT NULL AND "isForfeit" = false AND "isWon" = true  THEN 1 ELSE 0 END) AS "GamesWon",
              SUM(CASE WHEN winter_stats_player_games."playerId" IS NOT NULL AND "isWon" = false THEN 1 ELSE 0 END) AS "GamesLost",
              SUM(CASE WHEN "gameType" = 'cricket'                                     AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GameCricketPlayed",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1            AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GameSingle01Played",
              SUM(CASE WHEN "gameType" = 'double501'                                   AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GameDouble501Played",
              SUM(CASE WHEN "gameType" = 'doublecricket'                               AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GameDoubleCricketPlayed",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3            AND "isForfeit" = false THEN 1 ELSE 0 END) AS "GameTriple01Played",
              SUM(CASE WHEN "gameType" = 'cricket'      AND "isForfeit" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameCricketWon",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1 AND "isForfeit" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameSingle01Won",
              SUM(CASE WHEN "gameType" = 'double501'    AND "isForfeit" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameDouble501Won",
              SUM(CASE WHEN "gameType" = 'doublecricket' AND "isForfeit" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameDoubleCricketWon",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3 AND "isForfeit" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameTriple01Won",
              SUM(CASE WHEN "gameType" = 'cricket'      AND "isForfeit" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameCricketLost",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1 AND "isForfeit" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameSingle01Lost",
              SUM(CASE WHEN "gameType" = 'double501'    AND "isForfeit" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameDouble501Lost",
              SUM(CASE WHEN "gameType" = 'doublecricket' AND "isForfeit" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameDoubleCricketLost",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3 AND "isForfeit" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameTriple01Lost"
            FROM winter_season_team_players
            LEFT JOIN players ON winter_season_team_players."playerId" = players."Id"
            LEFT JOIN winter_season_teams ON winter_season_team_players."seasonTeamId" = winter_season_teams."Id"
            LEFT JOIN teams ON winter_season_teams."teamId" = teams."Id"
            LEFT JOIN winter_stats_player_games ON players."Id" = winter_stats_player_games."playerId"
              AND (winter_stats_player_games."seasonId" IS NULL OR winter_stats_player_games."seasonId" = {seasonId})
            WHERE winter_season_teams."seasonId" = {seasonId}
            {divFilter}
            {partFilter}
            GROUP BY teams."Id", teams."Name", players."Id", players."firstName", players."lastName"
            ORDER BY teams."Name"
            """;

        var rows = await _db.Database.SqlQueryRaw<PlayerStatRow>(sql).ToListAsync();

        // Weeks played requires a separate count per player
        var weeksByPlayer = await _db.WinterStatPlayerGames
            .Where(pg => pg.SeasonId == seasonId)
            .GroupBy(pg => pg.PlayerId)
            .Select(g => new { g.Key, Weeks = g.Select(x => x.Date.Date).Distinct().Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Weeks);

        return rows.Select(r =>
        {
            var s = new PlayerStat
            {
                SeasonId = seasonId,
                SeasonPart = seasonPart,
                TeamId = r.TeamId,
                TeamName = r.TeamName,
                PlayerId = r.PlayerId,
                PlayerName = r.PlayerName,
                GamesPlayed = r.GamesPlayed,
                WeeksPlayed = weeksByPlayer.GetValueOrDefault(r.PlayerId),
                PointsWon = r.PointsWon,
                PointsLost = r.PointsLost,
                GamesWon = r.GamesWon,
                GamesLost = r.GamesLost,
            };

            s.OverallWin = Pct(r.GamesWon, r.GamesLost);
            s.SinglesWin = Pct(r.GameSingle01Won + r.GameCricketWon, r.GameSingle01Played + r.GameCricketPlayed - r.GameSingle01Won - r.GameCricketWon);
            s.DoublesWin = Pct(r.GameDouble501Won + r.GameDoubleCricketWon, r.GameDouble501Played + r.GameDoubleCricketPlayed - r.GameDouble501Won - r.GameDoubleCricketWon);
            s.Eight01Win = Pct(r.GameTriple01Won, r.GameTriple01Played - r.GameTriple01Won);
            s.CricketWin = Pct(r.GameCricketWon + r.GameDoubleCricketWon, r.GameCricketPlayed + r.GameDoubleCricketPlayed - r.GameCricketWon - r.GameDoubleCricketWon);
            s.Oh1Win = Pct(r.GameSingle01Won + r.GameDouble501Won + r.GameTriple01Won,
                r.GameSingle01Played + r.GameDouble501Played + r.GameTriple01Played - r.GameSingle01Won - r.GameDouble501Won - r.GameTriple01Won);
            if (r.GameSingle01Played > 0) s.Singles301Win = (double)r.GameSingle01Won / r.GameSingle01Played;
            if (r.GameCricketPlayed > 0) s.SinglesCricketWin = (double)r.GameCricketWon / r.GameCricketPlayed;
            if (r.GameDoubleCricketPlayed > 0) s.DoublesCricketWin = (double)r.GameDoubleCricketWon / r.GameDoubleCricketPlayed;
            if (r.GameDouble501Played > 0) s.Doubles501Win = (double)r.GameDouble501Won / r.GameDouble501Played;
            if (r.GameTriple01Played > 0) s.Triples801Win = (double)r.GameTriple01Won / r.GameTriple01Played;

            s.Game301 = new GameTypeStat { Played = r.GameSingle01Played, Won = r.GameSingle01Won, Lost = r.GameSingle01Lost };
            s.GameCricket = new GameTypeStat { Played = r.GameCricketPlayed, Won = r.GameCricketWon, Lost = r.GameCricketLost };
            s.Game501 = new GameTypeStat { Played = r.GameDouble501Played, Won = r.GameDouble501Won, Lost = r.GameDouble501Lost };
            s.GameDoubleCricket = new GameTypeStat { Played = r.GameDoubleCricketPlayed, Won = r.GameDoubleCricketWon, Lost = r.GameDoubleCricketLost };
            s.Game801 = new GameTypeStat { Played = r.GameTriple01Played, Won = r.GameTriple01Won, Lost = r.GameTriple01Lost };

            return s;
        }).ToList();
    }

    public async Task<List<TeamStat>> GetTeamStatsForSeasonPartAsync(
        int seasonId, SeasonPart? seasonPart, string? division)
    {
        var partFilter = seasonPart == null ? "" :
            $"AND (winter_stats_team_games.\"seasonPart\" IS NULL OR winter_stats_team_games.\"seasonPart\" = '{seasonPart.Value.ToString().ToLower()}')";

        var divFilter = string.IsNullOrEmpty(division) ? "" :
            $"AND (winter_season_teams.\"preSeasonDiv\" = '{division}' OR winter_season_teams.\"regularSeasonDiv\" = '{division}')";

        var sql = $"""
            SELECT
              teams."Id" AS "TeamId",
              teams."Name" AS "TeamName",
              winter_season_teams."preSeasonDiv" AS "PreSeasonDiv",
              winter_season_teams."regularSeasonDiv" AS "RegularSeasonDiv",
              SUM(CASE WHEN "isWon" = true  AND "isForfeitGame" = false THEN "numberOfPoints" ELSE 0 END) AS "PointsWon",
              SUM(CASE WHEN "isWon" = false AND "isForfeitGame" = false THEN "numberOfPoints" ELSE 0 END) AS "PointsLost",
              SUM(CASE WHEN winter_stats_team_games."teamId" IS NOT NULL AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GamesPlayed",
              SUM(CASE WHEN "isWon" = true  AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GamesWon",
              SUM(CASE WHEN "isWon" = false AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GamesLost",
              SUM(CASE WHEN "gameType" = 'cricket'       AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GameCricketPlayed",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1 AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GameSingle01Played",
              SUM(CASE WHEN "gameType" = 'double501'     AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GameDouble501Played",
              SUM(CASE WHEN "gameType" = 'doublecricket' AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GameDoubleCricketPlayed",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3 AND "isForfeitGame" = false THEN 1 ELSE 0 END) AS "GameTriple01Played",
              SUM(CASE WHEN "gameType" = 'cricket'       AND "isForfeitGame" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameCricketWon",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1 AND "isForfeitGame" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameSingle01Won",
              SUM(CASE WHEN "gameType" = 'double501'     AND "isForfeitGame" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameDouble501Won",
              SUM(CASE WHEN "gameType" = 'doublecricket' AND "isForfeitGame" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameDoubleCricketWon",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3 AND "isForfeitGame" = false AND "isWon" = true THEN 1 ELSE 0 END) AS "GameTriple01Won",
              SUM(CASE WHEN "gameType" = 'cricket'       AND "isForfeitGame" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameCricketLost",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 1 AND "isForfeitGame" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameSingle01Lost",
              SUM(CASE WHEN "gameType" = 'double501'     AND "isForfeitGame" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameDouble501Lost",
              SUM(CASE WHEN "gameType" = 'doublecricket' AND "isForfeitGame" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameDoubleCricketLost",
              SUM(CASE WHEN "gameType" LIKE '%01' AND "numberOfPlayers" = 3 AND "isForfeitGame" = false AND "isWon" = false THEN 1 ELSE 0 END) AS "GameTriple01Lost"
            FROM winter_season_teams
            LEFT JOIN teams ON winter_season_teams."teamId" = teams."Id"
            LEFT JOIN winter_stats_team_games ON teams."Id" = winter_stats_team_games."teamId"
              AND (winter_stats_team_games."seasonId" IS NULL OR winter_stats_team_games."seasonId" = {seasonId})
            WHERE winter_season_teams."seasonId" = {seasonId}
            {divFilter}
            {partFilter}
            GROUP BY teams."Id", teams."Name", winter_season_teams."preSeasonDiv", winter_season_teams."regularSeasonDiv"
            ORDER BY teams."Name"
            """;

        var rows = await _db.Database.SqlQueryRaw<TeamStatRow>(sql).ToListAsync();

        // Match-level points come from winter_stats_matches
        var matchStatsQuery = _db.WinterStatMatches.Where(s => s.SeasonId == seasonId);
        if (seasonPart != null) matchStatsQuery = matchStatsQuery.Where(s => s.SeasonPart == seasonPart);
        var matchPoints = await matchStatsQuery
            .GroupBy(s => s.TeamId)
            .Select(g => new { TeamId = g.Key, Won = g.Sum(s => s.PointsWon), Lost = g.Sum(s => s.PointsLost) })
            .ToDictionaryAsync(x => x.TeamId);

        return rows.Select(r =>
        {
            var mp = matchPoints.GetValueOrDefault(r.TeamId);
            var s = new TeamStat
            {
                SeasonId = seasonId,
                SeasonPart = seasonPart,
                PreSeasonDiv = r.PreSeasonDiv,
                RegularSeasonDiv = r.RegularSeasonDiv,
                TeamId = r.TeamId,
                TeamName = r.TeamName,
                PointsWon = mp?.Won ?? r.PointsWon,
                PointsLost = mp?.Lost ?? r.PointsLost,
                GamesPlayed = r.GamesPlayed,
                GamesWon = r.GamesWon,
                GamesLost = r.GamesLost,
            };

            s.OverallWin = Pct(r.GamesWon, r.GamesLost);
            s.SinglesWin = Pct(r.GameSingle01Won + r.GameCricketWon, r.GameSingle01Played + r.GameCricketPlayed - r.GameSingle01Won - r.GameCricketWon);
            s.DoublesWin = Pct(r.GameDouble501Won + r.GameDoubleCricketWon, r.GameDouble501Played + r.GameDoubleCricketPlayed - r.GameDouble501Won - r.GameDoubleCricketWon);
            s.Eight01Win = Pct(r.GameTriple01Won, r.GameTriple01Played - r.GameTriple01Won);
            s.CricketWin = Pct(r.GameCricketWon + r.GameDoubleCricketWon, r.GameCricketPlayed + r.GameDoubleCricketPlayed - r.GameCricketWon - r.GameDoubleCricketWon);
            s.Oh1Win = Pct(r.GameSingle01Won + r.GameDouble501Won + r.GameTriple01Won,
                r.GameSingle01Played + r.GameDouble501Played + r.GameTriple01Played - r.GameSingle01Won - r.GameDouble501Won - r.GameTriple01Won);
            if (r.GameSingle01Played > 0) s.Singles301Win = (double)r.GameSingle01Won / r.GameSingle01Played;
            if (r.GameCricketPlayed > 0) s.SinglesCricketWin = (double)r.GameCricketWon / r.GameCricketPlayed;
            if (r.GameDoubleCricketPlayed > 0) s.DoublesCricketWin = (double)r.GameDoubleCricketWon / r.GameDoubleCricketPlayed;
            if (r.GameDouble501Played > 0) s.Doubles501Win = (double)r.GameDouble501Won / r.GameDouble501Played;
            if (r.GameTriple01Played > 0) s.Triples801Win = (double)r.GameTriple01Won / r.GameTriple01Played;

            s.Game301 = new GameTypeStat { Played = r.GameSingle01Played, Won = r.GameSingle01Won, Lost = r.GameSingle01Lost };
            s.GameCricket = new GameTypeStat { Played = r.GameCricketPlayed, Won = r.GameCricketWon, Lost = r.GameCricketLost };
            s.Game501 = new GameTypeStat { Played = r.GameDouble501Played, Won = r.GameDouble501Won, Lost = r.GameDouble501Lost };
            s.GameDoubleCricket = new GameTypeStat { Played = r.GameDoubleCricketPlayed, Won = r.GameDoubleCricketWon, Lost = r.GameDoubleCricketLost };
            s.Game801 = new GameTypeStat { Played = r.GameTriple01Played, Won = r.GameTriple01Won, Lost = r.GameTriple01Lost };

            return s;
        }).ToList();
    }

    public async Task<List<Data.Entities.WinterStatAward>> GetAwardsForSeasonAsync(
        int seasonId, SeasonPart? seasonPart, string? division, DateTime? weekDate)
    {
        var q = _db.WinterStatAwards.Where(a => a.SeasonId == seasonId);
        if (seasonPart != null)
            q = q.Where(a => a.SeasonPart == seasonPart);
        if (!string.IsNullOrEmpty(division))
            q = q.Where(a => a.Division == division);
        if (weekDate.HasValue)
            q = q.Where(a => a.Date <= weekDate.Value);

        return await q.OrderByDescending(a => a.AwardType)
                      .ThenByDescending(a => a.Value)
                      .ThenByDescending(a => a.Date)
                      .ToListAsync();
    }

    private static double Pct(int won, int lost) =>
        (won + lost) == 0 ? 0 : (double)won / (won + lost);
}

// ─── Raw SQL projection types ─────────────────────────────────────────────────

// These must have properties matching the SQL column aliases exactly.
// EF Core SqlQueryRaw maps by property name (case-insensitive).

internal class PlayerStatRow
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
    public int GamesPlayed { get; set; }
    public int PointsWon { get; set; }
    public int PointsLost { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GameCricketPlayed { get; set; }
    public int GameSingle01Played { get; set; }
    public int GameDouble501Played { get; set; }
    public int GameDoubleCricketPlayed { get; set; }
    public int GameTriple01Played { get; set; }
    public int GameCricketWon { get; set; }
    public int GameSingle01Won { get; set; }
    public int GameDouble501Won { get; set; }
    public int GameDoubleCricketWon { get; set; }
    public int GameTriple01Won { get; set; }
    public int GameCricketLost { get; set; }
    public int GameSingle01Lost { get; set; }
    public int GameDouble501Lost { get; set; }
    public int GameDoubleCricketLost { get; set; }
    public int GameTriple01Lost { get; set; }
}

internal class TeamStatRow
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public string? PreSeasonDiv { get; set; }
    public string? RegularSeasonDiv { get; set; }
    public int PointsWon { get; set; }
    public int PointsLost { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GameCricketPlayed { get; set; }
    public int GameSingle01Played { get; set; }
    public int GameDouble501Played { get; set; }
    public int GameDoubleCricketPlayed { get; set; }
    public int GameTriple01Played { get; set; }
    public int GameCricketWon { get; set; }
    public int GameSingle01Won { get; set; }
    public int GameDouble501Won { get; set; }
    public int GameDoubleCricketWon { get; set; }
    public int GameTriple01Won { get; set; }
    public int GameCricketLost { get; set; }
    public int GameSingle01Lost { get; set; }
    public int GameDouble501Lost { get; set; }
    public int GameDoubleCricketLost { get; set; }
    public int GameTriple01Lost { get; set; }
}
