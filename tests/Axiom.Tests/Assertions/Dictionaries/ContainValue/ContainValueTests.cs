using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.ContainValue;

public sealed class ContainValueTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void ContainValue_ReturnsContinuation_WhenValueExists()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainValue(2);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainValue_Throws_WhenValueDoesNotExist()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainValue(9));

        const string expected = "Expected values to contain value 9, but found values were [1, 2].";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainValue_ReturnsContinuation_WhenComparerMatchesValue()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var baseAssertions = values.Should();
        var continuation = baseAssertions.ContainValue("queued", StringComparer.OrdinalIgnoreCase);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void ContainValue_ThrowsArgumentNullException_WhenComparerIsNull()
    {
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
        };
        IEqualityComparer<string>? comparer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().ContainValue("created", comparer!));

        Assert.Equal("comparer", ex.ParamName);
    }

    [Fact]
    public void ContainValue_UsesConfiguredComparerProvider_WhenNoExplicitComparerIsSupplied()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new CaseInsensitiveStringComparerProvider());
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var ex = Record.Exception(() => values.Should().ContainValue("queued"));

        Assert.Null(ex);
    }

    [Fact]
    public void ContainValue_UsesExplicitComparerInsteadOfConfiguredComparerProvider()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new CaseInsensitiveStringComparerProvider());
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainValue("queued", StringComparer.Ordinal));

        const string expected = "Expected values to contain value \"queued\", but found values were [\"Created\", \"Queued\"].";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void ContainValue_FallsBackToDefaultComparer_WhenProviderDoesNotSupplyOne()
    {
        AxiomServices.Configure(c => c.ComparerProvider = new EmptyComparerProvider());
        Dictionary<string, string> values = new()
        {
            ["alpha"] = "Created",
            ["beta"] = "Queued",
        };

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().ContainValue("queued"));

        const string expected = "Expected values to contain value \"queued\", but found values were [\"Created\", \"Queued\"].";
        Assert.Equal(expected, ex.Message);
    }

    private sealed class CaseInsensitiveStringComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            if (typeof(T) == typeof(string))
            {
                comparer = (IEqualityComparer<T>)StringComparer.OrdinalIgnoreCase;
                return true;
            }

            comparer = null;
            return false;
        }
    }

    private sealed class EmptyComparerProvider : IComparerProvider
    {
        public bool TryGetEqualityComparer<T>(out IEqualityComparer<T>? comparer)
        {
            comparer = null;
            return false;
        }
    }
}
