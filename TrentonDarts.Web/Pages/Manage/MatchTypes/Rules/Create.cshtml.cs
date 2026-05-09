using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.MatchTypes.Rules;

public class CreateRuleModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateRuleModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int MatchTypeId { get; set; }

    [BindProperty]
    public GameRuleInput Input { get; set; } = new();

    public string MatchTypeName { get; set; } = string.Empty;
    public List<SelectListItem> GameTypeOptions => GameRuleOptions.GameTypes;
    public List<SelectListItem> WhoStartsOptions => GameRuleOptions.WhoStarts;

    public async Task<IActionResult> OnGetAsync()
    {
        var mt = await _db.MatchTypes.FindAsync(MatchTypeId);
        if (mt == null) return NotFound();
        MatchTypeName = mt.Name;

        Input.OrderId = await _db.MatchTypeGameRules
            .Where(r => r.MatchTypeId == MatchTypeId)
            .Select(r => (int?)r.OrderId).MaxAsync() + 1 ?? 1;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var mt = await _db.MatchTypes.FindAsync(MatchTypeId);
            MatchTypeName = mt?.Name ?? string.Empty;
            return Page();
        }

        _db.MatchTypeGameRules.Add(new MatchTypeGameRule
        {
            MatchTypeId = MatchTypeId,
            GameType = Input.GameType,
            DoubleIn = Input.DoubleIn,
            DoubleOut = Input.DoubleOut,
            OrderId = Input.OrderId,
            BestOfNumberOfLegs = Input.BestOfNumberOfLegs,
            NumberOfLegs = Input.NumberOfLegs,
            WhoStarts = string.IsNullOrEmpty(Input.WhoStarts) ? null : Input.WhoStarts,
            NumberOfPlayers = Input.NumberOfPlayers,
            GamePointValue = Input.GamePointValue,
            LegPointValue = Input.LegPointValue,
            ForfeitIfNoPlayers = Input.ForfeitIfNoPlayers,
            GroupName = Input.GroupName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId, matchTypeId = MatchTypeId });
    }
}
