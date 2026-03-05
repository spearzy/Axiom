using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToIgnoreActualNullMembersTests
{
    [Fact]
    public void GivenIgnoreActualNullMembers_WhenOnlyDifferenceIsActualNullMember_ThenNotBeEquivalentToThrows()
    {
        var actual = new Person { Name = "Bob", Nickname = null };
        var expected = new Person { Name = "Bob", Nickname = "Bobby" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.IgnoreActualNullMembers()));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenIgnoreActualNullMembers_WhenAnotherMemberDiffers_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "Alice", Nickname = null };
        var expected = new Person { Name = "Bob", Nickname = "Bobby" };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.IgnoreActualNullMembers()));

        Assert.Null(ex);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public string? Nickname { get; init; }
    }
}
