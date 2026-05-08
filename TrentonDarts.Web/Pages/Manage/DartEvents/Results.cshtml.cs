using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.DartEvents;

public class ResultsModel : PageModel
{
    private readonly AppDbContext _db;

    public ResultsModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int EventId { get; set; }

    public string EventName { get; private set; } = "";
    public List<DartEventResult> Results { get; private set; } = new();
    public List<Player> Players { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var ev = await _db.DartEvents.FindAsync(EventId);
        if (ev == null) return NotFound();

        EventName = ev.Name;
        Results = await _db.DartEventResults
            .Where(r => r.EventId == EventId)
            .Include(r => r.Player)
            .OrderBy(r => r.OrderId)
            .ToListAsync();
        Players = await _db.Players
            .Where(p => p.LeagueId == LeagueId)
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int eventId, string? specificEventName, int? playerId, string? finished)
    {
        var maxOrder = await _db.DartEventResults.Where(r => r.EventId == eventId).MaxAsync(r => (int?)r.OrderId) ?? 0;

        _db.DartEventResults.Add(new DartEventResult
        {
            EventId = eventId,
            SpecificEventName = specificEventName,
            PlayerId = playerId,
            Finished = finished,
            OrderId = maxOrder + 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _db.DartEventResults.FindAsync(id);
        if (result != null)
        {
            _db.DartEventResults.Remove(result);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
