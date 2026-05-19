using FluentAssertions;
using TrentonDarts.Web.Utilities;

namespace TrentonDarts.Tests;

public class SlugTests
{
    [Fact]
    public void LowercasesAndReplacesSpacesWithHyphens()
    {
        Slug.From("Hello World").Should().Be("hello-world");
    }

    [Fact]
    public void StripsNonAlphanumericCharacters()
    {
        Slug.From("Hello, World!").Should().Be("hello-world");
    }

    [Fact]
    public void CollapsesConsecutiveHyphens()
    {
        Slug.From("hello--world").Should().Be("hello-world");
    }

    [Fact]
    public void TrimsLeadingAndTrailingHyphens()
    {
        Slug.From("-hello-world-").Should().Be("hello-world");
    }

    [Fact]
    public void AlreadyCleanInputIsUnchanged()
    {
        Slug.From("hello-world").Should().Be("hello-world");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyOrWhitespaceReturnsEmptyString(string input)
    {
        Slug.From(input).Should().Be("");
    }
}
