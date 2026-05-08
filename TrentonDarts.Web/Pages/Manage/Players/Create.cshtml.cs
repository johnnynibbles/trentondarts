using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Players;

public class PlayerInput
{
    public string? UserId { get; set; }
    [Required] public string FirstName { get; set; } = "";
    [Required] public string LastName { get; set; } = "";
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? HomePhone { get; set; }
    public string? CellPhone { get; set; }
    public string? ShirtSize { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public bool AcceptText { get; set; }
    public bool AcceptEmail { get; set; }
    public string? Notes { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public PlayerInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var player = new Player
        {
            LeagueId = LeagueId,
            UserId = Input.UserId,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Nickname = Input.Nickname,
            Email = Input.Email,
            HomePhone = Input.HomePhone,
            CellPhone = Input.CellPhone,
            ShirtSize = Input.ShirtSize,
            Address1 = Input.Address1,
            Address2 = Input.Address2,
            City = Input.City,
            State = Input.State,
            Zip = Input.Zip,
            AcceptText = Input.AcceptText,
            AcceptEmail = Input.AcceptEmail,
            Notes = Input.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Players.Add(player);
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
