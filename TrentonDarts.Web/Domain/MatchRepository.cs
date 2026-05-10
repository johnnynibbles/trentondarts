using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Web.Domain;

public class MatchRepository
{
    private readonly AppDbContext _db;

    public MatchRepository(AppDbContext db) => _db = db;

    public async Task<MatchResult> GetMatchResultsFromMatchAsync(WinterSeasonMatch match)
    {
        var season = await _db.WinterSeasons.FindAsync(match.SeasonId)
            ?? throw new InvalidOperationException($"Season {match.SeasonId} not found");

        var week = await _db.WinterSeasonWeeks.FindAsync(match.WeekId)
            ?? throw new InvalidOperationException($"Week {match.WeekId} not found");

        var homeTeam = await _db.Teams.FindAsync(match.HomeTeamId)
            ?? throw new InvalidOperationException($"Team {match.HomeTeamId} not found");

        var awayTeam = await _db.Teams.FindAsync(match.AwayTeamId)
            ?? throw new InvalidOperationException($"Team {match.AwayTeamId} not found");

        // Load match rules from match type
        var matchRules = await BuildMatchRulesAsync(match.MatchTypeId, season);

        // Load all players into a dictionary for O(1) lookup
        var players = await _db.Players
            .ToDictionaryAsync(p => p.Id);

        // Load game results ordered by game rule orderId
        var gameResults = await _db.WinterGameResults
            .Where(gr => gr.MatchId == match.Id)
            .Join(_db.MatchTypeGameRules,
                gr => gr.GameRuleId,
                mgr => mgr.Id,
                (gr, mgr) => new { gr, mgr.OrderId })
            .OrderBy(x => x.OrderId)
            .Select(x => x.gr)
            .Include(gr => gr.Awards)
            .ToListAsync();

        // Load match result header row
        var matchDbResult = await _db.WinterMatchResults.FindAsync(match.Id);

        var snapshot = new MatchResultSnapshot
        {
            MatchId = match.Id,
            SeasonId = match.SeasonId,
            SeasonPart = week.WeekType,
            Division = match.Division,
            Date = week.Date,
            AwayTeamId = match.AwayTeamId,
            AwayTeamName = awayTeam.Name,
            HomeTeamId = match.HomeTeamId,
            HomeTeamName = homeTeam.Name,
            HasScorecard = matchDbResult?.HasScorecard ?? false,
            AwayScoreOverride = matchDbResult?.AwayScoreOverride ?? 0,
            HomeScoreOverride = matchDbResult?.HomeScoreOverride ?? 0,
        };

        foreach (var gr in gameResults)
        {
            var gameRules = matchRules.GameRules.FirstOrDefault(r => r.Id == gr.GameRuleId)
                ?? new GameRules();

            var awards = gr.Awards.Select(a => new GameAward
            {
                Id = a.Id,
                GameId = gr.Id,
                AwardType = a.AwardType,
                Value = a.Value,
                Player = players.TryGetValue(a.PlayerId, out var p)
                    ? new GamePlayer { Id = p.Id, Name = p.Name }
                    : null
            }).ToList();

            snapshot.GameResults.Add(new GameResultSnapshot
            {
                Id = gr.Id,
                HomePlayers = ParsePlayerIds(gr.HomePlayers, players),
                AwayPlayers = ParsePlayerIds(gr.AwayPlayers, players),
                Legs = ParseLegs(gr.Legs),
                ForfeitedBy = gr.ForfeitedBy,
                GameRules = gameRules,
                Awards = awards
            });
        }

        var result = new MatchResult();
        result.LoadSnapshot(snapshot);
        result.LoadRules(matchRules);
        return result;
    }

