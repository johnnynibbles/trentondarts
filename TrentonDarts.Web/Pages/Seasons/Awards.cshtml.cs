using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Seasons;

public class AwardsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly StatsService _statsService;

    public AwardsModel(AppDbContext db, StatsService statsService)
    {
        _db = db;
        _statsService = statsService;
    }

    public WinterSeason Season { get; private set; } = null!;
    public string SeasonPart { get; private set; } = "whole";
    public string Division { get; private set; } = "";
    public Dictionary<string, string> Divisions { get; private set; } = new();
    public List<WinterStatAward> Awards { get; private set; } = new();

    public async Task OnGetAsync(int seasonId,
        [FromQuery] string seasonPart = "whole",
        [FromQuery] string division = "")
    {
        Season = await _db.WinterSeasons.FindAsync(seasonId)
            ?? await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
            ?? throw new InvalidOperationException("Season not found");

        SeasonPart = seasonPart;
        Division = division;

        Divisions = await GetDivisionsAsync(seasonId);
        Awards = await _statsService.GetAwardsForSeasonAsync(
            seasonId,
            seasonPart == "whole" ? null : seasonPart,
            division == "" ? null : division,
            weekDate: null);
    }

    private async Task<Dictionary<string, string>> GetDivisionsAsync(int seasonId)
    {
        var pre = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == seasonId && st.PreSeasonDiv != null && st.PreSeasonDiv != "")
            .Select(st => st.PreSeasonDiv!)
            .Distinct().OrderBy(d => d).ToListAsync();

        var regular = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == seasonId && st.RegularSeasonDiv != null && st.RegularSeasonDiv != "")
            .Select(st => st.RegularSeasonDiv!)
            .Distinct().OrderBy(d => d).ToListAsync();

        return pre.Union(regular).Distinct().OrderBy(d => d)
            .ToDictionary(d => d, d => d);
    }
}
