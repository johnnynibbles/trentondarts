namespace TrentonDarts.Web.Data.Entities;

public class NavGroup
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<NavGroupItem> Items { get; set; } = new List<NavGroupItem>();
}
