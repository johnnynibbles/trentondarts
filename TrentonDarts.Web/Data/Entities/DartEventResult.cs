namespace TrentonDarts.Web.Data.Entities;

public class DartEventResult
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string? SpecificEventName { get; set; }
    public int? PlayerId { get; set; }
    public string? Finished { get; set; }
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public DartEvent Event { get; set; } = null!;
    public Player? Player { get; set; }
}
