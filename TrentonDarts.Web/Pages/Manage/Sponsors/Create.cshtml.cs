using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Sponsors;

public class SponsorInput
{
    [Required] public string Name { get; set; } = "";
    [Required] public string Type { get; set; } = "L";
    public string? ContactName { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Phone { get; set; }
    public string? Url { get; set; }
    public string? FacebookUrl { get; set; }
    public string? Email { get; set; }
    public string? MapUrl { get; set; }
    public string? Description { get; set; }
    public string? Comments { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public SponsorInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.Sponsors.Add(new Sponsor
        {
            LeagueId = LeagueId,
            Name = Input.Name,
            Type = Input.Type,
            ContactName = Input.ContactName,
            Address1 = Input.Address1,
            Address2 = Input.Address2,
            City = Input.City,
            State = Input.State,
            Zip = Input.Zip,
            Phone = Input.Phone,
            Url = Input.Url,
            FacebookUrl = Input.FacebookUrl,
            Email = Input.Email,
            MapUrl = Input.MapUrl,
            Description = Input.Description,
            Comments = Input.Comments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }
}
