using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SeasonService _seasonService;

    public IndexModel(AppDbContext db, SeasonService seasonService)
    {
        _db = db;
        _seasonService = seasonService;
    }

    public DartEvent? TitleEvent { get; private set; }
    public string WelcomeMessage { get; private set; } = "";
    public string PlayerOfTheWeek { get; private set; } = "";
    public int? CurrentSeasonId { get; private set; }
    public List<DivStandings> Standings { get; private set; } = new();
    public List<MatchRow> UpcomingMatches { get; private set; } = new();
    public DateTime? UpcomingWeekDate { get; private set; }

    public async Task OnGetAsync()
    {
        TitleEvent = await _db.DartEvents.FirstOrDefaultAsync(e => e.IsTitleEvent == true)
            ?? await _db.DartEvents
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .FirstOrDefaultAsync();

        var mainHeader = await _db.PageParts.FirstOrDefaultAsync(p => p.Name == "MainPageHeader");
        WelcomeMessage = mainHeader?.Html ?? "";

        var playerOfWeek = await _db.PageParts.FirstOrDefaultAsync(p => p.Name == "GTDL_PLAYER_OF_THE_WEEK");
        PlayerOfTheWeek = playerOfWeek?.Html ?? "";

        var currentSeason = await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent);
        CurrentSeasonId = currentSeason?.Id;

        if (currentSeason != null)
        {
            var today = DateTime.Today;

            // Find the nearest upcoming week (>= today), fall back to most recent past week
            var upcomingWeek = await _db.WinterSeasonWeeks
                .Where(w => w.SeasonId == currentSeason.Id && w.Date >= today)
                .OrderBy(w => w.Date)
                .FirstOrDefaultAsync()
                ?? await _db.WinterSeasonWeeks
                    .Where(w => w.SeasonId == currentSeason.Id)
                    .OrderByDescending(w => w.Date)
                    .FirstOrDefaultAsync();

            if (upcomingWeek != null)
            {
                UpcomingWeekDate = upcomingWeek.Date;
                var schedules = await _seasonService.GetWeekScheduleAsync(currentSeason, upcomingWeek.Date);
                UpcomingMatches = schedules.SelectMany(s => s.Weeks).SelectMany(w => w.Matches).ToList();
            }

            var weekDate = await _seasonService.GetCurrentWeekDateAsync(currentSeason, today);
            var part = await _seasonService.GetCurrentSeasonPartAsync(currentSeason, weekDate);
            Standings = await _seasonService.GetDivisionStandingsAsync(currentSeason, part, null);
        }
    }
}
