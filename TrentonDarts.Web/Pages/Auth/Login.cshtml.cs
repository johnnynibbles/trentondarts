using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly SignInManager<User> _signIn;

    public LoginModel(SignInManager<User> signIn)
    {
        _signIn = signIn;
    }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty, Required]
    public string Password { get; set; } = "";

    public string ErrorMessage { get; private set; } = "";

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect("/");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signIn.PasswordSignInAsync(Email, Password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
            return LocalRedirect(returnUrl ?? "/Manage/Index");

        ErrorMessage = "Invalid email or password.";
        return Page();
    }
}
