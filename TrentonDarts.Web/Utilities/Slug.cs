using System.Text.RegularExpressions;

namespace TrentonDarts.Web.Utilities;

public static class Slug
{
    public static string From(string title)
    {
        var lower = title.ToLowerInvariant();
        var hyphenated = lower.Replace(' ', '-');
        var stripped = new string(hyphenated.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());
        return Regex.Replace(stripped, "-{2,}", "-").Trim('-');
    }
}
