using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToDiagnosticsTests
{
    [Fact]
    public void GivenMissingMemberOnActual_WhenEquivalencyFails_ThenMessageExplainsWhichSideIsMissing()
    {
        PersonBase actual = new PersonBase { Name = "Bob" };
        PersonBase expected = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("actual.Email", ex.Message, StringComparison.Ordinal);
        Assert.Contains("member is missing on actual", ex.Message, StringComparison.Ordinal);
        Assert.Contains("expected \"bob@example.com\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenExtraMemberOnActual_WhenEquivalencyFails_ThenMessageExplainsActualHasExtraMember()
    {
        PersonBase actual = new PersonWithEmail { Name = "Bob", Email = "bob@example.com" };
        PersonBase expected = new PersonBase { Name = "Bob" };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.RequireStrictRuntimeTypes = false));

        Assert.Contains("actual.Email", ex.Message, StringComparison.Ordinal);
        Assert.Contains("member is present on actual but missing on expected", ex.Message, StringComparison.Ordinal);
        Assert.Contains("actual \"bob@example.com\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenTypedMappedNestedStringMismatch_WhenEquivalencyFails_ThenMessageShowsBothPaths()
    {
        var actual = new ActualUser
        {
            GivenName = "Alice",
            Address = new ActualAddress { Postcode = "AB1 2CD" },
        };
        var expected = new ExpectedUser
        {
            FirstName = "Alice",
            Location = new ExpectedLocation { ZipCode = "ZZ9 9ZZ" },
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                }));

        Assert.Contains(
            "actual.Address.Postcode (compared with expected.Location.ZipCode)",
            ex.Message,
            StringComparison.Ordinal);
        Assert.Contains("string mismatch;", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenStringMismatchInsideEquivalency_WhenEquivalencyFails_ThenMessageIncludesStringDifferenceDetail()
    {
        var actual = new Person { Name = "Alice" };
        var expected = new Person { Name = "Alicia" };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual.Name", ex.Message, StringComparison.Ordinal);
        Assert.Contains("string mismatch;", ex.Message, StringComparison.Ordinal);
        Assert.Contains("first string difference;", ex.Message, StringComparison.Ordinal);
        Assert.Contains("expected snippet", ex.Message, StringComparison.Ordinal);
        Assert.Contains("actual snippet", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMaxDifferencesReached_WhenRenderingReport_ThenMessageStatesHowManyWereOmitted()
    {
        var actual = new Person
        {
            Name = "Alice",
            Age = 31,
            Address = new Address { Postcode = "AB1A 1AA" },
            Tag = "one",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Address = new Address { Postcode = "EC1A 1BB" },
            Tag = "two",
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.MaxDifferences = 2));

        Assert.Contains("but found 4 difference(s):", ex.Message, StringComparison.Ordinal);
        Assert.Contains("additional difference(s) omitted", ex.Message, StringComparison.Ordinal);
        Assert.Contains("MaxDifferences = 2", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMultipleDifferences_WhenRenderingReport_ThenReportedPathsStayDeterministic()
    {
        var actual = new Person
        {
            Name = "Alice",
            Age = 31,
            Tag = "one",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Tag = "two",
        };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));
        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);

        var ageIndex = message.IndexOf("actual.Age", StringComparison.Ordinal);
        var nameIndex = message.IndexOf("actual.Name", StringComparison.Ordinal);
        var tagIndex = message.IndexOf("actual.Tag", StringComparison.Ordinal);

        Assert.True(ageIndex >= 0, message);
        Assert.True(nameIndex >= 0, message);
        Assert.True(tagIndex >= 0, message);
        Assert.True(ageIndex < nameIndex, message);
        Assert.True(nameIndex < tagIndex, message);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
        public Address? Address { get; init; }
        public string? Tag;
    }

    private sealed class Address
    {
        public string? Postcode { get; init; }
    }

    private class PersonBase
    {
        public string? Name { get; init; }
    }

    private sealed class PersonWithEmail : PersonBase
    {
        public string? Email { get; init; }
    }

    private sealed class ActualUser
    {
        public string? GivenName { get; init; }
        public ActualAddress? Address { get; init; }
    }

    private sealed class ActualAddress
    {
        public string? Postcode { get; init; }
    }

    private sealed class ExpectedUser
    {
        public string? FirstName { get; init; }
        public ExpectedLocation? Location { get; init; }
    }

    private sealed class ExpectedLocation
    {
        public string? ZipCode { get; init; }
    }
}
