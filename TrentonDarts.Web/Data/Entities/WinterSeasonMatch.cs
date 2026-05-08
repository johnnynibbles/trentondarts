namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonMatch
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int WeekId { get; set; }
    public int MatchTypeId { get; set; }
    public string? Division { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int LeagueId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterSeason Season { get; set; } = null!;
    public WinterSeasonWeek Week { get; set; } = null!;
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
    public MatchType MatchType { get; set; } = null!;
    public WinterMatchResult? MatchResult { get; set; }
}
