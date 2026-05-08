namespace TrentonDarts.Web.Data.Entities;

public class Player
{
    public int Id { get; set; }
    public int LeagueId { get; set; }
    public string? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? HomePhone { get; set; }
    public string? CellPhone { get; set; }
    public string? ShirtSize { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public bool AcceptText { get; set; }
    public bool AcceptEmail { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public string Name => $"{FirstName} {LastName}";
}
