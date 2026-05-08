namespace TrentonDarts.Web.Domain.Models;

public class GameRules
{
    public int Id { get; set; }
    public string GameType { get; set; } = string.Empty;
    public bool DoubleIn { get; set; }
    public bool DoubleOut { get; set; }
    public int OrderId { get; set; }
    public int? BestOfNumberOfLegs { get; set; }
    public int NumberOfLegs { get; set; }
    public string? WhoStarts { get; set; }
    public int NumberOfPlayers { get; set; }
    public int GamePointValue { get; set; }
    public int? LegPointValue { get; set; }
    public bool ForfeitIfNoPlayers { get; set; }
    public string? GroupName { get; set; }
}
