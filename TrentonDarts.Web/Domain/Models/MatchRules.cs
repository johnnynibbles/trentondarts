namespace TrentonDarts.Web.Domain.Models;

public class MatchRules
{
    public int Id { get; set; }
    public bool IsUsingMatchPoints { get; set; }
    public int WinPoints { get; set; }
    public int HalfPoints { get; set; }
    public int MinPointForHalfPoints { get; set; }
    public List<GameRules> GameRules { get; set; } = new();
}
