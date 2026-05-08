namespace TrentonDarts.Web.Data.Entities;

public class BoardMember
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? StartingSeason { get; set; }
    public string? EndingSeason { get; set; }
    public int? StartSeasonId { get; set; }
    public int? EndSeasonId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
