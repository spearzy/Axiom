using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Collections.NotContain;

public sealed class NotContainTests
{
    [Fact]
    public void NotContain_ReturnsContinuation_WhenNoItemsMatchPredicate()
    {
        int[] values = [1, 2, 3];

        var baseAssertions = values.Should();
        var continuation = baseAssertions.NotContain((int x) => x < 0);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void NotContain_Throws_WhenAnyItemMatchesPredicate()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() => values.Should().NotContain((int x) => x == 2));

        const string expected = "Expected values to not contain any item matching predicate (first matching index 1), but found 2.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void NotContain_Throws_WithReason_WhenProvided()
    {
        int[] values = [1, 2, 3];

        var ex = Assert.Throws<InvalidOperationException>(() =>
            values.Should().NotContain((int x) => x == 2, "reserved IDs are not allowed"));

        Assert.Contains("because reserved IDs are not allowed", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void NotContain_ThrowsArgumentNullException_WhenPredicateIsNull()
    {
        int[] values = [1];
        Func<int, bool>? predicate = null;

        var ex = Assert.Throws<ArgumentNullException>(() => values.Should().NotContain(predicate!));

        Assert.Equal("predicate", ex.ParamName);
    }
}
