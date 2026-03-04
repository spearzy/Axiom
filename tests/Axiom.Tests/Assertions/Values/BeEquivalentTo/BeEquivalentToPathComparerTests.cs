using System.Collections;
using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToPathComparerTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void GivenAbsolutePathComparer_WhenStringMemberDiffersByCase_ThenUsesConfiguredComparer()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenRelativePathComparer_WhenStringMemberDiffersByCase_ThenUsesConfiguredComparer()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.StringComparison = StringComparison.Ordinal;
                    options.UseComparerForPath(nameof(Person.Name), StringComparer.OrdinalIgnoreCase);
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPerPathAndPerTypeComparers_WhenBothConfiguredForSameMember_ThenPerPathComparerWins()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.UseComparerForType<string>(StringComparer.OrdinalIgnoreCase);
                    options.UseComparerForPath("actual.Name", new AlwaysFalseObjectComparer());
                }));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenGlobalPathComparer_WhenPerCallOverridesSamePath_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options =>
            options.UseComparerForPath("actual.Name", StringComparer.OrdinalIgnoreCase));

        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options => options.UseComparerForPath("actual.Name", new AlwaysFalseObjectComparer())));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenNullPathComparer_WhenConfiguringOptions_ThenThrowsArgumentNullException()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "abc", Age = 30 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForPath("actual.Name", null!)));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void GivenNullPath_WhenConfiguringOptions_ThenThrowsArgumentNullException()
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForPath(null!, StringComparer.OrdinalIgnoreCase)));

        Assert.Equal("path", ex.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenEmptyOrWhitespacePath_WhenConfiguringOptions_ThenThrowsArgumentException(string path)
    {
        var actual = new Person { Name = "ABC", Age = 30 };
        var expected = new Person { Name = "ABC", Age = 30 };

        var ex = Assert.Throws<ArgumentException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForPath(path, StringComparer.OrdinalIgnoreCase)));

        Assert.Equal("path", ex.ParamName);
    }

    [Fact]
    public void GivenToleranceAndPathComparerOnSameLeaf_WhenValueIsWithinTolerance_ThenToleranceWins()
    {
        var actual = new ScoreHolder { Score = 10.0 };
        var expected = new ScoreHolder { Score = 10.2 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.DoubleTolerance = 0.5;
                    options.UseComparerForPath("actual.Score", new AlwaysFalseObjectComparer());
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenToleranceAndPathComparerOnSameLeaf_WhenValueIsOutsideTolerance_ThenToleranceStillWins()
    {
        var actual = new ScoreHolder { Score = 10.0 };
        var expected = new ScoreHolder { Score = 11.0 };

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.DoubleTolerance = 0.5;
                    options.UseComparerForPath("actual.Score", new AlwaysTrueObjectComparer());
                }));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
    }

    private sealed class ScoreHolder
    {
        public double Score { get; init; }
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

    private sealed class AlwaysTrueObjectComparer : IEqualityComparer
    {
        public new bool Equals(object? x, object? y)
        {
            return true;
        }

        public int GetHashCode(object obj)
        {
            return 0;
        }
    }
}
