using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Files;

public class GetModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _storage;

    public GetModel(AppDbContext db, IFileStorageService storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        BrowsableFile? file;
        if (int.TryParse(slug, out var id))
            file = await _db.BrowsableFiles.FindAsync(id);
        else
            file = await _db.BrowsableFiles.FirstOrDefaultAsync(f => f.Slug == slug);

        if (file == null || string.IsNullOrEmpty(file.RelativePath)) return NotFound();

        var (stream, contentType) = await _storage.DownloadAsync(file.RelativePath);
        Response.Headers.ContentDisposition =
            $"inline; filename*=UTF-8''{Uri.EscapeDataString(file.FileName)}";
        return File(stream, contentType);
    }
}
