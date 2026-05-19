using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Web.Services;

/// <summary>
/// Recomputes all four denormalized stat tables from an already-loaded MatchResult.
/// Ports MatchStatsRepository, TeamGameRepository, PlayerGameRepository, AwardStatRepository.
/// </summary>
public class UpdateMatchStatsService
{
    private readonly AppDbContext _db;

    public UpdateMatchStatsService(AppDbContext db) => _db = db;

    public async Task UpdateAsync(MatchResult result)
    {
        await UpdateMatchStatsAsync(result);
        await UpdateTeamGameStatsAsync(result);
        await UpdatePlayerGameStatsAsync(result);
        await UpdateAwardStatsAsync(result);
    }

    // ── MatchStatsRepository.updateMatchStats ─────────────────────────────

    private async Task UpdateMatchStatsAsync(MatchResult result)
    {
        await UpsertMatchStatAsync(result, isHome: false);
        await UpsertMatchStatAsync(result, isHome: true);
        await _db.SaveChangesAsync();
    }

    private async Task UpsertMatchStatAsync(MatchResult result, bool isHome)
    {
        var teamId = isHome ? result.HomeTeamId : result.AwayTeamId;
        var stat = await _db.WinterStatMatches
            .FirstOrDefaultAsync(s => s.MatchId == result.GetMatchId() && s.TeamId == teamId);
        if (stat == null)
        {
            stat = new WinterStatMatch { CreatedAt = DateTime.UtcNow };
            _db.WinterStatMatches.Add(stat);
        }

        stat.SeasonId = result.SeasonId;
        stat.SeasonPart = result.SeasonPart;
        stat.MatchId = result.GetMatchId();
        stat.Division = result.Division;
        stat.Date = result.Date;
        stat.TeamId = teamId;
        stat.TeamName = isHome ? result.HomeTeamName : result.AwayTeamName;
        stat.PointsWon = isHome ? result.GetHomeScore() : result.GetAwayScore();
        stat.PointsLost = isHome ? result.GetAwayScore() : result.GetHomeScore();
        stat.MatchPoints = isHome ? result.GetHomeMatchPoints() : result.GetAwayMatchPoints();
        stat.HomeMatch = isHome;
        stat.HasScorecard = result.GetHasScorecard();
        stat.UpdatedAt = DateTime.UtcNow;
    }

    // ── TeamGameRepository.updateTeamGameStats ────────────────────────────

    private async Task UpdateTeamGameStatsAsync(MatchResult result)
    {
        if (!result.GetHasScorecard()) return;

        foreach (var game in result.GetGames())
        {
            await UpsertTeamGameStatAsync(result, game, isHome: false);
            await UpsertTeamGameStatAsync(result, game, isHome: true);
        }
        await _db.SaveChangesAsync();
    }

    private async Task UpsertTeamGameStatAsync(MatchResult result, GameResult game, bool isHome)
    {
        var teamId = isHome ? result.HomeTeamId : result.AwayTeamId;
        var stat = await _db.WinterStatTeamGames
            .FirstOrDefaultAsync(s => s.GameId == game.Id && s.TeamId == teamId);
        if (stat == null)
        {
            stat = new WinterStatTeamGame { CreatedAt = DateTime.UtcNow };
            _db.WinterStatTeamGames.Add(stat);
        }

        var gameScore = isHome ? game.GetHomeScore() : game.GetAwayScore();
        var oppScore = isHome ? game.GetAwayScore() : game.GetHomeScore();

        stat.SeasonId = result.SeasonId;
        stat.SeasonPart = result.SeasonPart;
        stat.MatchId = result.GetMatchId();
        stat.Division = result.Division;
        stat.Date = result.Date;
        stat.GameId = game.Id;
        stat.TeamId = teamId;
        stat.TeamName = isHome ? result.HomeTeamName : result.AwayTeamName;
        stat.GameType = game.GameRules.GameType;
        stat.NumberOfPlayers = game.GameRules.NumberOfPlayers;
        stat.NumberOfPoints = gameScore == 0 ? oppScore : gameScore;
        stat.IsWon = gameScore > oppScore;
        stat.IsForfeitGame = !string.IsNullOrEmpty(game.ForfeitedBy);
        stat.UpdatedAt = DateTime.UtcNow;
    }

    // ── PlayerGameRepository.updatePlayerGameStats ────────────────────────

