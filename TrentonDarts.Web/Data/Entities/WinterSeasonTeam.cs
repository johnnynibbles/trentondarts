namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonTeam
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
    public int TeamId { get; set; }
    public string? PreSeasonDiv { get; set; }
    public string? RegularSeasonDiv { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterSeason Season { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public ICollection<WinterSeasonTeamPlayer> TeamPlayers { get; set; } = new List<WinterSeasonTeamPlayer>();
}
