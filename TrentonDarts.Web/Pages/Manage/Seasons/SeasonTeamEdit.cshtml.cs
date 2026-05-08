using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class SeasonTeamEditModel : PageModel
{
    private readonly AppDbContext _db;

    public SeasonTeamEditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SeasonTeamId { get; set; }

    [BindProperty]
    public SeasonTeamInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var st = await _db.WinterSeasonTeams.FindAsync(SeasonTeamId);
        if (st == null) return NotFound();

        Input = new SeasonTeamInput
        {
            TeamId = st.TeamId,
            PreSeasonDiv = st.PreSeasonDiv,
            RegularSeasonDiv = st.RegularSeasonDiv
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var st = await _db.WinterSeasonTeams.FindAsync(SeasonTeamId);
        if (st == null) return NotFound();

        st.PreSeasonDiv = Input.PreSeasonDiv;
        st.RegularSeasonDiv = Input.RegularSeasonDiv;
        st.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Show");
    }
}
