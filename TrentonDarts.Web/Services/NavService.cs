using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Services;

public class NavItem
{
    public string Title { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public bool IsHeader { get; init; }
    public bool IsSeparator { get; init; }
    public List<NavItem> SubItems { get; init; } = new();
}

public class NavService
{
    private readonly AppDbContext _db;

    public NavService(AppDbContext db) => _db = db;

    public async Task<List<NavItem>> GetDefaultNavAsync()
    {
        var season = await _db.WinterSeasons.Where(s => s.IsCurrent).FirstOrDefaultAsync();
        var sid = season?.Id.ToString() ?? "0";

        var groups = await _db.NavGroups
            .Include(g => g.Items.Where(i => i.DeletedAt == null).OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.BrowsableFile)
            .Where(g => g.DeletedAt == null)
            .OrderBy(g => g.SortOrder)
            .ToListAsync();

        var nav = new List<NavItem>();
        foreach (var group in groups)
        {
            var subItems = group.Items
                .Where(i => i.ItemType != NavItemType.Document || i.BrowsableFile != null)
                .Select(i => new NavItem
                {
                    Title = i.Title,
                    Url = ResolveUrl(i, sid),
                    IsHeader = i.IsHeader,
                    IsSeparator = i.IsSeparator,
                })
                .ToList();

            if (!subItems.Any()) continue;

            nav.Add(new() { Title = group.Title, Url = "#", SubItems = subItems });
        }
        return nav;
    }

    public async Task<bool> IsBoardMemberAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }

    private static string ResolveUrl(NavGroupItem item, string seasonId) =>
        item.ItemType == NavItemType.Document
            ? $"/file/{item.BrowsableFileId}"
            : (item.UrlTemplate ?? "#").Replace("{currentSeasonId}", seasonId);
}
