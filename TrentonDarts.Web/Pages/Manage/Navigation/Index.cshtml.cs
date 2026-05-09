using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Navigation;

public class NavigationIndexModel : PageModel
{
    private readonly AppDbContext _db;

    public NavigationIndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public List<NavGroup> Groups { get; set; } = new();

    public async Task OnGetAsync()
    {
        Groups = await _db.NavGroups
            .Include(g => g.Items.Where(i => i.DeletedAt == null).OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.BrowsableFile)
            .Where(g => g.DeletedAt == null)
            .OrderBy(g => g.SortOrder)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostMoveGroupUpAsync(int groupId)
    {
        await MoveGroup(groupId, up: true);
        return RedirectToPage(new { leagueId = LeagueId });
    }

    public async Task<IActionResult> OnPostMoveGroupDownAsync(int groupId)
    {
        await MoveGroup(groupId, up: false);
        return RedirectToPage(new { leagueId = LeagueId });
    }

    public async Task<IActionResult> OnPostDeleteGroupAsync(int groupId)
    {
        var group = await _db.NavGroups
            .Include(g => g.Items)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        if (group != null)
        {
            var now = DateTime.UtcNow;
            group.DeletedAt = now;
            foreach (var item in group.Items)
                item.DeletedAt = now;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { leagueId = LeagueId });
    }

    public async Task<IActionResult> OnPostMoveItemUpAsync(int itemId)
    {
        await MoveItem(itemId, up: true);
        return RedirectToPage(new { leagueId = LeagueId });
    }

    public async Task<IActionResult> OnPostMoveItemDownAsync(int itemId)
    {
        await MoveItem(itemId, up: false);
        return RedirectToPage(new { leagueId = LeagueId });
    }

    public async Task<IActionResult> OnPostDeleteItemAsync(int itemId)
    {
        var item = await _db.NavGroupItems.FindAsync(itemId);
        if (item != null)
        {
            item.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage(new { leagueId = LeagueId });
    }

    private async Task MoveGroup(int groupId, bool up)
    {
        var groups = await _db.NavGroups
            .Where(g => g.DeletedAt == null)
            .OrderBy(g => g.SortOrder)
            .ToListAsync();

        var idx = groups.FindIndex(g => g.Id == groupId);
        if (idx < 0) return;
        var swapIdx = up ? idx - 1 : idx + 1;
        if (swapIdx < 0 || swapIdx >= groups.Count) return;

        (groups[idx].SortOrder, groups[swapIdx].SortOrder) = (groups[swapIdx].SortOrder, groups[idx].SortOrder);
        await _db.SaveChangesAsync();
    }

    private async Task MoveItem(int itemId, bool up)
    {
        var item = await _db.NavGroupItems.FindAsync(itemId);
        if (item == null) return;

        var siblings = await _db.NavGroupItems
            .Where(i => i.NavGroupId == item.NavGroupId && i.DeletedAt == null)
            .OrderBy(i => i.SortOrder)
            .ToListAsync();

        var idx = siblings.FindIndex(i => i.Id == itemId);
        if (idx < 0) return;
        var swapIdx = up ? idx - 1 : idx + 1;
        if (swapIdx < 0 || swapIdx >= siblings.Count) return;

        (siblings[idx].SortOrder, siblings[swapIdx].SortOrder) = (siblings[swapIdx].SortOrder, siblings[idx].SortOrder);
        await _db.SaveChangesAsync();
    }
}
