namespace TrentonDarts.Web.Domain.Models;

public class GameAward
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public GamePlayer? Player { get; set; }
    public string AwardType { get; set; } = string.Empty;
    public int Value { get; set; }
}
