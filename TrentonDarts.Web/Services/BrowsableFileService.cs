using Microsoft.AspNetCore.Http;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Services;

public class BrowsableFileService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _storage;

    public BrowsableFileService(AppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<BrowsableFile> UploadDocumentAsync(IFormFile file, string title, string? slug = null)
    {
        var ext = Path.GetExtension(file.FileName);
        var objectKey = $"documents/{Guid.NewGuid()}{ext}";

        using var stream = file.OpenReadStream();
        await _storage.UploadAsync(stream, objectKey, file.ContentType);

        var record = new BrowsableFile
        {
            Title = title,
            Slug = string.IsNullOrWhiteSpace(slug) ? null : slug.ToLowerInvariant(),
            Category = "document",
            FileName = file.FileName,
            RelativePath = objectKey,
            MimeType = file.ContentType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.BrowsableFiles.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public async Task DeleteAsync(int id)
    {
        var file = await _db.BrowsableFiles.FindAsync(id);
        if (file == null) return;

        file.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(file.RelativePath))
            await _storage.DeleteAsync(file.RelativePath);
    }
}
