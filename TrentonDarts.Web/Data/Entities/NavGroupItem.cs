namespace TrentonDarts.Web.Data.Entities;

public enum NavItemType { InternalLink, ExternalUrl, Document }

public class NavGroupItem
{
    public int Id { get; set; }
    public int NavGroupId { get; set; }
    public NavGroup NavGroup { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public NavItemType ItemType { get; set; }
    public string? UrlTemplate { get; set; }
    public int? BrowsableFileId { get; set; }
    public BrowsableFile? BrowsableFile { get; set; }
    public bool IsHeader { get; set; }
    public bool IsSeparator { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
