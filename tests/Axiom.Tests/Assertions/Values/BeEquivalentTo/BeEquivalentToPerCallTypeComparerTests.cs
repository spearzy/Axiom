using Axiom.Assertions.EntryPoints;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToPerCallTypeComparerTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
        EquivalencyDefaults.Reset();
    }

    [Fact]
    public void GivenPerCallTypeComparer_WhenLeafValuesDifferByDefault_ThenUsesConfiguredComparer()
    {
        var actual = 3;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(5, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPerCallTypeComparer_WhenNestedLeafValuesDifferByDefault_ThenUsesConfiguredComparer()
    {
        var actual = new NumberWrapper { Number = 3 };
        var expected = new NumberWrapper { Number = 5 };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(expected, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPerCallTypeComparerAndConflictingGlobalProvider_WhenComparing_ThenPerCallComparerWins()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new AlwaysFalseIntComparerProvider());
        var actual = 3;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(5, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenNullTypeComparer_WhenConfiguringOptions_ThenThrowsArgumentNullException()
    {
        var actual = 3;

        var ex = Assert.Throws<ArgumentNullException>(() =>
            actual.Should().BeEquivalentTo(5, options => options.UseComparerForType<int>(null!)));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void GivenNullableTypeComparer_WhenNullableLeafValuesHaveValues_ThenConfiguredComparerIsUsed()
    {
        int? actual = 3;

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo((int?)5, options => options.UseComparerForType<int?>(new OddEvenMatchNullableIntComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPerCallStringComparer_WhenStringComparisonIsOrdinal_ThenStringComparisonTakesPrecedence()
    {
        object actual = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(
                "abc",
                options =>
                {
                    options.UseComparerForType<string>(new AlwaysTrueStringComparer());
                    options.StringComparison = StringComparison.Ordinal;
                }));

        Assert.Contains("String values differ.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenPerCallStringComparer_WhenStringComparisonIsOrdinalIgnoreCase_ThenStringComparisonStillTakesPrecedence()
    {
        object actual = "ABC";

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                "abc",
                options =>
                {
                    options.UseComparerForType<string>(new AlwaysFalseStringComparer());
                    options.StringComparison = StringComparison.OrdinalIgnoreCase;
                }));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenGlobalTypeComparer_WhenPerCallOverridesSameType_ThenPerCallOverrideWins()
    {
        EquivalencyDefaults.Configure(options => options.UseComparerForType<int>(new OddEvenMatchIntComparer()));
        var actual = 3;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().BeEquivalentTo(5, options => options.UseComparerForType<int>(new AlwaysFalseIntComparer())));

        Assert.Contains("Values differ.", ex.Message, StringComparison.Ordinal);
    }

    private sealed class NumberWrapper
    {
        public int Number { get; init; }
    }

    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            // Treat numbers as equivalent when they are both odd or both even.
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }

    private sealed class AlwaysFalseIntComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(int))
            {
                comparer = (IEqualityComparer<T>)(object)new AlwaysFalseIntComparer();
                return true;
            }

            comparer = null;
            return false;
        }
    }

    private sealed class AlwaysFalseIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return false;
        }

        public int GetHashCode(int obj)
        {
            return 0;
        }
    }

    private sealed class OddEvenMatchNullableIntComparer : IEqualityComparer<int?>
    {
        public bool Equals(int? x, int? y)
        {
            if (!x.HasValue || !y.HasValue)
            {
                return x == y;
            }

            return x.Value % 2 == y.Value % 2;
        }

        public int GetHashCode(int? obj)
        {
            return obj.GetValueOrDefault() % 2;
        }
    }

    private sealed class AlwaysTrueStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            return true;
        }

        public int GetHashCode(string obj)
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
