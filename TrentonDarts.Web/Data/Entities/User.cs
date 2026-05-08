using Microsoft.AspNetCore.Identity;

namespace TrentonDarts.Web.Data.Entities;

public class User : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
