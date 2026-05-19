using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Data.Entities;

public class WinterStatMatch
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public SeasonPart? SeasonPart { get; set; }
    public int MatchId { get; set; }
    public string? Division { get; set; }
    public DateTime Date { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public int PointsWon { get; set; }
    public int PointsLost { get; set; }
    public int MatchPoints { get; set; }
    public bool HomeMatch { get; set; }
    public bool HasScorecard { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
