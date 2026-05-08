namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonTeamPlayer
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
    public int SeasonTeamId { get; set; }
    public int PlayerId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterSeason Season { get; set; } = null!;
    public WinterSeasonTeam SeasonTeam { get; set; } = null!;
    public Player Player { get; set; } = null!;
}
