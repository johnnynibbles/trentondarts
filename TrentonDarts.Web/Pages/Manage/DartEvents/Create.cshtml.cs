using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.DartEvents;

public class DartEventInput
{
    [Required] public string Name { get; set; } = "";
    public int? EventTypeId { get; set; }
    public DateTime? EventDate { get; set; }
    public DateTime? EventEndDate { get; set; }
    public string? DartType { get; set; }
    public string? EventContact { get; set; }
    public string? EventContact2 { get; set; }
    public string? HostName { get; set; }
    public string? HostUrl { get; set; }
    public string? HostPhone { get; set; }
    public string? LocationName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Url { get; set; }
    public string? FacebookUrl { get; set; }
    public string? RegistrationStartTime { get; set; }
    public string? RegistrationEndTime { get; set; }
    public string? DartStart { get; set; }
    public string? MapUrl { get; set; }
    public string? Description { get; set; }
    public bool IsTitleEvent { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public DartEventInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.DartEvents.Add(new DartEvent
        {
            Name = Input.Name,
            EventTypeId = Input.EventTypeId,
            EventDate = Input.EventDate,
            EventEndDate = Input.EventEndDate,
            DartType = Input.DartType,
            EventContact = Input.EventContact,
            EventContact2 = Input.EventContact2,
            HostName = Input.HostName,
            HostUrl = Input.HostUrl,
            HostPhone = Input.HostPhone,
            LocationName = Input.LocationName,
            Address1 = Input.Address1,
            Address2 = Input.Address2,
            City = Input.City,
            State = Input.State,
            Zip = Input.Zip,
            Url = Input.Url,
            FacebookUrl = Input.FacebookUrl,
            RegistrationStartTime = Input.RegistrationStartTime,
            RegistrationEndTime = Input.RegistrationEndTime,
            DartStart = Input.DartStart,
            MapUrl = Input.MapUrl,
            Description = Input.Description,
            IsTitleEvent = Input.IsTitleEvent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
