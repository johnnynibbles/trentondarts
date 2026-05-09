using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.DartEvents;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public DartEventInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var ev = await _db.DartEvents.FindAsync(Id);
        if (ev == null) return NotFound();

        Input = new DartEventInput
        {
            Name = ev.Name,
            EventTypeId = ev.EventTypeId,
            EventDate = ev.EventDate,
            EventEndDate = ev.EventEndDate,
            DartType = ev.DartType,
            EventContact = ev.EventContact,
            EventContact2 = ev.EventContact2,
            HostName = ev.HostName,
            HostUrl = ev.HostUrl,
            HostPhone = ev.HostPhone,
            LocationName = ev.LocationName,
            Address1 = ev.Address1,
            Address2 = ev.Address2,
            City = ev.City,
            State = ev.State,
            Zip = ev.Zip,
            Url = ev.Url,
            FacebookUrl = ev.FacebookUrl,
            RegistrationStartTime = ev.RegistrationStartTime,
            RegistrationEndTime = ev.RegistrationEndTime,
            DartStart = ev.DartStart,
            MapUrl = ev.MapUrl,
            Description = ev.Description,
            IsTitleEvent = ev.IsTitleEvent ?? false
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var ev = await _db.DartEvents.FindAsync(Id);
        if (ev == null) return NotFound();

        ev.Name = Input.Name;
        ev.EventTypeId = Input.EventTypeId;
        ev.EventDate = Input.EventDate;
        ev.EventEndDate = Input.EventEndDate;
        ev.DartType = Input.DartType;
        ev.EventContact = Input.EventContact;
        ev.EventContact2 = Input.EventContact2;
        ev.HostName = Input.HostName;
        ev.HostUrl = Input.HostUrl;
        ev.HostPhone = Input.HostPhone;
        ev.LocationName = Input.LocationName;
        ev.Address1 = Input.Address1;
        ev.Address2 = Input.Address2;
        ev.City = Input.City;
        ev.State = Input.State;
        ev.Zip = Input.Zip;
        ev.Url = Input.Url;
        ev.FacebookUrl = Input.FacebookUrl;
        ev.RegistrationStartTime = Input.RegistrationStartTime;
        ev.RegistrationEndTime = Input.RegistrationEndTime;
        ev.DartStart = Input.DartStart;
        ev.MapUrl = Input.MapUrl;
        ev.Description = Input.Description;
        ev.IsTitleEvent = Input.IsTitleEvent;
        ev.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