    private async Task UpdatePlayerGameStatsAsync(MatchResult result)
    {
        foreach (var game in result.GetGames())
        {
            var allPlayerIds = game.AwayPlayers.Concat(game.HomePlayers)
                .Where(p => p != null).Select(p => p!.Id).ToHashSet();

            var staleStats = await _db.WinterStatPlayerGames
                .Where(s => s.GameId == game.Id && !allPlayerIds.Contains(s.PlayerId))
                .ToListAsync();
            _db.WinterStatPlayerGames.RemoveRange(staleStats);

            await UpsertPlayerGameStatsForSideAsync(result, game, isHome: false);
            await UpsertPlayerGameStatsForSideAsync(result, game, isHome: true);
        }
        await _db.SaveChangesAsync();
    }

    private async Task UpsertPlayerGameStatsForSideAsync(MatchResult result, GameResult game, bool isHome)
    {
        var players = isHome ? game.HomePlayers : game.AwayPlayers;
        var teamId = isHome ? result.HomeTeamId : result.AwayTeamId;
        var teamName = isHome ? result.HomeTeamName : result.AwayTeamName;
        var gameScore = isHome ? game.GetHomeScore() : game.GetAwayScore();
        var oppScore = isHome ? game.GetAwayScore() : game.GetHomeScore();
        var forfeitWin = isHome ? game.ForfeitedBy == "away" : game.ForfeitedBy == "home";

        var position = 1;
        foreach (var player in players)
        {
            if (player == null) { position++; continue; }

            var stat = await _db.WinterStatPlayerGames
                .FirstOrDefaultAsync(s => s.PlayerId == player.Id && s.GameId == game.Id && s.TeamId == teamId);
            if (stat == null)
            {
                stat = new WinterStatPlayerGame { CreatedAt = DateTime.UtcNow };
                _db.WinterStatPlayerGames.Add(stat);
            }

            stat.SeasonId = result.SeasonId;
            stat.SeasonPart = result.SeasonPart;
            stat.MatchId = result.GetMatchId();
            stat.Division = result.Division;
            stat.Date = result.Date;
            stat.GameId = game.Id;
            stat.TeamId = teamId;
            stat.TeamName = teamName;
            stat.PlayerId = player.Id;
            stat.PlayerName = player.Name;
            stat.PlayerPosition = position;
            stat.GameType = game.GameRules.GameType;
            stat.NumberOfPlayers = game.GameRules.NumberOfPlayers;
            stat.NumberOfPoints = gameScore == 0 ? oppScore : gameScore;
            stat.IsWon = gameScore > oppScore;
            stat.IsForfeit = forfeitWin;
            stat.IsHome = isHome;
            stat.GameNumber = game.GameRules.OrderId;
            stat.UpdatedAt = DateTime.UtcNow;
            position++;
        }
    }

    // ── AwardStatRepository.updateAwardStats ──────────────────────────────

    private async Task UpdateAwardStatsAsync(MatchResult result)
    {
        var homePlayerIds = result.GetGames()
            .SelectMany(g => g.HomePlayers)
            .Where(p => p != null)
            .Select(p => p!.Id)
            .ToHashSet();

        foreach (var game in result.GetGames())
        {
            var currentAwards = await _db.WinterStatAwards.Where(a => a.GameId == game.Id).ToListAsync();

            foreach (var cur in currentAwards.Where(cur => game.Awards.All(a => a.Id != cur.AwardId)))
                _db.WinterStatAwards.Remove(cur);

            foreach (var award in game.Awards)
            {
                if (award.Player == null) continue;

                var stat = currentAwards.FirstOrDefault(a => a.AwardId == award.Id);
                if (stat == null)
                {
                    stat = new WinterStatAward { CreatedAt = DateTime.UtcNow };
                    _db.WinterStatAwards.Add(stat);
                }

                // Award player is always one of the game's players — derive team from HomePlayers
                var isHomePlayer = homePlayerIds.Contains(award.Player.Id);
                var teamId = isHomePlayer ? result.HomeTeamId : result.AwayTeamId;
                var teamName = isHomePlayer ? result.HomeTeamName : result.AwayTeamName;

                stat.SeasonId = result.SeasonId;
                stat.SeasonPart = result.SeasonPart;
                stat.Date = result.Date;
                stat.MatchId = result.GetMatchId();
                stat.Division = result.Division;
                stat.TeamId = teamId;
                stat.TeamName = teamName;
                stat.GameId = game.Id;
                stat.PlayerId = award.Player.Id;
                stat.PlayerName = award.Player.Name;
                stat.AwardId = award.Id;
                stat.AwardType = award.AwardType;
                stat.Value = award.Value;
                stat.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _db.SaveChangesAsync();
    }
}
