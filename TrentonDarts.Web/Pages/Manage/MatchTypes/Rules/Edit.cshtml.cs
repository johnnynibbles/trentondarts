using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.MatchTypes.Rules;

public class EditRuleModel : PageModel
{
    private readonly AppDbContext _db;

    public EditRuleModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int MatchTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public GameRuleInput Input { get; set; } = new();

    public string MatchTypeName { get; set; } = string.Empty;
    public List<SelectListItem> GameTypeOptions => GameRuleOptions.GameTypes;
    public List<SelectListItem> WhoStartsOptions => GameRuleOptions.WhoStarts;

    public async Task<IActionResult> OnGetAsync()
    {
        var rule = await _db.MatchTypeGameRules.FindAsync(Id);
        if (rule == null) return NotFound();

        var mt = await _db.MatchTypes.FindAsync(MatchTypeId);
        MatchTypeName = mt?.Name ?? string.Empty;

        Input = new GameRuleInput
        {
            GameType = rule.GameType,
            DoubleIn = rule.DoubleIn,
            DoubleOut = rule.DoubleOut,
            OrderId = rule.OrderId,
            BestOfNumberOfLegs = rule.BestOfNumberOfLegs ?? 0,
            NumberOfLegs = rule.NumberOfLegs,
            WhoStarts = rule.WhoStarts ?? string.Empty,
            NumberOfPlayers = rule.NumberOfPlayers,
            GamePointValue = rule.GamePointValue,
            LegPointValue = rule.LegPointValue ?? 0,
            ForfeitIfNoPlayers = rule.ForfeitIfNoPlayers,
            GroupName = rule.GroupName,
        };
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

        var rule = await _db.MatchTypeGameRules.FindAsync(Id);
        if (rule == null) return NotFound();

        rule.GameType = Input.GameType;
        rule.DoubleIn = Input.DoubleIn;
        rule.DoubleOut = Input.DoubleOut;
        rule.OrderId = Input.OrderId;
        rule.BestOfNumberOfLegs = Input.BestOfNumberOfLegs;
        rule.NumberOfLegs = Input.NumberOfLegs;
        rule.WhoStarts = string.IsNullOrEmpty(Input.WhoStarts) ? null : Input.WhoStarts;
        rule.NumberOfPlayers = Input.NumberOfPlayers;
        rule.GamePointValue = Input.GamePointValue;
        rule.LegPointValue = Input.LegPointValue;
        rule.ForfeitIfNoPlayers = Input.ForfeitIfNoPlayers;
        rule.GroupName = Input.GroupName;
        rule.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId, matchTypeId = MatchTypeId });
    }
}
