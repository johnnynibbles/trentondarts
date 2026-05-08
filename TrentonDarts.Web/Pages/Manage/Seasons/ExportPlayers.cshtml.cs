using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TrentonDarts.Web.Data;

namespace TrentonDarts.Web.Pages.Manage.Seasons;

public class ExportPlayersModel : PageModel
{
    private readonly AppDbContext _db;

    public ExportPlayersModel(AppDbContext db) => _db = db;

    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var teamPlayers = await _db.WinterSeasonTeamPlayers
            .Where(p => p.SeasonId == Id)
            .Include(p => p.Player)
            .Include(p => p.SeasonTeam).ThenInclude(st => st.Team)
            .OrderBy(p => p.SeasonTeam.Team.Name)
            .ThenBy(p => p.Player.LastName)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Team,First Name,Last Name,Nickname,Email,Home Phone,Cell Phone,Role");

        foreach (var tp in teamPlayers)
        {
            var p = tp.Player;
            sb.AppendLine($"{CsvEscape(tp.SeasonTeam.Team.Name)},{CsvEscape(p.FirstName)},{CsvEscape(p.LastName)},{CsvEscape(p.Nickname ?? "")},{CsvEscape(p.Email ?? "")},{CsvEscape(p.HomePhone ?? "")},{CsvEscape(p.CellPhone ?? "")},{CsvEscape(tp.Role)}");
        }

        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"season_{Id}_players.csv");
    }

    private static string CsvEscape(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
