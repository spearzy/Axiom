using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.OnlyContain;

public sealed class OnlyContainTests
{
    [Fact]
    public void OnlyContain_ReturnsContinuation_WhenAllItemsMatchPredicate()
    {
        int[] values = [2, 4, 6];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.OnlyContain((int x) => x % 2 == 0);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void OnlyContain_Throws_WhenAnyItemDoesNotMatchPredicate()
    {
        int[] values = [2, 3, 6];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().OnlyContain((int x) => x % 2 == 0));

        const string expected = "Expected values to only contain items matching predicate `(int x) => x % 2 == 0` (first non-matching index 1), but found 3.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void OnlyContain_Throws_WithReason_WhenProvided()
    {
        int[] values = [2, 3, 6];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().OnlyContain((int x) => x % 2 == 0, "all IDs must be even"));

        Assert.Contains("because all IDs must be even", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void OnlyContain_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        int[] values = [1];
        Func<int, bool>? predicate = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().OnlyContain(predicate!));

        Assert.Equal("predicate", ex.ParamName);
    }

    [Fact]
    public void OnlyContain_UsesPredicateVariableName_InFailureMessage()
    {
        int[] values = [2, 3, 6];
        Func<int, bool> isEven = x => x % 2 == 0;

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().OnlyContain(isEven));

        Assert.Contains("predicate `isEven`", ex.Message, StringComparison.Ordinal);
    }
}
