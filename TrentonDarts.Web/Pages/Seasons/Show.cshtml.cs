using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Seasons;

public class ShowModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SeasonService _seasonService;
    private readonly StatsService _statsService;

    public ShowModel(AppDbContext db, SeasonService seasonService, StatsService statsService)
    {
        _db = db;
        _seasonService = seasonService;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public SeasonPart SeasonPart { get; private set; }
    public DateTime? ResultWeekDate { get; private set; }
    public DateTime? PreviousWeekDate { get; private set; }
    public DateTime? NextWeekDate { get; private set; }
    public List<DivisionSchedule> DivSchedules { get; private set; } = new();
    public List<DivisionSchedule> NextWeekSchedules { get; private set; } = new();
    public List<DivStandings> DivStandings { get; private set; } = new();
    public List<WinterStatAward> Awards { get; private set; } = new();
    public bool ViewAllAwards { get; private set; }
    public bool IsBoardMember { get; private set; }
    public int CurrentWeekNumber { get; private set; }
    public int TotalWeeks { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id,
        [FromQuery] string? week, [FromQuery] bool allAwards = false)
    {
        Season = await _db.WinterSeasons.FindAsync(id)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
            ?? throw new InvalidOperationException("Season not found");

        ViewAllAwards = allAwards;
        IsBoardMember = await IsBoardMemberAsync();

        var today = DateTime.Today;
        ResultWeekDate = week != null && DateTime.TryParse(week, out var parsedWeek)
            ? parsedWeek
            : await _seasonService.GetCurrentWeekDateAsync(Season, today);

        SeasonPart = await _seasonService.GetCurrentSeasonPartAsync(Season, ResultWeekDate);
        PreviousWeekDate = ResultWeekDate.HasValue
            ? await _seasonService.GetPreviousWeekDateAsync(Season, ResultWeekDate.Value)
            : null;
        NextWeekDate = await _seasonService.GetNextWeekDateAsync(Season,
            ResultWeekDate ?? today);

        var allWeekDates = await _db.WinterSeasonWeeks
            .Where(w => w.SeasonId == Season.Id)
            .OrderBy(w => w.Date)
            .Select(w => w.Date)
            .ToListAsync();
        TotalWeeks = allWeekDates.Count;
        CurrentWeekNumber = ResultWeekDate.HasValue
            ? allWeekDates.IndexOf(ResultWeekDate.Value.Date) + 1
            : 0;

        if (ResultWeekDate.HasValue)
            DivSchedules = await _seasonService.GetWeekScheduleAsync(Season, ResultWeekDate.Value);

        NextWeekSchedules = await _seasonService.GetNextWeekScheduleAsync(Season, ResultWeekDate ?? today);

        DivStandings = await _seasonService.GetDivisionStandingsAsync(Season, SeasonPart, ResultWeekDate);

        Awards = await _statsService.GetAwardsForSeasonAsync(
            Season.Id,
            seasonPart: null,
            division: null,
            weekDate: ViewAllAwards ? null : ResultWeekDate);

        return Page();
    }

    private async Task<bool> IsBoardMemberAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
