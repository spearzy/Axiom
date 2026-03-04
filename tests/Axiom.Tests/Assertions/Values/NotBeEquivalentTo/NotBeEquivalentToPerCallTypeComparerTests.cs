using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.NotBeEquivalentTo;

public sealed class NotBeEquivalentToPerCallTypeComparerTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void GivenPerCallTypeComparer_WhenValuesEquivalentByConfiguredComparer_ThenNotBeEquivalentToThrows()
    {
        var actual = 3;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(5, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Contains("to not be equivalent to 5", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenPerCallTypeComparer_WhenValuesNotEquivalentByConfiguredComparer_ThenNotBeEquivalentToDoesNotThrow()
    {
        var actual = 3;

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo(8, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Null(ex);
    }

    [Fact]
    public void GivenPerCallTypeComparerAndConflictingGlobalProvider_WhenComparing_ThenPerCallComparerWins()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new AlwaysFalseIntComparerProvider());
        var actual = 3;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo(5, options => options.UseComparerForType<int>(new OddEvenMatchIntComparer())));

        Assert.Contains("to not be equivalent to 5", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenNullableTypeComparer_WhenNullableLeafValuesEquivalent_ThenNotBeEquivalentToThrows()
    {
        int? actual = 3;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            actual.Should().NotBeEquivalentTo((int?)5, options => options.UseComparerForType<int?>(new OddEvenMatchNullableIntComparer())));

        Assert.Contains("to not be equivalent to 5", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GivenNullableTypeComparer_WhenNullableLeafValuesNotEquivalent_ThenNotBeEquivalentToDoesNotThrow()
    {
        int? actual = 3;

        var ex = Record.Exception(() =>
            actual.Should().NotBeEquivalentTo((int?)8, options => options.UseComparerForType<int?>(new OddEvenMatchNullableIntComparer())));

        Assert.Null(ex);
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
}
