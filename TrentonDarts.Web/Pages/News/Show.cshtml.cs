using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.News;

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;

    public ShowModel(AppDbContext db) => _db = db;

    public NewsPost Post { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string slug)
    {
        var post = await _db.NewsPosts
            .Include(p => p.CoverImage)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.PublishedAt != null);

        if (post == null) return NotFound();

        Post = post;
        return Page();
    }
}
