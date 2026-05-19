using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage;

public class UploadImageModel : PageModel
{
    private readonly BrowsableFileService _files;

    public UploadImageModel(BrowsableFileService files) => _files = files;

    public async Task<IActionResult> OnPostAsync(IFormFile file)
    {
        if (file == null || !file.ContentType.StartsWith("image/"))
            return BadRequest(new { error = "Invalid file." });

        var result = await _files.UploadImageAsync(file, file.FileName);
        return new JsonResult(new { url = $"/file/{result.Id}" });
    }
}
