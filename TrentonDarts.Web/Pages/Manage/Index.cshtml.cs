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
        await Task.WhenAll(
            _db.Players.CountAsync().ContinueWith(t => PlayerCount = t.Result),
            _db.Teams.CountAsync().ContinueWith(t => TeamCount = t.Result),
            _db.Sponsors.CountAsync().ContinueWith(t => SponsorCount = t.Result),
            _db.BoardMembers.CountAsync().ContinueWith(t => BoardMemberCount = t.Result),
            _db.MatchTypes.CountAsync().ContinueWith(t => MatchTypeCount = t.Result),
            _db.DartEvents.CountAsync().ContinueWith(t => DartEventCount = t.Result),
            _db.PageParts.CountAsync().ContinueWith(t => PagePartCount = t.Result),
            _db.BrowsableFiles.CountAsync().ContinueWith(t => DocumentCount = t.Result),
            _db.NavGroups.CountAsync().ContinueWith(t => NavGroupCount = t.Result),
            _db.WinterSeasons.CountAsync().ContinueWith(t => SeasonCount = t.Result)
        );
    }
}
