using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Teams;

public class TeamInput
{
    [Required] public string Name { get; set; } = "";
    public int? SponsorId { get; set; }
    public string? Notes { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public TeamInput Input { get; set; } = new();

    public List<Sponsor> Sponsors { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Sponsors = await _db.Sponsors.Where(s => s.LeagueId == LeagueId).OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Sponsors = await _db.Sponsors.Where(s => s.LeagueId == LeagueId).OrderBy(s => s.Name).ToListAsync();
            return Page();
        }

        _db.Teams.Add(new Team
        {
            LeagueId = LeagueId,
            Name = Input.Name,
            SponsorId = Input.SponsorId,
            Notes = Input.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
