using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class WeekInput
{
    [Required] public DateTime Date { get; set; } = DateTime.Today;
    [Required] public string WeekType { get; set; } = "regular";
}

public class WeekCreateModel : PageModel
{
    private readonly AppDbContext _db;

    public WeekCreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public WeekInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.WinterSeasonWeeks.Add(new WinterSeasonWeek
        {
            LeagueId = LeagueId,
            SeasonId = Id,
            Date = Input.Date,
            WeekType = Enum.Parse<SeasonPart>(Input.WeekType, ignoreCase: true),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Show", new { leagueId = LeagueId, id = Id });
    }
}
