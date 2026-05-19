using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonWeek
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
    public SeasonPart WeekType { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterSeason Season { get; set; } = null!;
    public ICollection<WinterSeasonMatch> Matches { get; set; } = new List<WinterSeasonMatch>();
}
