using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToIgnoreExpectedNullMembersTests
{
    [Fact]
    public void GivenIgnoreExpectedNullMembers_WhenExpectedMemberIsNull_ThenSkipsThatMemberComparison()
    {
        var actual = new Person { Name = "Bob", Nickname = "Bobby" };
        var expected = new Person { Name = "Bob", Nickname = null };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnoreExpectedNullMembers()));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenIgnoreExpectedNullMembers_WhenMissingMemberOnActualHasNullExpectedValue_ThenDoesNotThrow()
    {
        PersonBase actual = new PersonBase { Name = "Bob" };
        PersonBase expected = new PersonWithNickname { Name = "Bob", Nickname = null };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options =>
            {
                options.RequireStrictRuntimeTypes = false;
                options.IgnoreExpectedNullMembers();
            }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenIgnoreExpectedNullMembers_WhenExpectedMemberIsNotNullAndDiffers_ThenStillThrows()
    {
        var actual = new Person { Name = "Bob", Nickname = "Bobby" };
        var expected = new Person { Name = "Bob", Nickname = "Bee" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnoreExpectedNullMembers()));

        Assert.Contains("actual.Nickname", ex.Message, StringComparison.Ordinal);
    }

    private class PersonBase
    {
        public string? Name { get; init; }
    }

    private sealed class PersonWithNickname : PersonBase
    {
        public string? Nickname { get; init; }
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public string? Nickname { get; init; }
    }
}
