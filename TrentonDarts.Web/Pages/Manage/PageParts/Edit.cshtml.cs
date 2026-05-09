using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.PageParts;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public PagePartInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var part = await _db.PageParts.FindAsync(Id);
        if (part == null) return NotFound();

        Input = new PagePartInput { Name = part.Name, Description = part.Description, Html = part.Html };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var part = await _db.PageParts.FindAsync(Id);
        if (part == null) return NotFound();

        part.Name = Input.Name;
        part.Description = Input.Description;
        part.Html = Input.Html;
        part.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
