namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonTeamPayment
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int TeamId { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
