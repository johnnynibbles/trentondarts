using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Teams;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public WinterSeason Season { get; private set; } = null!;
    public List<WinterSeasonTeam> SeasonTeams { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Season = await _db.WinterSeasons.FirstOrDefaultAsync(s => s.IsCurrent)
            ?? await _db.WinterSeasons.OrderByDescending(s => s.EndYear).FirstAsync();

        SeasonTeams = await _db.WinterSeasonTeams
            .Where(st => st.SeasonId == Season.Id)
            .Include(st => st.Team).ThenInclude(t => t.Sponsor)
            .Include(st => st.TeamPlayers.Where(tp => tp.Role == "Captain" || tp.Role == "Co-Captain"))
                .ThenInclude(tp => tp.Player)
            .ToListAsync();
    }
}
