using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;

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
        var sid = season?.Id ?? 0;

        var docs = await _db.BrowsableFiles
            .Where(f => f.Category == "document")
            .OrderBy(f => f.Title)
            .ToListAsync();

        var nav = new List<NavItem>
        {
            new() { Title = "Current", Url = "#", SubItems = new List<NavItem>
            {
                new() { Title = "Weekly Standings", Url = $"/season/{sid}" },
                new() { Title = "Full Schedule", Url = $"/season/{sid}/schedule" },
                new() { Title = "Stats", Url = $"/season/{sid}/stats" },
                new() { Title = "Leaderboards", Url = $"/season/{sid}/leaderboard" },
                new() { Title = "Awards", Url = $"/season/{sid}/awards" },
                new() { Title = "Teams", Url = "/teams" },
                new() { Title = "GTDL on DartConnect", Url = "https://tv.dartconnect.com/leaguemenu/gtdl" },
                new() { Title = "GTDL Singles on DartConnect", Url = "https://tv.dartconnect.com/leaguemenu/gtdls" },
            }},
            new() { Title = "Activities and Events", Url = "#", SubItems = new List<NavItem>
            {
                new() { Title = "GTDL Player Results at Events", Url = "/event/results" },
                new() { Title = "Darts for Dreams Info", Url = "/old/charity" },
            }},
            new() { Title = "League", Url = "#", SubItems = new List<NavItem>
            {
                new() { Title = "Where we Play", Url = "/teams" },
                new() { Title = "Sponsors and Partners", Url = "/sponsor" },
            }},
        };

        if (docs.Any())
        {
            nav.Add(new() { Title = "Other", Url = "#", SubItems = docs
                .Select(d => new NavItem { Title = d.Title, Url = $"/file/{d.Id}" })
                .ToList()
            });
        }

        return nav;
    }

    public async Task<bool> IsBoardMemberAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
