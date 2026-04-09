using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.Batch;

public sealed class CollectionBatchRoutingTests
{
    private sealed record WorkflowStep(int Position, string Name);
    private sealed record User(int Id, string Email);

    [Fact]
    public void Contain_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().Contain(9));
    }

    [Fact]
    public void Contain_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().Contain(9));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainAll_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().ContainAll(1, 9));
    }

    [Fact]
    public void ContainAll_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainAll(1, 9));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainAny_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().ContainAny(9, 10));
    }

    [Fact]
    public void ContainAny_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainAny(9, 10));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainExactly_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactly([1, 2]));
    }

    [Fact]
    public void ContainExactly_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainExactly([1, 2]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainExactlyInAnyOrder_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 2];

        Assert.Throws<InvalidOperationException>(() => values.Should().ContainExactlyInAnyOrder([1, 2, 3]));
    }

    [Fact]
    public void ContainExactlyInAnyOrder_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainExactlyInAnyOrder([1, 2, 3]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainExactlyInAnyOrder_WithComparer_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        string[] values = ["Alpha", "beta", "delta"];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().ContainExactlyInAnyOrder(
                ["alpha", "beta", "charlie"],
                StringComparer.OrdinalIgnoreCase));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeSubsetOf_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 4];

        Assert.Throws<InvalidOperationException>(() => values.Should().BeSubsetOf([1, 2, 3]));
    }

    [Fact]
    public void BeSubsetOf_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 4];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().BeSubsetOf([1, 2, 3]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeSupersetOf_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2];

        Assert.Throws<InvalidOperationException>(() => values.Should().BeSupersetOf([1, 2, 4]));
    }

    [Fact]
    public void BeSupersetOf_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().BeSupersetOf([1, 2, 4]));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeEmpty_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1];

        Assert.Throws<InvalidOperationException>(() => values.Should().BeEmpty());
    }

    [Fact]
    public void ContainSingle_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().ContainSingle());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void ContainSingle_WithPredicate_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() =>
            values.Should().ContainSingle((int item) => item == 9));
    }

    [Fact]
    public void ContainSingle_WithPredicate_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().ContainSingle((int item) => item == 9));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void OnlyContain_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().OnlyContain((int x) => x % 2 == 0));
    }

    [Fact]
    public void NotContain_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().NotContain((int x) => x == 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotContainItem_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().NotContain(2));
    }

    [Fact]
    public void NotContainItem_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().NotContain(2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotContainAny_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().NotContainAny(9, 2));
    }

    [Fact]
    public void NotContainAny_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().NotContainAny(9, 2));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveUniqueItems_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, 2, 3];

        Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItems());
    }

    [Fact]
    public void HaveUniqueItems_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, 2, 3];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().HaveUniqueItems());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveUniqueItemsBy_OutsideBatch_ThrowsImmediately()
    {
        User[] values =
        [
            new(1, "a@example.com"),
            new(2, "b@example.com"),
            new(1, "c@example.com")
        ];

        Assert.Throws<InvalidOperationException>(() => values.Should().HaveUniqueItemsBy((User user) => user.Id));
    }

    [Fact]
    public void HaveUniqueItemsBy_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        User[] values =
        [
            new(1, "a@example.com"),
            new(2, "b@example.com"),
            new(1, "c@example.com")
        ];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().HaveUniqueItemsBy((User user) => user.Id));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void AllSatisfy_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2, -1];

        Assert.Throws<InvalidOperationException>(() =>
            values.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0)));
    }

    [Fact]
    public void AllSatisfy_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2, -1];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void SatisfyRespectively_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 2];

        Assert.Throws<InvalidOperationException>(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(5)));
    }

    [Fact]
    public void SatisfyRespectively_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().SatisfyRespectively(
                (int item) => item.Should().BeGreaterThan(0),
                (int item) => item.Should().BeGreaterThan(5)));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInAscendingOrder_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [1, 3, 2];

        Assert.Throws<InvalidOperationException>(() => values.Should().BeInAscendingOrder());
    }

    [Fact]
    public void BeInAscendingOrder_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [1, 3, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().BeInAscendingOrder());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInAscendingOrder_WithComparer_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        string[] values = ["alpha", "charlie", "Beta"];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInDescendingOrder_OutsideBatch_ThrowsImmediately()
    {
        int[] values = [3, 1, 2];

        Assert.Throws<InvalidOperationException>(() => values.Should().BeInDescendingOrder());
    }

    [Fact]
    public void BeInDescendingOrder_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        int[] values = [3, 1, 2];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().BeInDescendingOrder());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInDescendingOrder_WithComparer_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        string[] values = ["charlie", "alpha", "Beta"];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() =>
            values.Should().BeInDescendingOrder(StringComparer.OrdinalIgnoreCase));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeInAscendingOrder_ByKey_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        User[] values =
        [
            new(1, "a@example.com"),
            new(3, "b@example.com"),
            new(2, "c@example.com")
        ];

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => values.Should().BeInAscendingOrder((User user) => user.Id));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromCollectionAssertions()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collections");
            values.Should().Contain(9);
            values.Should().HaveCount(2);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collections' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to contain 9, but found System.Int32[].", message);
        Assert.Contains("2) Expected values to have count 2, but found 3.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNewCollectionAssertions()
    {
        int[] nonEmpty = [1];
        int[] empty = [];
        int[] multiple = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-shape");
            nonEmpty.Should().BeEmpty();
            empty.Should().NotBeEmpty();
            multiple.Should().ContainSingle();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-shape' failed with 3 assertion failure(s):", message);
        Assert.Contains("1) Expected nonEmpty to be empty, but found 1.", message);
        Assert.Contains("2) Expected empty to not be empty, but found 0.", message);
        Assert.Contains("3) Expected multiple to contain a single item, but found 2.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromPredicateAndOrderCollectionAssertions()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-rules");
            values.Should().OnlyContain((int x) => x % 2 == 0);
            values.Should().NotContain((int x) => x == 2);
            values.Should().ContainInOrder([1, 3, 2]);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-rules' failed with 3 assertion failure(s):", message);
        Assert.Contains("1) Expected values to only contain items matching predicate `(int x) => x % 2 == 0` (first non-matching index 0), but found 1.", message);
        Assert.Contains("2) Expected values to not contain any item matching predicate `(int x) => x == 2` (first matching index 1), but found 2.", message);
        Assert.Contains("3) Expected values to contain items in order [1, 3, 2], but found missing expected item at sequence index 2: 2.", message);
    }

    [Fact]
    public void Batch_Dispose_UsesPredicateVariableNames_ForCollectionPredicateFailures()
    {
        int[] values = [1, 2, 3];
        Func<int, bool> isEven = x => x % 2 == 0;
        Func<int, bool> isTwo = x => x == 2;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-predicate-vars");
            values.Should().OnlyContain(isEven);
            values.Should().NotContain(isTwo);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-predicate-vars' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to only contain items matching predicate `isEven` (first non-matching index 0), but found 1.", message);
        Assert.Contains("2) Expected values to not contain any item matching predicate `isTwo` (first matching index 1), but found 2.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromItemAndAllSatisfyAssertions()
    {
        int[] values = [1, 2, -1];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-advanced");
            values.Should().NotContain(2);
            values.Should().AllSatisfy((int item) => item.Should().BeGreaterThan(0));
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-advanced' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to not contain 2, but found 2.", message);
        Assert.Contains("2) Expected item to be greater than 0, but found -1.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromContainExactlyAssertions()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-exact");
            values.Should().ContainExactly([1, 9, 3]);
            values.Should().ContainExactly([1, 2, 3, 4]);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-exact' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to contain exactly [1, 9, 3], but found item mismatch at index 1: expected 9 but found 2.", message);
        Assert.Contains("2) Expected values to contain exactly [1, 2, 3, 4], but found missing item at index 3: 4.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromContainExactlyInAnyOrderAssertions()
    {
        int[] values = [1, 2, 2];
        string[] names = ["Alpha", "beta", "delta"];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-exact-any-order");
            values.Should().ContainExactlyInAnyOrder([1, 2, 3]);
            names.Should().ContainExactlyInAnyOrder(
                ["alpha", "beta", "charlie"],
                StringComparer.OrdinalIgnoreCase);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-exact-any-order' failed with 2 assertion failure(s):", message);
        Assert.Contains(
            "1) Expected values to contain exactly in any order [1, 2, 3], but found missing item 3: expected count 1 but found 0.",
            message);
        Assert.Contains(
            "2) Expected names to contain exactly in any order [\"alpha\", \"beta\", \"charlie\"], but found missing item \"charlie\": expected count 1 but found 0.",
            message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromBeSubsetOfAssertions()
    {
        int[] values = [1, 4, 5];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-subset");
            values.Should().BeSubsetOf([1, 2, 3]);
            values.Should().BeSubsetOf([1, 2, 4]);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-subset' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to be a subset of [1, 2, 3], but found missing item at index 1: 4.", message);
        Assert.Contains("2) Expected values to be a subset of [1, 2, 4], but found missing item at index 2: 5.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromBeSupersetOfAssertions()
    {
        int[] values = [1, 2];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-superset");
            values.Should().BeSupersetOf([1, 2, 4]);
            values.Should().BeSupersetOf([1, 2, 5]);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-superset' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected values to be a superset of [1, 2, 4], but found missing expected item at index 2: 4.", message);
        Assert.Contains("2) Expected values to be a superset of [1, 2, 5], but found missing expected item at index 2: 5.", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromKeySelectedOrderAssertions()
    {
        WorkflowStep[] steps =
        [
            new(1, "validate"),
            new(2, "enrich"),
            new(3, "persist")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-order-keys");
            steps.Should().ContainInOrder([1, 3, 2], (WorkflowStep step) => step.Position);
            steps.Should().ContainInOrder([1, 3], (WorkflowStep step) => step.Position, allowGaps: false);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-order-keys' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected steps to contain selected values in order [1, 3, 2], but found missing expected selected value at sequence index 2: 2.", message);
        Assert.Contains("2) Expected steps to contain selected values in order with no gaps [1, 3], but found missing adjacent ordered sequence for selected values.", message);
    }

    [Fact]
    public void ComparerOverloads_InsideBatch_DoNotThrowAtAssertionCallSite()
    {
        string[] values = ["Alpha", "Beta"];
        string[] duplicates = ["Alpha", "alpha", "Gamma"];
        WorkflowStep[] steps =
        [
            new(1, "Validate"),
            new(2, "Enrich"),
            new(3, "Persist")
        ];

        using var batch = new Axiom.Core.Batch();

        var containEx = Record.Exception(() => values.Should().Contain("gamma", StringComparer.OrdinalIgnoreCase));
        var containAllEx = Record.Exception(() => values.Should().ContainAll(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase));
        var containAnyEx = Record.Exception(() => values.Should().ContainAny(["gamma", "delta"], StringComparer.OrdinalIgnoreCase));
        var notContainAnyEx = Record.Exception(() => values.Should().NotContainAny(["gamma", "alpha"], StringComparer.OrdinalIgnoreCase));
        var notContainEx = Record.Exception(() => values.Should().NotContain("alpha", StringComparer.OrdinalIgnoreCase));
        var uniqueEx = Record.Exception(() => duplicates.Should().HaveUniqueItems(StringComparer.OrdinalIgnoreCase));
        var containExactlyEx = Record.Exception(() => values.Should().ContainExactly(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase));
        var subsetEx = Record.Exception(() => values.Should().BeSubsetOf(["alpha"], StringComparer.OrdinalIgnoreCase));
        var supersetEx = Record.Exception(() => values.Should().BeSupersetOf(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase));
        var containInOrderEx = Record.Exception(() => values.Should().ContainInOrder(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase));
        var containInOrderByKeyEx = Record.Exception(() =>
            steps.Should().ContainInOrder(
                ["validate", "archive"],
                (WorkflowStep step) => step.Name,
                StringComparer.OrdinalIgnoreCase));

        Assert.Null(containEx);
        Assert.Null(containAllEx);
        Assert.Null(containAnyEx);
        Assert.Null(notContainAnyEx);
        Assert.Null(notContainEx);
        Assert.Null(uniqueEx);
        Assert.Null(containExactlyEx);
        Assert.Null(subsetEx);
        Assert.Null(supersetEx);
        Assert.Null(containInOrderEx);
        Assert.Null(containInOrderByKeyEx);

        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromComparerOverloads_InCallOrder()
    {
        string[] values = ["Alpha", "Beta"];
        string[] duplicates = ["Alpha", "alpha", "Gamma"];
        WorkflowStep[] steps =
        [
            new(1, "Validate"),
            new(2, "Enrich"),
            new(3, "Persist")
        ];

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("collection-comparers");
            values.Should().Contain("gamma", StringComparer.OrdinalIgnoreCase);
            values.Should().ContainAll(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase);
            values.Should().ContainAny(["gamma", "delta"], StringComparer.OrdinalIgnoreCase);
            values.Should().NotContainAny(["gamma", "alpha"], StringComparer.OrdinalIgnoreCase);
            values.Should().NotContain("alpha", StringComparer.OrdinalIgnoreCase);
            duplicates.Should().HaveUniqueItems(StringComparer.OrdinalIgnoreCase);
            values.Should().ContainExactly(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase);
            values.Should().BeSubsetOf(["alpha"], StringComparer.OrdinalIgnoreCase);
            values.Should().BeSupersetOf(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase);
            values.Should().ContainInOrder(["alpha", "gamma"], StringComparer.OrdinalIgnoreCase);
            steps.Should().ContainInOrder(
                ["validate", "archive"],
                (WorkflowStep step) => step.Name,
                StringComparer.OrdinalIgnoreCase);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'collection-comparers' failed with 11 assertion failure(s):", message);
        Assert.Contains("1) Expected values to contain \"gamma\", but found System.String[].", message);
        Assert.Contains("2) Expected values to contain all [\"alpha\", \"gamma\"], but found missing expected item at index 1: \"gamma\".", message);
        Assert.Contains("3) Expected values to contain any of [\"gamma\", \"delta\"], but found none of the expected items were found.", message);
        Assert.Contains("4) Expected values to not contain any of [\"gamma\", \"alpha\"], but found first matching item at subject index 0: \"Alpha\".", message);
        Assert.Contains("5) Expected values to not contain \"alpha\", but found \"Alpha\".", message);
        Assert.Contains("6) Expected duplicates to have unique items, but found first duplicate item at index 1: \"alpha\".", message);
        Assert.Contains("7) Expected values to contain exactly [\"alpha\", \"gamma\"], but found item mismatch at index 1: expected \"gamma\" but found \"Beta\".", message);
        Assert.Contains("8) Expected values to be a subset of [\"alpha\"], but found missing item at index 1: \"Beta\".", message);
        Assert.Contains("9) Expected values to be a superset of [\"alpha\", \"gamma\"], but found missing expected item at index 1: \"gamma\".", message);
        Assert.Contains("10) Expected values to contain items in order [\"alpha\", \"gamma\"], but found missing expected item at sequence index 1: \"gamma\".", message);
        Assert.Contains("11) Expected steps to contain selected values in order [\"validate\", \"archive\"], but found missing expected selected value at sequence index 1: \"archive\".", message);
    }
}
