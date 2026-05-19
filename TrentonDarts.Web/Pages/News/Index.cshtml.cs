using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.News;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    public const int PageSize = 10;

    public List<NewsPost> Posts { get; private set; } = new();
    public string? Query { get; private set; }
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }

    public async Task OnGetAsync(string? q, int page = 1)
    {
        Query = q?.Trim();
        CurrentPage = Math.Max(1, page);

        var query = _db.NewsPosts
            .Include(p => p.CoverImage)
            .Where(p => p.PublishedAt != null);

        if (!string.IsNullOrEmpty(Query))
            query = query.Where(p => EF.Functions.ILike(p.Title, $"%{Query}%"));

        var total = await query.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        Posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
}
