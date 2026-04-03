using System.Collections;
using Axiom.Assertions;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToMemberComparerTests
{
    [Fact]
    public void GivenMemberComparer_WhenStringMemberDiffersByCase_ThenUsesConfiguredComparer()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForMember(nameof(Person.Name), StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenMemberAndTypeComparers_WhenBothConfiguredForSameMember_ThenMemberComparerWins()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForType<string>(new AlwaysFalseStringComparer());
                    options.UseComparerForMember(nameof(Person.Name), StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenIgnoredMemberWithConfiguredMemberComparer_WhenMemberDiffers_ThenIgnoreWins()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "xyz", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.UseComparerForMember(nameof(Person.Name), new AlwaysFalseObjectComparer());
                    options.IgnoreMember(nameof(Person.Name));
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenNullMemberPath_WhenConfiguringOptions_ThenThrowsArgumentNullException()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForMember(null!, StringComparer.OrdinalIgnoreCase)));

        Assert.Equal("memberPath", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrWhitespaceMemberPath_WhenConfiguringOptions_ThenThrowsArgumentException(string memberPath)
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForMember(memberPath, StringComparer.OrdinalIgnoreCase)));

        Assert.Equal("memberPath", ex.ParamName);
    }

    [Fact]
    public void GivenNullComparer_WhenConfiguringOptions_ThenThrowsArgumentNullException()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForMember(nameof(Person.Name), null!)));

        Assert.Equal("comparer", ex.ParamName);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class AlwaysFalseObjectComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return false;
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }

    private sealed class AlwaysFalseStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            return false;
        }

        public int GetHashCode(string obj)
        {
            return 0;
        }
    }
}
