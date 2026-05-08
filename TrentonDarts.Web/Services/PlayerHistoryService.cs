using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Web.Services;

public class PlayerGameHistory
{
    public int MatchId { get; set; }
    public DateTime Date { get; set; }
    public string GameType { get; set; } = "";
    public int GameOrder { get; set; }
    public string SeasonPart { get; set; } = "";
    public bool IsHomeGame { get; set; }
    public List<GamePlayer> TeamPlayers { get; set; } = new();
    public List<GamePlayer> Opponents { get; set; } = new();
    public string OpponentTeamName { get; set; } = "";
    public int OpponentTeamId { get; set; }
    public bool IsWon { get; set; }
}

public class PlayerHistoryService
{
    private readonly AppDbContext _db;

    public PlayerHistoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlayerGameHistory>> GetPlayerGameHistoryAsync(int seasonId, int playerId)
    {
        var playerGames = await _db.WinterStatPlayerGames
            .Where(g => g.SeasonId == seasonId && g.PlayerId == playerId && !g.IsForfeit)
            .OrderBy(g => g.Date).ThenBy(g => g.GameNumber).ThenBy(g => g.PlayerPosition)
            .Select(g => g.GameId)
            .ToListAsync();

        var allGames = await _db.WinterStatPlayerGames
            .Where(g => playerGames.Contains(g.GameId))
            .ToListAsync();

        var histories = new List<PlayerGameHistory>();

        foreach (var gameId in playerGames)
        {
            var playerGame = allGames.FirstOrDefault(g => g.GameId == gameId && g.PlayerId == playerId);
            if (playerGame == null) continue;

            var history = new PlayerGameHistory
            {
                MatchId = playerGame.MatchId,
                Date = playerGame.Date,
                GameType = GetHistoryGameType(playerGame.NumberOfPlayers, playerGame.GameType),
                GameOrder = playerGame.GameNumber,
                SeasonPart = playerGame.SeasonPart ?? "",
                IsHomeGame = playerGame.IsHome,
                IsWon = playerGame.IsWon
            };

            foreach (var g in allGames
                .Where(g => g.GameId == gameId && g.TeamId == playerGame.TeamId && g.PlayerId != playerId)
                .OrderBy(g => g.PlayerPosition))
            {
                history.TeamPlayers.Add(new GamePlayer { Id = g.PlayerId, Name = g.PlayerName ?? "" });
            }

            foreach (var g in allGames
                .Where(g => g.GameId == gameId && g.TeamId != playerGame.TeamId)
                .OrderBy(g => g.PlayerPosition))
            {
                history.OpponentTeamName = g.TeamName ?? "";
                history.OpponentTeamId = g.TeamId;
                history.Opponents.Add(new GamePlayer { Id = g.PlayerId, Name = g.PlayerName ?? "" });
            }

            histories.Add(history);
        }

        return histories;
    }

    private static string GetHistoryGameType(int numberOfPlayers, string? gameType)
    {
        var name = numberOfPlayers switch
        {
            2 => "D-",
            3 => "T-",
            _ => ""
        };

        name += (gameType == "cricket" || gameType == "doublecricket") ? "Cricket" : (gameType ?? "");
        return name;
    }
}
