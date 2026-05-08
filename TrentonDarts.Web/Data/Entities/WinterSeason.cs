namespace TrentonDarts.Web.Data.Entities;

public class WinterSeason
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public string? SeasonType { get; set; }
    public bool IsCurrent { get; set; }
    public int? DefaultMatchTypeId { get; set; }
    public bool IsUsingMatchPoints { get; set; }
    public int WinPoints { get; set; }
    public int HalfPoints { get; set; }
    public int MinPointForHalfPoints { get; set; }
    public bool AccumulatePointsForAllParts { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<WinterSeasonTeam> SeasonTeams { get; set; } = new List<WinterSeasonTeam>();
    public ICollection<WinterSeasonWeek> Weeks { get; set; } = new List<WinterSeasonWeek>();
}
