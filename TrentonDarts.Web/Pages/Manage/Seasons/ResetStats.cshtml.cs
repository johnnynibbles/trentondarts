using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

[Authorize(Roles = "Owner,Admin")]
public class ResetStatsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly MatchRepository _matchRepo;
    private readonly UpdateMatchStatsService _statsService;

    public ResetStatsModel(AppDbContext db, MatchRepository matchRepo, UpdateMatchStatsService statsService)
    {
        _db = db;
        _matchRepo = matchRepo;
        _statsService = statsService;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public IActionResult OnGet() =>
        RedirectToPage("Show", new { leagueId = LeagueId, id = Id });

    public async Task<IActionResult> OnPostAsync()
    {
        var matches = await _db.WinterSeasonMatches
            .Where(m => m.SeasonId == Id)
            .ToListAsync();

        foreach (var match in matches)
        {
            var matchResult = await _matchRepo.GetMatchResultsFromMatchAsync(match);
            await _statsService.UpdateAsync(matchResult);
        }

        return RedirectToPage("Show", new { leagueId = LeagueId, id = Id });
    }
}
