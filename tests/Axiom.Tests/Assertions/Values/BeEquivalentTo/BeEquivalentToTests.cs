using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToTests
{
    [Fact]
    public void BeEquivalentTo_DoesNotThrow_WhenLeafValuesAreEqual()
    {
        var value = 42;

        var ex = Xunit.Record.Exception(() => value.Should().BeEquivalentTo(42));

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void BeEquivalentTo_ThrowsDeterministicReport_WhenLeafValuesDiffer()
    {
        var value = 42;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().BeEquivalentTo(7));

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        const string expected = """
            Expected value to be equivalent to 7, but found 1 difference(s):
            1) value -> expected 7, but found 42 (Values differ.)
            """;
        Xunit.Assert.Equal(expected, message);
    }

    [Fact]
    public void BeEquivalentTo_UsesConfiguredStringComparison()
    {
        object value = "ABC";

        var ex = Xunit.Record.Exception(() =>
            value.Should().BeEquivalentTo("abc", options => options.StringComparison = StringComparison.OrdinalIgnoreCase));

        Xunit.Assert.Null(ex);
    }

    [Fact]
    public void BeEquivalentTo_Throws_WhenRuntimeTypesDifferByDefault()
    {
        object value = 42;

        var ex = Xunit.Assert.Throws<InvalidOperationException>(() => value.Should().BeEquivalentTo(42L));

        Xunit.Assert.Contains("Runtime types differ", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("System.Int64", ex.Message, StringComparison.Ordinal);
        Xunit.Assert.Contains("System.Int32", ex.Message, StringComparison.Ordinal);
    }
}
