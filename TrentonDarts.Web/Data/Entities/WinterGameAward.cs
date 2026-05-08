namespace TrentonDarts.Web.Data.Entities;

public class WinterGameAward
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public string AwardType { get; set; } = string.Empty;
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public WinterGameResult GameResult { get; set; } = null!;
    public Player Player { get; set; } = null!;
}
