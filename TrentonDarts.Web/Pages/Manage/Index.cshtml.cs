using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public int PlayerCount { get; private set; }
    public int TeamCount { get; private set; }
    public int SponsorCount { get; private set; }
    public int BoardMemberCount { get; private set; }
    public int MatchTypeCount { get; private set; }
    public int DartEventCount { get; private set; }
    public int PagePartCount { get; private set; }
    public int DocumentCount { get; private set; }
    public int NavGroupCount { get; private set; }
    public int SeasonCount { get; private set; }

    public async Task OnGetAsync()
    {
        PlayerCount = await _db.Players.CountAsync();
        TeamCount = await _db.Teams.CountAsync();
        SponsorCount = await _db.Sponsors.CountAsync();
        BoardMemberCount = await _db.BoardMembers.CountAsync();
        MatchTypeCount = await _db.MatchTypes.CountAsync();
        DartEventCount = await _db.DartEvents.CountAsync();
        PagePartCount = await _db.PageParts.CountAsync();
        DocumentCount = await _db.BrowsableFiles.CountAsync();
        NavGroupCount = await _db.NavGroups.CountAsync();
        SeasonCount = await _db.WinterSeasons.CountAsync();
    }
}
