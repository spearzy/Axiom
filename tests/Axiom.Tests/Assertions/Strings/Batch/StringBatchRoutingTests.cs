using Axiom.Assertions.EntryPoints;

namespace Axiom.Tests.Assertions.Strings.Batch;

public sealed class StringBatchRoutingTests
{
    [Fact]
    public void Contain_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().Contain("ab"));
    }

    [Fact]
    public void Contain_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().Contain("ab"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotContain_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotContain("es"));
    }

    [Fact]
    public void NotContain_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotContain("es"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveLength_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().HaveLength(3));
    }

    [Fact]
    public void HaveLength_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().HaveLength(3));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void HaveLength_WithNegativeExpectedLength_ThrowsArgumentOutOfRangeImmediately()
    {
        const string value = "test";

        Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().HaveLength(-1));
    }

    [Fact]
    public void BeEmpty_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().BeEmpty());
    }

    [Fact]
    public void BeEmpty_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeEmpty());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeEmpty_OutsideBatch_ThrowsImmediately()
    {
        const string value = "";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeEmpty());
    }

    [Fact]
    public void NotBeEmpty_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeEmpty());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromStringAssertions()
    {
        const string valueA = "test";
        const string valueB = "test";
        const string valueC = "test";
        const string valueD = "test";
        const string valueE = "";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("string-extended");
            valueA.Should().Contain("ab");
            valueB.Should().NotContain("es");
            valueC.Should().HaveLength(3);
            valueD.Should().BeEmpty();
            valueE.Should().NotBeEmpty();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'string-extended' failed with 5 assertion failure(s):", message);
        Assert.Contains("1) Expected valueA to contain \"ab\", but found \"test\".", message);
        Assert.Contains("2) Expected valueB to not contain \"es\", but found \"test\".", message);
        Assert.Contains("3) Expected valueC to have length 3, but found 4.", message);
        Assert.Contains("4) Expected valueD to be empty, but found \"test\".", message);
        Assert.Contains("5) Expected valueE to not be empty, but found \"\".", message);
    }
}
