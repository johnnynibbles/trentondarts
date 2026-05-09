namespace TrentonDarts.Web.Data.Entities;

public class BrowsableFile
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Category { get; set; }
    public string? FileName { get; set; }
    public string? RelativePath { get; set; }
    public string? MimeType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
