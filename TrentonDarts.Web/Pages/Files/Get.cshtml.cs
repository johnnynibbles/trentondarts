using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Files;

public class GetModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public GetModel(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var file = await _db.BrowsableFiles.FindAsync(id);
        if (file == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath, file.RelativePath ?? "", file.FileName ?? "");
        if (!System.IO.File.Exists(filePath)) return NotFound();

        return PhysicalFile(filePath, file.MimeType ?? "application/octet-stream", file.FileName);
    }
}
