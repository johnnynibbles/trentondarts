namespace TrentonDarts.Web.Domain.Models;

public class MatchResultSnapshot
{
    public int MatchId { get; set; }
    public int SeasonId { get; set; }
    public string? SeasonPart { get; set; }
    public string? Division { get; set; }
    public DateTime Date { get; set; }
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public bool HasScorecard { get; set; }
    public int AwayScoreOverride { get; set; }
    public int HomeScoreOverride { get; set; }
    public List<GameResultSnapshot> GameResults { get; set; } = new();
}
