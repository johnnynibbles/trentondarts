using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using MatchType = TrentonDarts.Web.Data.Entities.MatchType;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public SeasonInput Input { get; set; } = new();

    public List<MatchType> MatchTypes { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var s = await _db.WinterSeasons.FindAsync(Id);
        if (s == null) return NotFound();

        Input = new SeasonInput
        {
            Name = s.Name, StartYear = s.StartYear, EndYear = s.EndYear,
            SeasonType = s.SeasonType, IsCurrent = s.IsCurrent,
            DefaultMatchTypeId = s.DefaultMatchTypeId,
            IsUsingMatchPoints = s.IsUsingMatchPoints,
            WinPoints = s.WinPoints, HalfPoints = s.HalfPoints,
            MinPointForHalfPoints = s.MinPointForHalfPoints,
            AccumulatePointsForAllParts = s.AccumulatePointsForAllParts
        };
        MatchTypes = await _db.MatchTypes.OrderBy(m => m.Name).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            MatchTypes = await _db.MatchTypes.OrderBy(m => m.Name).ToListAsync();
            return Page();
        }

        var s = await _db.WinterSeasons.FindAsync(Id);
        if (s == null) return NotFound();

        s.Name = Input.Name;
        s.StartYear = Input.StartYear;
        s.EndYear = Input.EndYear;
        s.SeasonType = Input.SeasonType;
        s.IsCurrent = Input.IsCurrent;
        s.DefaultMatchTypeId = Input.DefaultMatchTypeId;
        s.IsUsingMatchPoints = Input.IsUsingMatchPoints;
        s.WinPoints = Input.WinPoints;
        s.HalfPoints = Input.HalfPoints;
        s.MinPointForHalfPoints = Input.MinPointForHalfPoints;
        s.AccumulatePointsForAllParts = Input.AccumulatePointsForAllParts;
        s.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