    public async Task SaveMatchResultsDataAsync(int matchId, ScorecardSaveDto data)
    {
        var match = await _db.WinterSeasonMatches.FindAsync(matchId)
            ?? throw new InvalidOperationException($"Match {matchId} not found");

        var matchResult = await GetMatchResultsFromMatchAsync(match);
        var games = matchResult.GetGames();

        matchResult.SetHasScorecard(data.HasScorecard);
        if (!data.HasScorecard)
        {
            matchResult.SetAwayScoreOverride(data.AwayScoreOverride);
            matchResult.SetHomeScoreOverride(data.HomeScoreOverride);
        }

        foreach (var group in data.GameGroups)
        {
            foreach (var game in group.Games)
            {
                var gameResult = games.FirstOrDefault(g => g.GameRules.Id == game.Id)
                    ?? throw new InvalidOperationException($"Game rule {game.Id} not found in match");

                ApplyPlayer(gameResult, game.AwayPlayer, isHome: false, position: 0);
                ApplyPlayer(gameResult, game.AwayPlayer2, isHome: false, position: 1);
                ApplyPlayer(gameResult, game.AwayPlayer3, isHome: false, position: 2);
                ApplyPlayer(gameResult, game.HomePlayer, isHome: true, position: 0);
                ApplyPlayer(gameResult, game.HomePlayer2, isHome: true, position: 1);
                ApplyPlayer(gameResult, game.HomePlayer3, isHome: true, position: 2);

                // Determine forfeit
                var hasHome = gameResult.HomePlayers.Any(p => p != null);
                var hasAway = gameResult.AwayPlayers.Any(p => p != null);
                gameResult.ForfeitedBy = (!hasHome && !hasAway) ? "" : !hasHome ? "home" : !hasAway ? "away" : "";

                // Legs: prefer explicit array (multi-leg), fall back to single Winner value
                var legs = new List<string>();
                if (game.Legs is { Count: > 0 })
                    legs = game.Legs.Where(l => !string.IsNullOrEmpty(l)).ToList();
                else if (game.Winner != null)
                    legs.Add(game.Winner);
                gameResult.SetLegs(legs);

                // Awards — remove deleted, add new
                var newAwards = (game.Awards ?? new List<AwardDto>())
                    .Select(a => new GameAward
                    {
                        Id = a.Id,
                        GameId = gameResult.Id,
                        AwardType = a.AwardType,
                        Value = a.AwardValue,
                        Player = a.Player != null ? new GamePlayer { Id = a.Player.Id, Name = a.Player.Name } : null
                    }).ToList();

                gameResult.Awards.RemoveAll(a => newAwards.All(n => n.Id != a.Id));
                foreach (var newAward in newAwards.Where(n => n.Id <= 0))
                    gameResult.Awards.Add(newAward);
            }
        }

        await PersistMatchResultsAsync(matchResult);
    }

