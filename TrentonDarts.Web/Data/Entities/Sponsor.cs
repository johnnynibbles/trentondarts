namespace TrentonDarts.Web.Data.Entities;

public class Sponsor
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Phone { get; set; }
    public string? Url { get; set; }
    public string? FacebookUrl { get; set; }
    public string? Email { get; set; }
    public string? MapUrl { get; set; }
    public string? Description { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();

    public static readonly Dictionary<string, string> SponsorTypes = new()
    {
        { "L", "League Sponsors" },
        { "P", "Player Companies" },
        { "C", "Charity Partners" },
        { "T", "Team Sponsors" }
    };
}
