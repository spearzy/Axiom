namespace Axiom.Tests;

public class BatchTests
{
    [Fact]
    public void Current_IsSet_AndRestored_ForNestedBatches()
    {
        Xunit.Assert.Null(Batch.Current);

        using (var outer = Axiom.Assert.Batch("outer"))
        {
            Xunit.Assert.Same(outer, Batch.Current);

            using (var inner = new Batch("inner"))
            {
                Xunit.Assert.Same(inner, Batch.Current);
            }

            Xunit.Assert.Same(outer, Batch.Current);
        }

        Xunit.Assert.Null(Batch.Current);
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenNoFailures()
    {
        var exception = Xunit.Record.Exception(() =>
        {
            using var _ = new Batch();
        });

        Xunit.Assert.Null(exception);
    }

    [Fact]
    public void Root_Dispose_ThrowsSingleCombinedException_ForNestedFailures()
    {
        var exception = Xunit.Assert.Throws<InvalidOperationException>(() =>
        {
            using var outer = new Batch("outer");
            outer.AddFailure("first");

            using (var inner = new Batch("inner"))
            {
                inner.AddFailure("second");
            }

            outer.AddFailure("third");
        });

        const string expected = "Batch 'outer' failed with 3 assertion failure(s):\n1) first\n2) second\n3) third";
        Xunit.Assert.Equal(expected, NormaliseNewLines(exception.Message));
    }

    private static string NormaliseNewLines(string value)
    {
        return value.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
