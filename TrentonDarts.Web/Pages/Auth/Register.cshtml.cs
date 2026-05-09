using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _users;
    private readonly SignInManager<User> _signIn;

    public RegisterModel(UserManager<User> users, SignInManager<User> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    [BindProperty, Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty, Required, MinLength(6)]
    public string Password { get; set; } = "";

    [BindProperty, Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Manage/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new User { UserName = Email, Email = Email, Name = Name };
        var result = await _users.CreateAsync(user, Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _users.AddToRoleAsync(user, Roles.User);
        await _signIn.SignInAsync(user, isPersistent: true);
        return RedirectToPage("/Manage/Index");
    }
}
