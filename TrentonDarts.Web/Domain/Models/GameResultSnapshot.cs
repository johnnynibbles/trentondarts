namespace TrentonDarts.Web.Domain.Models;

public class GameResultSnapshot
{
    public int Id { get; set; }
    // Nullable entries preserve position gaps (e.g. position 0 set, position 1 null, position 2 set)
    public List<GamePlayer?> HomePlayers { get; set; } = new();
    public List<GamePlayer?> AwayPlayers { get; set; } = new();
    public List<string> Legs { get; set; } = new();
    public List<GameAward> Awards { get; set; } = new();
    public string? ForfeitedBy { get; set; }
    public GameRules GameRules { get; set; } = new();
}
