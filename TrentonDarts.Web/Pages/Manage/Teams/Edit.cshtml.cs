using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Teams;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public TeamInput Input { get; set; } = new();

    public List<Sponsor> Sponsors { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var team = await _db.Teams.FindAsync(Id);
        if (team == null) return NotFound();

        Input = new TeamInput { Name = team.Name, SponsorId = team.SponsorId, Notes = team.Notes };
        Sponsors = await _db.Sponsors.Where(s => s.LeagueId == LeagueId).OrderBy(s => s.Name).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Sponsors = await _db.Sponsors.Where(s => s.LeagueId == LeagueId).OrderBy(s => s.Name).ToListAsync();
            return Page();
        }

        var team = await _db.Teams.FindAsync(Id);
        if (team == null) return NotFound();

        team.Name = Input.Name;
        team.SponsorId = Input.SponsorId;
        team.Notes = Input.Notes;
        team.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
