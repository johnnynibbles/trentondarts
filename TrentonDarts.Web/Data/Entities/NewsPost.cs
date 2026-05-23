namespace TrentonDarts.Web.Data.Entities;

public class NewsPost
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? Html { get; set; }
    public NewsPostType PostType { get; set; } = NewsPostType.News;
    public int? CoverImageId { get; set; }
    public int? WinterSeasonId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public BrowsableFile? CoverImage { get; set; }
    public WinterSeason? WinterSeason { get; set; }

    public bool IsDraft => PublishedAt == null;

    public void Publish() => PublishedAt = DateTime.UtcNow;

    public void Unpublish() => PublishedAt = null;
}
