using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Navigation;

public class CreateGroupModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateGroupModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = string.Empty;

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var maxOrder = _db.NavGroups.Any() ? _db.NavGroups.Max(g => g.SortOrder) : 0;
        _db.NavGroups.Add(new NavGroup
        {
            Title = Title,
            SortOrder = maxOrder + 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
