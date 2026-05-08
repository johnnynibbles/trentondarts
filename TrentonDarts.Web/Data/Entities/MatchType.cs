namespace TrentonDarts.Web.Data.Entities;

public class MatchType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<MatchTypeGameRule> GameRules { get; set; } = new List<MatchTypeGameRule>();
}
