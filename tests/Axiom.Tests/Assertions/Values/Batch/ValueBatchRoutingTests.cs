using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Batch;

public sealed class ValueBatchRoutingTests
{
    private sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x % 2 == y % 2;
        }

        public int GetHashCode(int obj)
        {
            return obj % 2;
        }
    }

    private sealed class Marker(string id)
    {
        public string Id { get; } = id;

        public override string ToString()
        {
            return $"Marker({Id})";
        }
    }

    [Fact]
    public void Be_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));
    }

    [Fact]
    public void Be_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        var ex = Record.Exception(() =>
        {
            using var batch = new Axiom.Core.Batch();
            value.Should().Be(7);
        });

        Assert.NotNull(ex);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromValueAssertions()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("values");
            value.Should().Be(7);
            value.Should().NotBe(42);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'values' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to be 7, but found 42.", message);
        Assert.Contains("2) Expected value to not be 42, but found 42.", message);
    }

    [Fact]
    public void ComparerBasedValueAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var value = 3;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            value.Should().Be(4, new OddEvenMatchIntComparer());
            value.Should().NotBe(5, new OddEvenMatchIntComparer());
        });

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeOneOf_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeOneOf([1, 2, 3]));
    }

    [Fact]
    public void BeOneOf_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeOneOf([1, 2, 3]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeOneOf_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeOneOf([41, 42, 43]));
    }

    [Fact]
    public void NotBeOneOf_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeOneOf([41, 42, 43]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromOneOfAssertions()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("one-of");
            value.Should().BeOneOf([1, 2, 3]);
            value.Should().NotBeOneOf([41, 42, 43]);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'one-of' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to be one of [1, 2, 3], but found 42.", message);
        Assert.Contains("2) Expected value to not be one of [41, 42, 43], but found 42.", message);
    }

    [Fact]
    public void ComparerBasedOneOfAssertions_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        var value = 3;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
        {
            value.Should().BeOneOf([2, 4], new OddEvenMatchIntComparer());
            value.Should().NotBeOneOf([2, 4], new OddEvenMatchIntComparer());
        });

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Satisfy_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().Satisfy(static x => x < 40));
    }

    [Fact]
    public void Satisfy_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().Satisfy(static x => x < 40));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotSatisfy_OutsideBatch_ThrowsImmediately()
    {
        var value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().NotSatisfy(static x => x > 40));
    }

    [Fact]
    public void NotSatisfy_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotSatisfy(static x => x > 40));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromPredicateAssertions()
    {
        var value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("predicates");
            value.Should().Satisfy(static x => x < 40);
            value.Should().NotSatisfy(static x => x > 40);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'predicates' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to satisfy predicate `x => x < 40`, but found 42.", message);
        Assert.Contains("2) Expected value to not satisfy predicate `x => x > 40`, but found 42.", message);
    }

    [Fact]
    public void Batch_Dispose_UsesPredicateVariableNames_InPredicateFailures()
    {
        var value = 42;
        Func<int, bool> isNegative = x => x < 0;
        Func<int, bool> isPositive = x => x > 0;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("predicate-vars");
            value.Should().Satisfy(isNegative);
            value.Should().NotSatisfy(isPositive);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'predicate-vars' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected value to satisfy predicate `isNegative`, but found 42.", message);
        Assert.Contains("2) Expected value to not satisfy predicate `isPositive`, but found 42.", message);
    }

    [Fact]
    public void BeFalse_OutsideBatch_ThrowsImmediately()
    {
        const bool value = true;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeFalse());
    }

    [Fact]
    public void BeFalse_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const bool value = true;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeFalse());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeTrue_OutsideBatch_ThrowsImmediately()
    {
        const bool value = false;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeTrue());
    }

    [Fact]
    public void BeTrue_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const bool value = false;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeTrue());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromBooleanAssertions()
    {
        const bool isEnabled = true;
        const bool isReady = false;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("bools");
            isEnabled.Should().BeFalse();
            isReady.Should().BeTrue();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'bools' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected isEnabled to be False, but found True.", message);
        Assert.Contains("2) Expected isReady to be True, but found False.", message);
    }

    [Fact]
    public void BeNull_OutsideBatch_ThrowsImmediately()
    {
        int? value = 1;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeNull());
    }

    [Fact]
    public void BeNull_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int? value = 1;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeNull());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeNull_OutsideBatch_ThrowsImmediately()
    {
        int? value = null;

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNull());
    }

    [Fact]
    public void NotBeNull_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int? value = null;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeNull());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNullabilityAssertions()
    {
        int? hasValue = 1;
        int? noValue = null;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("nullability");
            hasValue.Should().BeNull();
            noValue.Should().NotBeNull();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'nullability' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected hasValue to be null, but found 1.", message);
        Assert.Contains("2) Expected noValue to not be null, but found <null>.", message);
    }

    [Fact]
    public void BeSameAs_OutsideBatch_ThrowsImmediately()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        Assert.Throws<InvalidOperationException>(() => first.Should().BeSameAs(second));
    }

    [Fact]
    public void BeSameAs_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => first.Should().BeSameAs(second));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromReferenceAssertions()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("references");
            first.Should().BeSameAs(second);
            second.Should().BeSameAs(first);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'references' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected first to be same reference as Marker(two), but found Marker(one).", message);
        Assert.Contains("2) Expected second to be same reference as Marker(one), but found Marker(two).", message);
    }

    [Fact]
    public void NotBeSameAs_OutsideBatch_ThrowsImmediately()
    {
        var marker = new Marker("one");

        Assert.Throws<InvalidOperationException>(() => marker.Should().NotBeSameAs(marker));
    }

    [Fact]
    public void NotBeSameAs_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        var marker = new Marker("one");

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => marker.Should().NotBeSameAs(marker));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromReferenceInequalityAssertions()
    {
        var first = new Marker("one");
        var second = new Marker("two");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("reference-inequality");
            first.Should().NotBeSameAs(first);
            second.Should().NotBeSameAs(second);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'reference-inequality' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected first to not be same reference as Marker(one), but found Marker(one).", message);
        Assert.Contains("2) Expected second to not be same reference as Marker(two), but found Marker(two).", message);
    }

    [Fact]
    public void BeOfType_OutsideBatch_ThrowsImmediately()
    {
        object value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeOfType<string>());
    }

    [Fact]
    public void BeOfType_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        object value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeOfType<string>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeOfType_InsideBatch_DoesNotThrowOnDispose_WhenTypesMatch()
    {
        object value = "hello";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeOfType<string>());
        var disposeEx = Record.Exception(() => batch.Dispose());

        Assert.Null(callEx);
        Assert.Null(disposeEx);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromTypeAssertions()
    {
        object first = 42;
        object second = 7d;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("types");
            first.Should().BeOfType<string>();
            second.Should().BeOfType<int>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'types' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected first to be of type System.String, but found 42.", message);
        Assert.Contains("2) Expected second to be of type System.Int32, but found 7.", message);
    }

    [Fact]
    public void BeAssignableTo_OutsideBatch_ThrowsImmediately()
    {
        object value = 42;

        Assert.Throws<InvalidOperationException>(() => value.Should().BeAssignableTo<string>());
    }

    [Fact]
    public void BeAssignableTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        object value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeAssignableTo<string>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeAssignableTo_InsideBatch_DoesNotThrowOnDispose_WhenAssignable()
    {
        object value = "hello";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeAssignableTo<object>());
        var disposeEx = Record.Exception(() => batch.Dispose());

        Assert.Null(callEx);
        Assert.Null(disposeEx);
    }

    [Fact]
    public void NotBeAssignableTo_OutsideBatch_ThrowsImmediately()
    {
        object value = "hello";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeAssignableTo<string>());
    }

    [Fact]
    public void NotBeAssignableTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        object value = "hello";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeAssignableTo<string>());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeAssignableTo_InsideBatch_DoesNotThrowOnDispose_WhenNotAssignable()
    {
        object value = 42;

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeAssignableTo<string>());
        var disposeEx = Record.Exception(() => batch.Dispose());

        Assert.Null(callEx);
        Assert.Null(disposeEx);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromAssignableTypeAssertions()
    {
        object first = 42;
        object second = "hello";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("assignable-types");
            first.Should().BeAssignableTo<string>();
            second.Should().NotBeAssignableTo<object>();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'assignable-types' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected first to be assignable to System.String, but found 42.", message);
        Assert.Contains("2) Expected second to not be assignable to System.Object, but found \"hello\".", message);
    }
}
