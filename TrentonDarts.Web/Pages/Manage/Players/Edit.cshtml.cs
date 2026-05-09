using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.Players;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public PlayerInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var player = await _db.Players.FindAsync(Id);
        if (player == null) return NotFound();

        Input = new PlayerInput
        {
            UserId = player.UserId,
            FirstName = player.FirstName,
            LastName = player.LastName,
            Nickname = player.Nickname,
            Email = player.Email,
            HomePhone = player.HomePhone,
            CellPhone = player.CellPhone,
            ShirtSize = player.ShirtSize,
            Address1 = player.Address1,
            Address2 = player.Address2,
            City = player.City,
            State = player.State,
            Zip = player.Zip,
            AcceptText = player.AcceptText,
            AcceptEmail = player.AcceptEmail,
            Notes = player.Notes
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var player = await _db.Players.FindAsync(Id);
        if (player == null) return NotFound();

        player.UserId = Input.UserId;
        player.FirstName = Input.FirstName;
        player.LastName = Input.LastName;
        player.Nickname = Input.Nickname;
        player.Email = Input.Email;
        player.HomePhone = Input.HomePhone;
        player.CellPhone = Input.CellPhone;
        player.ShirtSize = Input.ShirtSize;
        player.Address1 = Input.Address1;
        player.Address2 = Input.Address2;
        player.City = Input.City;
        player.State = Input.State;
        player.Zip = Input.Zip;
        player.AcceptText = Input.AcceptText;
        player.AcceptEmail = Input.AcceptEmail;
        player.Notes = Input.Notes;
        player.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
