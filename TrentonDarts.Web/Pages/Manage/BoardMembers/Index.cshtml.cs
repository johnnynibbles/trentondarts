using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.BoardMembers;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<BoardMember> Members { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Members = await _db.BoardMembers
            .Where(m => m.LeagueId == LeagueId)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var member = await _db.BoardMembers.FindAsync(id);
        if (member != null)
        {
            member.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
