namespace TrentonDarts.Web.Data.Entities;

public class WinterGameResult
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    // Semicolon-delimited player IDs — split/join in domain layer only
    public string? HomePlayers { get; set; }
    public string? AwayPlayers { get; set; }
    public string? Legs { get; set; }
    public string? ForfeitedBy { get; set; }
    public int GameRuleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterMatchResult MatchResult { get; set; } = null!;
    public MatchTypeGameRule GameRule { get; set; } = null!;
    public ICollection<WinterGameAward> Awards { get; set; } = new List<WinterGameAward>();
}
