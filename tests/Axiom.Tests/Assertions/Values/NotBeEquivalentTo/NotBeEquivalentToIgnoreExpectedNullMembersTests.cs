using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToIgnoreExpectedNullMembersTests
{
    [Fact]
    public void GivenIgnoreExpectedNullMembers_WhenOnlyDifferenceIsExpectedNullMember_ThenNotBeEquivalentToThrows()
    {
        var actual = new Person { Name = "Bob", Nickname = "Bobby" };
        var expected = new Person { Name = "Bob", Nickname = null };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.IgnoreExpectedNullMembers()));

        Assert.Contains("to not be equivalent to", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenIgnoreExpectedNullMembers_WhenAnotherMemberDiffers_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = new Person { Name = "Alice", Nickname = "Bobby" };
        var expected = new Person { Name = "Bob", Nickname = null };

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(expected, options => options.IgnoreExpectedNullMembers()));

        Assert.Null(ex);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public string? Nickname { get; init; }
    }
}
