using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.BoardMembers;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public BoardMemberInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var m = await _db.BoardMembers.FindAsync(Id);
        if (m == null) return NotFound();

        Input = new BoardMemberInput
        {
            UserId = m.UserId, Name = m.Name, Position = m.Position,
            StartingSeason = m.StartingSeason, EndingSeason = m.EndingSeason,
            StartSeasonId = m.StartSeasonId, EndSeasonId = m.EndSeasonId
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var m = await _db.BoardMembers.FindAsync(Id);
        if (m == null) return NotFound();

        m.UserId = Input.UserId;
        m.Name = Input.Name;
        m.Position = Input.Position;
        m.StartingSeason = Input.StartingSeason;
        m.EndingSeason = Input.EndingSeason;
        m.StartSeasonId = Input.StartSeasonId;
        m.EndSeasonId = Input.EndSeasonId;
        m.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
