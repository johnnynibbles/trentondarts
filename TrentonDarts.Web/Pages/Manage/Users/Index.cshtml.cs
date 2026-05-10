using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Users;

[Authorize(Roles = "Owner,Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<User> Users { get; private set; } = [];
    public Dictionary<string, string> UserRoles { get; private set; } = [];
    public Dictionary<string, string> PlayersByUser { get; private set; } = [];
    public Dictionary<string, string> BoardMembersByUser { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Users = await _db.Users.OrderBy(u => u.Name).ToListAsync();

        var roleList = await (
            from ur in _db.UserRoles
            join r in _db.Roles on ur.RoleId equals r.Id
            select new { ur.UserId, r.Name }
        ).ToListAsync();

        UserRoles = roleList
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.First().Name);

        PlayersByUser = await _db.Players
            .Where(p => p.UserId != null && p.LeagueId == LeagueId)
            .ToDictionaryAsync(p => p.UserId!, p => p.Name);

        BoardMembersByUser = await _db.BoardMembers
            .Where(b => b.UserId != null && b.LeagueId == LeagueId)
            .ToDictionaryAsync(b => b.UserId!, b => b.Name);
    }
}
