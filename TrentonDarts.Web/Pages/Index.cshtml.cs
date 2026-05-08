using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public DartEvent? TitleEvent { get; private set; }
    public string WelcomeMessage { get; private set; } = "";
    public string PlayerOfTheWeek { get; private set; } = "";

    public async Task OnGetAsync()
    {
        TitleEvent = await _db.DartEvents.FirstOrDefaultAsync(e => e.IsTitleEvent == true)
            ?? await _db.DartEvents
                .Where(e => e.EventDate >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .FirstOrDefaultAsync();

        var mainHeader = await _db.PageParts.FirstOrDefaultAsync(p => p.Name == "MainPageHeader");
        WelcomeMessage = mainHeader?.Html ?? "";

        var playerOfWeek = await _db.PageParts.FirstOrDefaultAsync(p => p.Name == "GTDL_PLAYER_OF_THE_WEEK");
        PlayerOfTheWeek = playerOfWeek?.Html ?? "";
    }
}
