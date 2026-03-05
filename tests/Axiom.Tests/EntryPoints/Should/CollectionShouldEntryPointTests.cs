using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.EntryPoints.Should;

public sealed class CollectionShouldEntryPointTests
{
    [Fact]
    public void Should_ForArray_UsesCollectionAssertionExtensions()
    {
        int[] values = [1, 2, 3];

        var continuation = values.Should().Contain(2);

        Assert.Same(values, continuation.And.Subject);
        Assert.Equal("values", continuation.And.SubjectExpression);
    }

    [Fact]
    public void Should_ForEnumerableVariable_AllowsNullSubject()
    {
        IEnumerable<int>? values = null;

        var ex = Assert.Throws<InvalidOperationException>(() => values!.Should().Contain(1));

        const string expected = "Expected values to contain 1, but found <null>.";
        Assert.Equal(expected, ex.Message);
    }
}
