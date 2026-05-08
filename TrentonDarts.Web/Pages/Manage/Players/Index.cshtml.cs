using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Players;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<Player> Players { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Players = await _db.Players
            .Where(p => p.LeagueId == LeagueId)
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var player = await _db.Players.FindAsync(id);
        if (player != null)
        {
            player.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
