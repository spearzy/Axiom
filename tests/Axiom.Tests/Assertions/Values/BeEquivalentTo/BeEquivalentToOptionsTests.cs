using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToOptionsTests
{
    [Fact]
    public void GivenReorderedCollection_WhenUsingDefaultOptions_ThenThrowsBecauseStrictOrderIsUsed()
    {
        var actual = new[] { 3, 1, 2 };
        var expected = new[] { 1, 2, 3 };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains("actual[0]", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenIgnoredPath_WhenNestedValueDiffers_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "AB1A 1AA" },
        };
        var expected = new Person
        {
            Name = "Bob",
            Address = new Address { Postcode = "EC1A 1BB" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnorePath("actual.Address")));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenMaxDifferences_WhenMoreDifferencesExist_ThenReportIsTruncated()
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
    public void GivenAssignableTypesAllowed_WhenRuntimeTypesDiffer_ThenDoesNotThrow()
    {
        Base actual = new Derived { Id = 10 };
        Base expected = new Base { Id = 10 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.RequireStrictRuntimeTypes = false));

        Assert.Null(ex);
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

    private class Base
    {
        public int Id { get; init; }
    }

    private sealed class Derived : Base
    {
    }
}
