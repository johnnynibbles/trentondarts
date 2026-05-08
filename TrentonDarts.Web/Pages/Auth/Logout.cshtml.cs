using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Auth;

public class LogoutModel : PageModel
{
    private readonly SignInManager<User> _signIn;

    public LogoutModel(SignInManager<User> signIn)
    {
        _signIn = signIn;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _signIn.SignOutAsync();
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _signIn.SignOutAsync();
        return RedirectToPage("/Index");
    }
}
