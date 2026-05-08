using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Web.Services;

/// <summary>
/// Recomputes all four denormalized stat tables for a given match.
/// Ports MatchStatsRepository, TeamGameRepository, PlayerGameRepository, AwardStatRepository.
/// </summary>
public class UpdateMatchStatsService
{
    private readonly AppDbContext _db;
    private readonly MatchRepository _matchRepo;

    public UpdateMatchStatsService(AppDbContext db, MatchRepository matchRepo)
    {
        _db = db;
        _matchRepo = matchRepo;
    }

    public async Task UpdateAsync(int matchId)
    {
        var match = await _db.WinterSeasonMatches.FindAsync(matchId);
        if (match == null) return;

        var result = await _matchRepo.GetMatchResultsFromMatchAsync(match);

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
            // Remove stats for players no longer in game
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
        // Pre-load season team membership for team lookup
        var awayTeamPlayerIds = await _db.WinterSeasonTeamPlayers
            .Where(tp => tp.SeasonId == result.SeasonId && tp.SeasonTeam.TeamId == result.AwayTeamId)
            .Select(tp => tp.PlayerId)
            .ToHashSetAsync();

        foreach (var game in result.GetGames())
        {
            var currentAwards = await _db.WinterStatAwards.Where(a => a.GameId == game.Id).ToListAsync();

            // Remove awards no longer present
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

                // Determine team for this award's player
                var isAwayPlayer = awayTeamPlayerIds.Contains(award.Player.Id);
                var teamId = isAwayPlayer ? result.AwayTeamId : result.HomeTeamId;
                var teamName = isAwayPlayer ? result.AwayTeamName : result.HomeTeamName;

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
