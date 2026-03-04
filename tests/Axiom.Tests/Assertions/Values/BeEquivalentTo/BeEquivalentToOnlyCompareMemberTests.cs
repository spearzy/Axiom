using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToOnlyCompareMemberTests
{
    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
        public Address? Address { get; init; }
    }

    private sealed class Address
    {
        public string? Postcode { get; init; }
        public string? Country { get; init; }
    }

    [Fact]
    public void GivenOnlyCompareMember_WhenUnselectedMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Alice", Age = 99 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name))));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenOnlyCompareMember_WhenSelectedMemberDiffers_ThenThrows()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Bob", Age = 30 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.OnlyCompareMember(nameof(Person.Name))));

        Assert.Contains("actual.Name", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenOnlyCompareMember_WhenNestedMemberSelected_ThenComparesOnlyThatNestedMember()
    {
        var actual = new Person
        {
            Name = "Alice",
            Address = new Address { Postcode = "AB1 2CD", Country = "UK" },
        };
        var expected = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "AB1 2CD", Country = "US" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options =>
                options.OnlyCompareMember($"{nameof(Person.Address)}.{nameof(Address.Postcode)}")));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenOnlyCompareMembers_WhenAnySelectedMemberDiffers_ThenThrows()
    {
        var actual = new Person
        {
            Name = "Alice",
            Address = new Address { Postcode = "AB1 2CD", Country = "UK" },
        };
        var expected = new Person
        {
            Name = "Alice",
            Address = new Address { Postcode = "ZZ1 9ZZ", Country = "UK" },
        };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(expected, options =>
                options.OnlyCompareMembers(
                    nameof(Person.Name),
                    $"{nameof(Person.Address)}.{nameof(Address.Postcode)}")));

        Assert.Contains("actual.Address.Postcode", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenBlankMemberPath_WhenOnlyCompareMemberCalled_ThenThrowsArgumentException()
    {
        var actual = 42;

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(42, options => options.OnlyCompareMember(" ")));

        Assert.Equal("memberPath", ex.ParamName);
    }

    [Fact]
    public void GivenNullMemberPathsArray_WhenOnlyCompareMembersCalled_ThenThrowsArgumentNullException()
    {
        var actual = 42;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(42, options => options.OnlyCompareMembers((string[]?)null!)));

        Assert.Equal("memberPaths", ex.ParamName);
    }
}
