using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Data.Entities;

public class WinterStatTeamGame
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public SeasonPart? SeasonPart { get; set; }
    public int MatchId { get; set; }
    public string? Division { get; set; }
    public int GameId { get; set; }
    public DateTime Date { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? GameType { get; set; }
    public int NumberOfPlayers { get; set; }
    public int NumberOfPoints { get; set; }
    public bool IsWon { get; set; }
    public bool IsForfeitGame { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
