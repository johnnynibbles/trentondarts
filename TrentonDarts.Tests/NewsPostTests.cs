using FluentAssertions;
using TrentonDarts.Web.Data.Entities;

namespace TrentonDarts.Tests;

public class NewsPostTests
{
    [Fact]
    public void Publish_SetsPublishedAt()
    {
        var post = new NewsPost { Title = "Test", Slug = "test" };

        post.Publish();

        post.PublishedAt.Should().NotBeNull();
        post.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Unpublish_ClearsPublishedAt()
    {
        var post = new NewsPost { Title = "Test", Slug = "test", PublishedAt = DateTime.UtcNow };

        post.Unpublish();

        post.PublishedAt.Should().BeNull();
    }

    [Fact]
    public void IsDraft_TrueWhenPublishedAtIsNull()
    {
        var post = new NewsPost { Title = "Test", Slug = "test" };

        post.IsDraft.Should().BeTrue();
    }

    [Fact]
    public void IsDraft_FalseWhenPublishedAtIsSet()
    {
        var post = new NewsPost { Title = "Test", Slug = "test", PublishedAt = DateTime.UtcNow };

        post.IsDraft.Should().BeFalse();
    }
}
