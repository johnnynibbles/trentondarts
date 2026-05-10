using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Manage.Users;

[Authorize(Roles = "Owner,Admin")]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public EditModel(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; } = string.Empty;

    public User? EditUser { get; private set; }

    [BindProperty]
    public UserEditInput Input { get; set; } = new();

    public List<Player> Players { get; private set; } = [];
    public List<BoardMember> BoardMembers { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        EditUser = await _db.Users.FindAsync(UserId);
        if (EditUser == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(EditUser);
        Input.Role = roles.FirstOrDefault() ?? string.Empty;

        var currentPlayer = await _db.Players.FirstOrDefaultAsync(p => p.UserId == UserId);
        Input.PlayerId = currentPlayer?.Id;

        var currentBoardMember = await _db.BoardMembers.FirstOrDefaultAsync(b => b.UserId == UserId);
        Input.BoardMemberId = currentBoardMember?.Id;

        await LoadDropdownsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        EditUser = await _db.Users.FindAsync(UserId);
        if (EditUser == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(EditUser);
        if (currentRoles.Count > 0)
            await _userManager.RemoveFromRolesAsync(EditUser, currentRoles);
        if (!string.IsNullOrEmpty(Input.Role))
            await _userManager.AddToRoleAsync(EditUser, Input.Role);

        var oldPlayer = await _db.Players.FirstOrDefaultAsync(p => p.UserId == UserId);
        if (oldPlayer?.Id != Input.PlayerId)
        {
            if (oldPlayer != null)
            {
                oldPlayer.UserId = null;
                oldPlayer.UpdatedAt = DateTime.UtcNow;
            }
            if (Input.PlayerId.HasValue)
            {
                var newPlayer = await _db.Players.FindAsync(Input.PlayerId.Value);
                if (newPlayer != null)
                {
                    newPlayer.UserId = UserId;
                    newPlayer.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        var oldBoardMember = await _db.BoardMembers.FirstOrDefaultAsync(b => b.UserId == UserId);
        if (oldBoardMember?.Id != Input.BoardMemberId)
        {
            if (oldBoardMember != null)
            {
                oldBoardMember.UserId = null;
                oldBoardMember.UpdatedAt = DateTime.UtcNow;
            }
            if (Input.BoardMemberId.HasValue)
            {
                var newBoardMember = await _db.BoardMembers.FindAsync(Input.BoardMemberId.Value);
                if (newBoardMember != null)
                {
                    newBoardMember.UserId = UserId;
                    newBoardMember.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("Index", new { leagueId = LeagueId });
    }

    private async Task LoadDropdownsAsync()
    {
        Players = await _db.Players
            .Where(p => p.LeagueId == LeagueId)
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .ToListAsync();

        BoardMembers = await _db.BoardMembers
            .Where(b => b.LeagueId == LeagueId)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }
}

public class UserEditInput
{
    public string Role { get; set; } = string.Empty;
    public int? PlayerId { get; set; }
    public int? BoardMemberId { get; set; }
}
