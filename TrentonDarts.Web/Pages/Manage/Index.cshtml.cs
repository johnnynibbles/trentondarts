using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrentonDarts.Web.Pages.Manage;

public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int LeagueId { get; set; }

    public void OnGet() { }
}
