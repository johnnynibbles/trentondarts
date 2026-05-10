using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Services;

namespace TrentonDarts.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly UserManager<User> _users;
    private readonly TurnstileService _turnstile;
    private readonly SmtpEmailSender _email;
    private readonly TurnstileOptions _turnstileOpts;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<User> users,
        TurnstileService turnstile,
        SmtpEmailSender email,
        IOptions<TurnstileOptions> turnstileOpts,
        ILogger<RegisterModel> logger)
    {
        _users = users;
        _turnstile = turnstile;
        _email = email;
        _turnstileOpts = turnstileOpts.Value;
        _logger = logger;
    }

    public string TurnstileSiteKey => _turnstileOpts.SiteKey;

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

        var turnstileToken = Request.Form["cf-turnstile-response"];
        if (!await _turnstile.ValidateAsync(turnstileToken))
        {
            ModelState.AddModelError(string.Empty, "Please complete the human verification.");
            return Page();
        }

        var isFirstUser = !_users.Users.Any();

        var user = new User { UserName = Email, Email = Email, Name = Name };
        var result = await _users.CreateAsync(user, Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _users.AddToRoleAsync(user, isFirstUser ? Roles.Admin : Roles.User);

        var token = await _users.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var confirmLink = Url.Page("/Auth/ConfirmEmail",
            pageHandler: null,
            values: new { userId = user.Id, token = encodedToken },
            protocol: Request.Scheme)!;

        try
        {
            await _email.SendEmailAsync(
                Email,
                "Confirm your Trenton Darts account",
                $"<p>Thanks for registering! Please <a href='{confirmLink}'>click here to confirm your email</a>.</p>" +
                $"<p>If you didn't register for a Trenton Darts account, you can ignore this email.</p>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", Email);
        }

        return RedirectToPage("/Auth/RegisterConfirmation");
    }
}
