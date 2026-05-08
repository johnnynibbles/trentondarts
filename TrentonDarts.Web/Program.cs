using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Authorization;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;
using TrentonDarts.Web.Domain;
using TrentonDarts.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// EF Core + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity mapped to existing users table
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IPasswordHasher<User>, LaravelPasswordHasher>();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Auth/Login";
    opts.AccessDeniedPath = "/Auth/Login";
});

builder.Services.AddAntiforgery(opts => opts.HeaderName = "X-CSRF-Token");

builder.Services.AddScoped<NavService>();
builder.Services.AddScoped<SeasonService>();
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<PlayerHistoryService>();
builder.Services.AddScoped<TrentonDarts.Web.Domain.MatchRepository>();
builder.Services.AddScoped<UpdateMatchStatsService>();

builder.Services.AddRazorPages(opts =>
    opts.Conventions.AuthorizeFolder("/Manage"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Temporary DB connectivity test endpoint
app.MapGet("/dbtest", async (AppDbContext db) =>
    Results.Json(new { players = await db.Players.CountAsync() }));

// Scorecard save endpoint — consumed by scoresheet-edit.js
app.MapPost("/season/{seasonId:int}/match/{matchId:int}",
    async (int seasonId, int matchId,
           [FromBody] ScorecardSaveDto data,
           MatchRepository matchRepo,
           UpdateMatchStatsService stats,
           HttpContext ctx,
           IAntiforgery antiforgery) =>
    {
        await antiforgery.ValidateRequestAsync(ctx);
        await matchRepo.SaveMatchResultsDataAsync(matchId, data);
        await stats.UpdateAsync(matchId);
        return Results.Ok(new { redirectUrl = $"/season/{seasonId}/schedule" });
    }).RequireAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
