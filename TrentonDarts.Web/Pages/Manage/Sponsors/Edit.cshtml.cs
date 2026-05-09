using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.Sponsors;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public SponsorInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var s = await _db.Sponsors.FindAsync(Id);
        if (s == null) return NotFound();

        Input = new SponsorInput
        {
            Name = s.Name, Type = s.Type, ContactName = s.ContactName,
            Address1 = s.Address1, Address2 = s.Address2, City = s.City,
            State = s.State, Zip = s.Zip, Phone = s.Phone, Url = s.Url,
            FacebookUrl = s.FacebookUrl, Email = s.Email, MapUrl = s.MapUrl,
            Description = s.Description, Comments = s.Comments
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var s = await _db.Sponsors.FindAsync(Id);
        if (s == null) return NotFound();

        s.Name = Input.Name;
        s.Type = Input.Type;
        s.ContactName = Input.ContactName;
        s.Address1 = Input.Address1;
        s.Address2 = Input.Address2;
        s.City = Input.City;
        s.State = Input.State;
        s.Zip = Input.Zip;
        s.Phone = Input.Phone;
        s.Url = Input.Url;
        s.FacebookUrl = Input.FacebookUrl;
        s.Email = Input.Email;
        s.MapUrl = Input.MapUrl;
        s.Description = Input.Description;
        s.Comments = Input.Comments;
        s.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
