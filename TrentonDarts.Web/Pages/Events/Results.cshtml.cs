using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Events;

public class EventResultRow
{
    public string EventName { get; set; } = "";
    public DateTime? EventDate { get; set; }
    public string SpecificEvent { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string Place { get; set; } = "";
}

public class ResultsModel : PageModel
{
    private readonly AppDbContext _db;

    public ResultsModel(AppDbContext db)
    {
        _db = db;
    }

    public List<EventResultRow> EventResults { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var events = await _db.DartEvents
            .Where(e => e.EventDate != null && e.EventDate <= DateTime.Today)
            .Include(e => e.Results).ThenInclude(r => r.Player)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();

        foreach (var ev in events)
        {
            foreach (var r in ev.Results)
            {
                EventResults.Add(new EventResultRow
                {
                    EventName = ev.Name,
                    EventDate = ev.EventDate,
                    SpecificEvent = r.SpecificEventName ?? "",
                    PlayerName = r.Player?.Name ?? "",
                    Place = r.Finished ?? ""
                });
            }
        }
    }
}
