using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Auth;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<User> _users;

    public ConfirmEmailModel(UserManager<User> users) => _users = users;

    public bool Succeeded { get; private set; }

    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return RedirectToPage("/Index");

        var user = await _users.FindByIdAsync(userId);
        if (user == null)
        {
            Succeeded = false;
            return Page();
        }

        var decodedToken = Uri.UnescapeDataString(token);
        var result = await _users.ConfirmEmailAsync(user, decodedToken);
        Succeeded = result.Succeeded;
        return Page();
    }
}
