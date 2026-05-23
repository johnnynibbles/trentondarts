using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;
using TrentonDarts.Web.Utilities;

namespace TrentonDarts.Web.Pages.Manage.News;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly BrowsableFileService _files;

    public EditModel(AppDbContext db, BrowsableFileService files)
    {
        _db = db;
        _files = files;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public NewsPostInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? CoverImage { get; set; }

    [BindProperty]
    public bool RemoveCoverImage { get; set; }

    public BrowsableFile? ExistingCoverImage { get; private set; }

    public List<WinterSeason> Seasons { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var post = await _db.NewsPosts
            .Include(p => p.CoverImage)
            .FirstOrDefaultAsync(p => p.Id == Id);
        if (post == null) return NotFound();

        ExistingCoverImage = post.CoverImage;
        await LoadSeasonsAsync();
        Input = new NewsPostInput
        {
            Title = post.Title,
            Slug = post.Slug,
            Excerpt = post.Excerpt,
            Html = post.Html,
            PostType = post.PostType,
            WinterSeasonId = post.WinterSeasonId,
            Publish = !post.IsDraft
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSeasonsAsync();

        var slug = Slug.From(Input.Slug);
        if (string.IsNullOrEmpty(slug))
            ModelState.AddModelError("Input.Slug", "Slug is required.");

        var slugExists = await _db.NewsPosts.AnyAsync(p => p.Slug == slug && p.Id != Id);
        if (slugExists)
            ModelState.AddModelError("Input.Slug", "A post with this slug already exists.");

        if (CoverImage != null && !CoverImage.ContentType.StartsWith("image/"))
            ModelState.AddModelError("CoverImage", "Only image files are allowed.");

        if (!ModelState.IsValid) return Page();

        var post = await _db.NewsPosts.FindAsync(Id);
        if (post == null) return NotFound();

        post.Title = Input.Title;
        post.Slug = slug;
        post.Excerpt = Input.Excerpt;
        post.Html = Input.Html;
        post.PostType = Input.PostType;
        post.WinterSeasonId = Input.WinterSeasonId;
        post.UpdatedAt = DateTime.UtcNow;

        if (RemoveCoverImage)
        {
            post.CoverImageId = null;
        }
        else if (CoverImage != null)
        {
            var file = await _files.UploadImageAsync(CoverImage, Input.Title);
            post.CoverImageId = file.Id;
        }

        if (Input.Publish && post.IsDraft)
            post.Publish();
        else if (!Input.Publish && !post.IsDraft)
            post.Unpublish();

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }

    private async Task LoadSeasonsAsync()
    {
        Seasons = await _db.WinterSeasons
            .OrderByDescending(s => s.StartYear)
            .ThenByDescending(s => s.Id)
            .ToListAsync();
    }
}
