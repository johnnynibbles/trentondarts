using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TrentonDarts.Web.Domain;

public enum SeasonPart
{
    Pre,
    Regular,
    Post
}

public class SeasonPartConverter : ValueConverter<SeasonPart, string>
{
    public SeasonPartConverter() : base(
        v => v.ToString().ToLower(),
        v => Enum.Parse<SeasonPart>(v, ignoreCase: true)) { }
}
