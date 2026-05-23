using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;
using TrentonDarts.Web.Utilities;

namespace TrentonDarts.Web.Pages.Manage.News;

public class NewsPostInput
{
    [Required] public string Title { get; set; } = "";
    [Required] public string Slug { get; set; } = "";
    public string? Excerpt { get; set; }
    public string? Html { get; set; }
    public NewsPostType PostType { get; set; } = NewsPostType.News;
    public int? WinterSeasonId { get; set; }
    public bool Publish { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly BrowsableFileService _files;

    public CreateModel(AppDbContext db, BrowsableFileService files)
    {
        _db = db;
        _files = files;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SeasonId { get; set; }

    [BindProperty(SupportsGet = true)]
    public NewsPostType? PostType { get; set; }

    [BindProperty]
    public NewsPostInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? CoverImage { get; set; }

    public List<WinterSeason> Seasons { get; private set; } = new();

    public async Task OnGetAsync()
    {
        await LoadSeasonsAsync();
        if (SeasonId.HasValue) Input.WinterSeasonId = SeasonId;
        if (PostType.HasValue) Input.PostType = PostType.Value;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSeasonsAsync();

        var slug = Slug.From(Input.Slug);
        if (string.IsNullOrEmpty(slug))
            ModelState.AddModelError("Input.Slug", "Slug is required.");

        var slugExists = await _db.NewsPosts.AnyAsync(p => p.Slug == slug);
        if (slugExists)
            ModelState.AddModelError("Input.Slug", "A post with this slug already exists.");

        if (CoverImage != null && !CoverImage.ContentType.StartsWith("image/"))
            ModelState.AddModelError("CoverImage", "Only image files are allowed.");

        if (!ModelState.IsValid) return Page();

        int? coverImageId = null;
        if (CoverImage != null)
        {
            var file = await _files.UploadImageAsync(CoverImage, Input.Title);
            coverImageId = file.Id;
        }

        var post = new NewsPost
        {
            Title = Input.Title,
            Slug = slug,
            Excerpt = Input.Excerpt,
            Html = Input.Html,
            PostType = Input.PostType,
            CoverImageId = coverImageId,
            WinterSeasonId = Input.WinterSeasonId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (Input.Publish)
            post.Publish();

        _db.NewsPosts.Add(post);
        await _db.SaveChangesAsync();

        if (SeasonId.HasValue)
            return Redirect($"/Manage/{LeagueId}/seasons/{SeasonId.Value}");

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
