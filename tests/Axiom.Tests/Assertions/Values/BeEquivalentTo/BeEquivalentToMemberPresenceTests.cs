using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToMemberPresenceTests
{
    [Fact]
    public void GivenMissingMemberOnActual_WhenAllowingAssignableTypes_ThenThrows()
    {
        PersonBase actual = new PersonBase { Name = "Bob" };
        PersonBase expected = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("actual.Email", ex.Message, StringComparison.Ordinal);
        Assert.Contains("member is missing on actual", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMissingMemberOnActual_WhenFailOnMissingMembersDisabled_ThenDoesNotThrow()
    {
        PersonBase actual = new PersonBase { Name = "Bob" };
        PersonBase expected = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options =>
            {
                options.RequireStrictRuntimeTypes = false;
                options.FailOnMissingMembers = false;
            }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenExtraMemberOnActual_WhenAllowingAssignableTypes_ThenThrows()
    {
        PersonBase actual = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };
        PersonBase expected = new PersonBase { Name = "Bob" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("actual.Email", ex.Message, StringComparison.Ordinal);
        Assert.Contains("member is present on actual but missing on expected", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenExtraMemberOnActual_WhenFailOnExtraMembersDisabled_ThenDoesNotThrow()
    {
        PersonBase actual = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };
        PersonBase expected = new PersonBase { Name = "Bob" };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options =>
            {
                options.RequireStrictRuntimeTypes = false;
                options.FailOnExtraMembers = false;
            }));

        Assert.Null(ex);
    }

    private class PersonBase
    {
        public string? Name { get; init; }
    }

    private sealed class PersonWithEmail : PersonBase
    {
        public string? Email { get; init; }
    }
}
