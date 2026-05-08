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

        return new List<NavItem>
        {
            new() { Title = "Current", Url = "#", SubItems = new List<NavItem>
            {
                new() { Title = "Weekly Standings", Url = $"/season/{sid}" },
                new() { Title = "Full Schedule", Url = $"/season/{sid}/schedule" },
                new() { Title = "Stats", Url = $"/season/{sid}/stats" },
                new() { Title = "Leaderboards", Url = $"/season/{sid}/leaderboard" },
                new() { Title = "Awards", Url = $"/season/{sid}/awards" },
                new() { Title = "Teams", Url = "/team" },
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
                new() { Title = "Where we Play", Url = "/team" },
                new() { Title = "Sponsors and Partners", Url = "/sponsor" },
            }},
            new() { Title = "Other", Url = "#", SubItems = new List<NavItem>
            {
                new() { Title = "League Rules", Url = "/documents/static/gtdlrules.pdf" },
                new() { Title = "Scoresheet", Url = "/documents/static/scoresheet.pdf" },
                new() { Title = "01 Strategy", Url = "/documents/static/playersseries1.pdf" },
                new() { Title = "Cricket Strategy", Url = "/documents/static/playersseries2.pdf" },
                new() { Title = "Advanced 01 Strategy", Url = "/documents/static/playerseries3.pdf" },
            }},
        };
    }

    public async Task<bool> IsBoardMemberAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
