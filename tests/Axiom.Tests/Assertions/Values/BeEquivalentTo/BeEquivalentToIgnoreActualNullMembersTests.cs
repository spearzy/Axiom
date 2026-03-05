using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToIgnoreActualNullMembersTests
{
    [Fact]
    public void GivenIgnoreActualNullMembers_WhenActualMemberIsNull_ThenSkipsThatMemberComparison()
    {
        var actual = new Person { Name = "Bob", Nickname = null };
        var expected = new Person { Name = "Bob", Nickname = "Bobby" };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnoreActualNullMembers()));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenIgnoreActualNullMembers_WhenExtraMemberOnActualHasNullValue_ThenDoesNotThrow()
    {
        PersonBase actual = new PersonWithNickname { Name = "Bob", Nickname = null };
        PersonBase expected = new PersonBase { Name = "Bob" };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options =>
            {
                options.RequireStrictRuntimeTypes = false;
                options.IgnoreActualNullMembers();
            }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenIgnoreActualNullMembers_WhenActualMemberIsNotNullAndDiffers_ThenStillThrows()
    {
        var actual = new Person { Name = "Bob", Nickname = "Bobby" };
        var expected = new Person { Name = "Bob", Nickname = "Bee" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnoreActualNullMembers()));

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
