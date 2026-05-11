using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrentonDarts.Web.Authorization;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

// Npgsql 6+ defaults DateTime to timestamptz; our entities use unspecified-kind DateTime
// (matching the original Laravel/MySQL schema), so opt into the legacy mapping.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(ResolveConnectionString(builder.Configuration)));

// DO App Platform injects DATABASE_URL as a postgres:// URI; Npgsql expects key-value format.
static string ResolveConnectionString(IConfiguration config)
{
    var raw = config.GetConnectionString("DefaultConnection") ?? string.Empty;
    if (!raw.StartsWith("postgresql://") && !raw.StartsWith("postgres://"))
        return raw;

    var withoutQuery = raw.Split('?')[0];
    var uri = new Uri(withoutQuery);
    var parts = uri.UserInfo.Split(':', 2);
    var user = Uri.UnescapeDataString(parts[0]);
    var pass = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
    var db = uri.AbsolutePath.TrimStart('/');
    return $"Host={uri.Host};Port={uri.Port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
}

// ASP.NET Core Identity mapped to existing users table
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequiredLength = 6;
    opts.SignIn.RequireConfirmedAccount = true;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    opts.Lockout.MaxFailedAccessAttempts = 5;
    opts.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IPasswordHasher<User>, LaravelPasswordHasher>();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Auth/Login";
    opts.AccessDeniedPath = "/Auth/Login";
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddAntiforgery(opts => opts.HeaderName = "X-CSRF-Token");

builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("Storage"));
builder.Services.AddSingleton<IFileStorageService, S3FileStorageService>();
builder.Services.AddScoped<BrowsableFileService>();

builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<TurnstileOptions>(builder.Configuration.GetSection("Turnstile"));
builder.Services.AddHttpClient<TurnstileService>();
builder.Services.AddScoped<SmtpEmailSender>();

builder.Services.AddScoped<NavService>();
builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<PlayerHistoryService>();
builder.Services.AddScoped<TrentonDarts.Web.Domain.MatchRepository>();
builder.Services.AddScoped<UpdateMatchStatsService>();

builder.Services.AddRazorPages(opts =>
    opts.Conventions.AuthorizeFolder("/Manage"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Scorecard save endpoint — consumed by scoresheet-edit.js
app.MapPost("/season/{seasonId:int}/match/{matchId:int}",
    async (int seasonId, int matchId,
           [FromBody] ScorecardSaveDto data,
           MatchRepository matchRepo,
           UpdateMatchStatsService stats,
           AppDbContext db,
           HttpContext ctx,
           IAntiforgery antiforgery) =>
    {
        await antiforgery.ValidateRequestAsync(ctx);

        var user = ctx.User;
        if (!user.IsInRole(Roles.Admin) && !user.IsInRole(Roles.Owner))
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isBoardMember = userId != null && await db.BoardMembers.AnyAsync(b => b.UserId == userId);
            if (!isBoardMember)
                return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        await matchRepo.SaveMatchResultsDataAsync(matchId, data);
        await stats.UpdateAsync(matchId);
        return Results.Ok(new { redirectUrl = $"/season/{seasonId}/schedule" });
    }).RequireAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
