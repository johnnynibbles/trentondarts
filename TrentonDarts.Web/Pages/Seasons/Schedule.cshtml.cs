using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Seasons;

public class ScheduleModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly SeasonService _seasonService;

    public ScheduleModel(AppDbContext db, SeasonService seasonService)
    {
        _db = db;
        _seasonService = seasonService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public List<DivisionSchedule> PreSchedules { get; private set; } = new();
    public List<DivisionSchedule> RegularSchedules { get; private set; } = new();
    public bool IsBoardMember { get; private set; }

    public async Task<IActionResult> OnGetAsync(int seasonId)
    {
        Season = await _db.WinterSeasons.FindAsync(seasonId)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent);
        if (Season == null) return NotFound();

        IsBoardMember = await IsBoardMemberAsync();

        var (pre, regular) = await _seasonService.GetFullScheduleAsync(Season);
        PreSchedules = pre;
        RegularSchedules = regular;

        return Page();
    }

    private async Task<bool> IsBoardMemberAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return false;
        return await _db.BoardMembers.AnyAsync(b => b.UserId == userId);
    }
}
