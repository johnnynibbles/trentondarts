using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;
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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var file = await _db.BrowsableFiles.FindAsync(id);
        if (file == null || string.IsNullOrEmpty(file.RelativePath)) return NotFound();

        var (stream, contentType) = await _storage.DownloadAsync(file.RelativePath);
        Response.Headers.ContentDisposition =
            $"inline; filename*=UTF-8''{Uri.EscapeDataString(file.FileName)}";
        return File(stream, contentType);
    }
}
