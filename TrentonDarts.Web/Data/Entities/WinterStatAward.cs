namespace TrentonDarts.Web.Data.Entities;

public class WinterStatAward
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public string? SeasonPart { get; set; }
    public int MatchId { get; set; }
    public string? Division { get; set; }
    public int GameId { get; set; }
    public DateTime Date { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public int PlayerId { get; set; }
    public string? PlayerName { get; set; }
    public int AwardId { get; set; }
    public string? AwardType { get; set; }
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
