using Axiom.Assertions;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToComplexObjectTests
{
    [Fact]
    public void GivenComplexObjects_WhenMembersMatch_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Age = 30,
            Address = new Address { Postcode = "AB1A 1AA" },
            Tag = "dev",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Address = new Address { Postcode = "AB1A 1AA" },
            Tag = "dev",
        };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenComplexObjects_WhenNestedMemberDiffers_ThenReportsNestedPath()
    {
        var actual = new Person
        {
            Name = "Bob",
            Age = 30,
            Address = new Address { Postcode = "AB1A 1AA" },
            Tag = "dev",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Address = new Address { Postcode = "EC1A 1BB" },
            Tag = "dev",
        };

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeEquivalentTo(expected));

        Assert.Contains(
            "actual.Address.Postcode -> expected \"EC1A 1BB\", but found \"AB1A 1AA\" (String values differ.)",
            ex.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void GivenMultipleMemberDifferences_WhenComparing_ThenOrdersDifferencesDeterministically()
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

    [Fact]
    public void GivenIgnoredMemberName_WhenMemberDiffers_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Age = 31,
            Tag = "dev",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Tag = "dev",
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IgnoreMember(nameof(Person.Age))));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPublicFieldsExcluded_WhenFieldDiffers_ThenDoesNotThrow()
    {
        var actual = new Person
        {
            Name = "Bob",
            Age = 30,
            Tag = "alpha",
        };
        var expected = new Person
        {
            Name = "Bob",
            Age = 30,
            Tag = "beta",
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.IncludePublicFields = false));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenAnyCollectionOrderOption_WhenItemsAreReordered_ThenDoesNotThrow()
    {
        var actual = new[] { 3, 1, 2 };
        var expected = new[] { 1, 2, 3 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.CollectionOrder = EquivalencyCollectionOrder.Any));

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
}
