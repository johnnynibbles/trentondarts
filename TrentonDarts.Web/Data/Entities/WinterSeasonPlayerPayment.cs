namespace TrentonDarts.Web.Data.Entities;

public class WinterSeasonPlayerPayment
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int PlayerId { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
