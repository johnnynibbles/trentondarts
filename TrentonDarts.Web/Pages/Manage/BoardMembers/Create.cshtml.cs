using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.BoardMembers;

public class BoardMemberInput
{
    public string? UserId { get; set; }
    [Required] public string Name { get; set; } = "";
    public string? Position { get; set; }
    public string? StartingSeason { get; set; }
    public string? EndingSeason { get; set; }
    public int? StartSeasonId { get; set; }
    public int? EndSeasonId { get; set; }
}

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty]
    public BoardMemberInput Input { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _db.BoardMembers.Add(new BoardMember
        {
            LeagueId = LeagueId,
            UserId = Input.UserId,
            Name = Input.Name,
            Position = Input.Position,
            StartingSeason = Input.StartingSeason,
            EndingSeason = Input.EndingSeason,
            StartSeasonId = Input.StartSeasonId,
            EndSeasonId = Input.EndSeasonId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
