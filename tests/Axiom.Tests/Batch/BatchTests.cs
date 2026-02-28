namespace Axiom.Tests.Batch;

public class BatchTests
{
    [Fact]
    public void Current_IsSet_AndRestored_ForNestedBatches()
    {
        Xunit.Assert.Null(Axiom.Core.Batch.Current);

        using (var outer = Axiom.Core.Assert.Batch("outer"))
        {
            Xunit.Assert.Same(outer, Axiom.Core.Batch.Current);

            using (var inner = new Axiom.Core.Batch("inner"))
            {
                Xunit.Assert.Same(inner, Axiom.Core.Batch.Current);
            }

            Xunit.Assert.Same(outer, Axiom.Core.Batch.Current);
        }

        Xunit.Assert.Null(Axiom.Core.Batch.Current);
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenNoFailures()
    {
        var exception = Xunit.Record.Exception(() =>
        {
            using var _ = new Axiom.Core.Batch();
        });

        Xunit.Assert.Null(exception);
    }

    [Fact]
    public void Root_Dispose_ThrowsSingleCombinedException_ForNestedFailures()
    {
        var exception = Xunit.Assert.Throws<InvalidOperationException>(() =>
        {
            using var outer = new Axiom.Core.Batch("outer");
            outer.AddFailure("first");

            using (var inner = new Axiom.Core.Batch("inner"))
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
