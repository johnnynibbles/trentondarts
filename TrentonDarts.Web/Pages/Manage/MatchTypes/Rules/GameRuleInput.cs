using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrentonDarts.Web.Pages.Manage.MatchTypes.Rules;

public class GameRuleInput
{
    [Required]
    public string GameType { get; set; } = string.Empty;
    public bool DoubleIn { get; set; }
    public bool DoubleOut { get; set; }
    public int OrderId { get; set; }
    public int BestOfNumberOfLegs { get; set; }
    public int NumberOfLegs { get; set; } = 1;
    public string? WhoStarts { get; set; }
    public int NumberOfPlayers { get; set; } = 1;
    public int GamePointValue { get; set; } = 1;
    public int LegPointValue { get; set; }
    public bool ForfeitIfNoPlayers { get; set; }
    public string? GroupName { get; set; }
}

public static class GameRuleOptions
{
    public static List<SelectListItem> GameTypes =>
    [
        new("Cricket", "cricket"),
        new("Double Cricket", "doublecricket"),
        new("501", "501"),
        new("Double 501", "double501"),
        new("801", "801"),
    ];

    public static List<SelectListItem> WhoStarts =>
    [
        new("— Not Specified —", ""),
        new("Home", "H"),
        new("Away", "A"),
    ];
}
