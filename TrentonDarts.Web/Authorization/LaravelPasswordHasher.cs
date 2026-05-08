using Microsoft.AspNetCore.Identity;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Authorization;

/// <summary>
/// Laravel uses the $2y$ BCrypt prefix; .NET BCrypt libraries use $2b$.
/// The two are functionally identical — we just normalize the prefix before verifying.
/// </summary>
public class LaravelPasswordHasher : IPasswordHasher<User>
{
    private readonly PasswordHasher<User> _inner = new();

    public string HashPassword(User user, string password)
        => _inner.HashPassword(user, password);

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        // Normalize Laravel $2y$ to standard $2b$ before verification
        var normalized = hashedPassword.StartsWith("$2y$")
            ? "$2b$" + hashedPassword[4..]
            : hashedPassword;

        return _inner.VerifyHashedPassword(user, normalized, providedPassword);
    }
}
