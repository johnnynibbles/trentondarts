using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    // Active season spotlight
    public WinterSeason? CurrentSeason { get; private set; }
    public int CurrentSeasonTeamCount { get; private set; }
    public int CurrentSeasonTotalWeeks { get; private set; }
    public int CurrentSeasonWeeksPlayed { get; private set; }

    // Tile counts
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
        CurrentSeason = await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent);
        if (CurrentSeason != null)
        {
            CurrentSeasonTeamCount = await _db.WinterSeasonTeams.CountAsync(t => t.SeasonId == CurrentSeason.Id);
            var weekDates = await _db.WinterSeasonWeeks
                .Where(w => w.SeasonId == CurrentSeason.Id)
                .Select(w => w.Date)
                .OrderBy(d => d)
                .ToListAsync();
            CurrentSeasonTotalWeeks = weekDates.Count;
            CurrentSeasonWeeksPlayed = weekDates.Count(d => d <= DateTime.Today);
        }

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
