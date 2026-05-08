namespace TrentonDarts.Web.Data.Entities;

public class WinterMatchResult
{
    // Id == matchId (FK to WinterSeasonMatch, not auto-generated)
    public int Id { get; set; }
    public bool HasScorecard { get; set; }
    public int? AwayScoreOverride { get; set; }
    public int? HomeScoreOverride { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterSeasonMatch Match { get; set; } = null!;
    public ICollection<WinterGameResult> GameResults { get; set; } = new List<WinterGameResult>();
}
