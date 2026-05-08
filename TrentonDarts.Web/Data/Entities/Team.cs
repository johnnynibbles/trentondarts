namespace TrentonDarts.Web.Data.Entities;

public class Team
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? SponsorId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Sponsor? Sponsor { get; set; }
}
