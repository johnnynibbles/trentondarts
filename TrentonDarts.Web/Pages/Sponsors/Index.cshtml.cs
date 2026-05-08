using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrentonDarts.Web.Data;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Web.Pages.Sponsors;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public Dictionary<string, string> SponsorTypes => Sponsor.SponsorTypes;
    public string SelectedType { get; private set; } = "L";
    public List<Sponsor> Sponsors { get; private set; } = new();

    public async Task OnGetAsync([FromQuery] string type = "L")
    {
        SelectedType = type;
        Sponsors = await _db.Sponsors
            .Where(s => s.Type == type)
            .Include(s => s.Teams)
            .ToListAsync();
    }
}
