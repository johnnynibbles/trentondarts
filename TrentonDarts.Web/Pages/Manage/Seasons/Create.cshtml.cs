using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using MatchType = TrentonDarts.Web.Data.Entities.MatchType;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class SeasonInput
{
    [Required] public string Name { get; set; } = "";
    [Required] public int StartYear { get; set; } = DateTime.Now.Year;
    [Required] public int EndYear { get; set; } = DateTime.Now.Year + 1;
    public string? SeasonType { get; set; }
    public bool IsCurrent { get; set; }
    public int? DefaultMatchTypeId { get; set; }
    public bool IsUsingMatchPoints { get; set; }
    public int WinPoints { get; set; } = 2;
    public int HalfPoints { get; set; } = 1;
    public int MinPointForHalfPoints { get; set; } = 7;
    public bool AccumulatePointsForAllParts { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public SeasonInput Input { get; set; } = new();

    public List<MatchType> MatchTypes { get; private set; } = new();

    public async Task OnGetAsync()
    {
        MatchTypes = await _db.MatchTypes.OrderBy(m => m.Name).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            MatchTypes = await _db.MatchTypes.OrderBy(m => m.Name).ToListAsync();
            return Page();
        }

        _db.WinterSeasons.Add(new WinterSeason
        {
            LeagueId = LeagueId,
            Name = Input.Name,
            StartYear = Input.StartYear,
            EndYear = Input.EndYear,
            SeasonType = Input.SeasonType,
            IsCurrent = Input.IsCurrent,
            DefaultMatchTypeId = Input.DefaultMatchTypeId,
            IsUsingMatchPoints = Input.IsUsingMatchPoints,
            WinPoints = Input.WinPoints,
            HalfPoints = Input.HalfPoints,
            MinPointForHalfPoints = Input.MinPointForHalfPoints,
            AccumulatePointsForAllParts = Input.AccumulatePointsForAllParts,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
