using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.Navigation;

public class EditGroupModel : PageModel
{
    private readonly AppDbContext _db;

    public EditGroupModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int GroupId { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var group = await _db.NavGroups.FindAsync(GroupId);
        if (group == null) return NotFound();
        Title = group.Title;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var group = await _db.NavGroups.FindAsync(GroupId);
        if (group == null) return NotFound();
        group.Title = Title;
        group.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