    private async Task PersistMatchResultsAsync(MatchResult matchResult)
    {
        var snapshot = matchResult.GetSnapshot();

        // Upsert winter_match_results header
        var existing = await _db.WinterMatchResults.FindAsync(snapshot.MatchId);
        if (existing == null)
        {
            _db.WinterMatchResults.Add(new WinterMatchResult
            {
                Id = snapshot.MatchId,
                HasScorecard = snapshot.HasScorecard,
                AwayScoreOverride = snapshot.AwayScoreOverride,
                HomeScoreOverride = snapshot.HomeScoreOverride,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.HasScorecard = snapshot.HasScorecard;
            existing.AwayScoreOverride = snapshot.AwayScoreOverride;
            existing.HomeScoreOverride = snapshot.HomeScoreOverride;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        foreach (var gameSnap in snapshot.GameResults)
        {
            var gr = gameSnap.Id > 0
                ? await _db.WinterGameResults.FindAsync(gameSnap.Id)
                : null;

            if (gr == null)
            {
                gr = new WinterGameResult { CreatedAt = DateTime.UtcNow };
                _db.WinterGameResults.Add(gr);
            }

            gr.MatchId = snapshot.MatchId;
            gr.HomePlayers = string.Join(";", gameSnap.HomePlayers.Where(p => p != null).Select(p => p!.Id));
            gr.AwayPlayers = string.Join(";", gameSnap.AwayPlayers.Where(p => p != null).Select(p => p!.Id));
            gr.Legs = string.Join(";", gameSnap.Legs);
            gr.ForfeitedBy = gameSnap.ForfeitedBy ?? "";
            gr.GameRuleId = gameSnap.GameRules.Id;
            gr.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            // Upsert awards
            var existingAwards = await _db.WinterGameAwards.Where(a => a.GameId == gr.Id).ToListAsync();
            foreach (var ea in existingAwards.Where(ea => gameSnap.Awards.All(a => a.Id != ea.Id)))
                _db.WinterGameAwards.Remove(ea);

            foreach (var award in gameSnap.Awards.Where(a => a.Id <= 0 && a.Player != null))
            {
                _db.WinterGameAwards.Add(new WinterGameAward
                {
                    GameId = gr.Id,
                    PlayerId = award.Player!.Id,
                    AwardType = award.AwardType,
                    Value = award.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    private async Task<MatchRules> BuildMatchRulesAsync(int matchTypeId, WinterSeason season)
    {
        var gameRuleRows = await _db.MatchTypeGameRules
            .Where(r => r.MatchTypeId == matchTypeId)
            .OrderBy(r => r.OrderId)
            .ToListAsync();

        return new MatchRules
        {
            Id = matchTypeId,
            IsUsingMatchPoints = season.IsUsingMatchPoints,
            WinPoints = season.WinPoints,
            HalfPoints = season.HalfPoints,
            MinPointForHalfPoints = season.MinPointForHalfPoints,
            GameRules = gameRuleRows.Select(r => new GameRules
            {
                Id = r.Id,
                GameType = r.GameType,
                DoubleIn = r.DoubleIn,
                DoubleOut = r.DoubleOut,
                OrderId = r.OrderId,
                BestOfNumberOfLegs = r.BestOfNumberOfLegs,
                NumberOfLegs = r.NumberOfLegs,
                WhoStarts = r.WhoStarts,
                NumberOfPlayers = r.NumberOfPlayers,
                GamePointValue = r.GamePointValue,
                LegPointValue = r.LegPointValue,
                ForfeitIfNoPlayers = r.ForfeitIfNoPlayers,
                GroupName = r.GroupName
            }).ToList()
        };
    }

    private static List<GamePlayer?> ParsePlayerIds(string? raw, Dictionary<int, Data.Entities.Player> players)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new List<GamePlayer?>();
        return raw.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(id =>
            {
                if (!int.TryParse(id.Trim(), out var pid)) return null;
                return players.TryGetValue(pid, out var p)
                    ? (GamePlayer?)new GamePlayer { Id = p.Id, Name = p.Name }
                    : null;
            }).ToList();
    }

    private static List<string> ParseLegs(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return new List<string>();
        return raw.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static void ApplyPlayer(GameResult game, GamePlayerDto? dto, bool isHome, int position)
    {
        if (dto != null && dto.Id > 0)
        {
            var gp = new GamePlayer { Id = dto.Id, Name = dto.Name ?? "" };
            if (isHome) game.SetHomePlayer(gp, position);
            else game.SetAwayPlayer(gp, position);
        }
        else
        {
            if (isHome) game.RemoveHomePlayerAtPosition(position);
            else game.RemoveAwayPlayerAtPosition(position);
        }
    }
}

// DTOs for the scorecard save endpoint
public record ScorecardSaveDto(bool HasScorecard, int AwayScoreOverride, int HomeScoreOverride,
    List<GameGroupDto> GameGroups);
public record GameGroupDto(string Name, List<GameDto> Games);
public record GameDto(
    int Id,
    GamePlayerDto? AwayPlayer, GamePlayerDto? AwayPlayer2, GamePlayerDto? AwayPlayer3,
    GamePlayerDto? HomePlayer, GamePlayerDto? HomePlayer2, GamePlayerDto? HomePlayer3,
    string? Winner, List<string>? Legs, List<AwardDto>? Awards);
public record GamePlayerDto(int Id, string? Name);
public record AwardDto(int Id, int GameId, string AwardType, int AwardValue, GamePlayerDto? Player);
