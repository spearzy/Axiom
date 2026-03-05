using Axiom.Assertions;

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
    public void BeNullOrEmpty_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().BeNullOrEmpty());
    }

    [Fact]
    public void BeNullOrEmpty_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeNullOrEmpty());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeNullOrEmpty_OutsideBatch_ThrowsImmediately()
    {
        const string value = "";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void NotBeNullOrEmpty_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeNullOrEmpty());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void BeNullOrWhiteSpace_OutsideBatch_ThrowsImmediately()
    {
        const string value = "test";

        Assert.Throws<InvalidOperationException>(() => value.Should().BeNullOrWhiteSpace());
    }

    [Fact]
    public void BeNullOrWhiteSpace_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "test";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeNullOrWhiteSpace());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_OutsideBatch_ThrowsImmediately()
    {
        const string value = " ";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public void NotBeNullOrWhiteSpace_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = " ";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotBeNullOrWhiteSpace());

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Match_OutsideBatch_ThrowsImmediately()
    {
        const string value = "AB-12";

        Assert.Throws<InvalidOperationException>(() => value.Should().Match(@"^[A-Z]{2}-\d{3}$"));
    }

    [Fact]
    public void Match_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "AB-12";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().Match(@"^[A-Z]{2}-\d{3}$"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void NotMatch_OutsideBatch_ThrowsImmediately()
    {
        const string value = "AB-123";

        Assert.Throws<InvalidOperationException>(() => value.Should().NotMatch(@"^[A-Z]{2}-\d{3}$"));
    }

    [Fact]
    public void NotMatch_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "AB-123";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().NotMatch(@"^[A-Z]{2}-\d{3}$"));

        Assert.Null(callEx);
        Assert.Throws<InvalidOperationException>(() => batch.Dispose());
    }

    [Fact]
    public void Match_ThrowsArgumentException_WhenPatternIsInvalid()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentException>(() => value.Should().Match("["));

        Assert.Equal("pattern", ex.ParamName);
    }

    [Fact]
    public void Match_ThrowsArgumentOutOfRangeException_WhenTimeoutIsNotPositive()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().Match(@"\w+", TimeSpan.Zero));

        Assert.Equal("timeout", ex.ParamName);
    }

    [Fact]
    public void NotMatch_ThrowsArgumentException_WhenPatternIsInvalid()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentException>(() => value.Should().NotMatch("["));

        Assert.Equal("pattern", ex.ParamName);
    }

    [Fact]
    public void NotMatch_ThrowsArgumentOutOfRangeException_WhenTimeoutIsNotPositive()
    {
        const string value = "AB-123";

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().NotMatch(@"\w+", TimeSpan.Zero));

        Assert.Equal("timeout", ex.ParamName);
    }

    [Fact]
    public void BeEquivalentTo_OutsideBatch_ThrowsImmediately()
    {
        const string value = "ABC";

        Assert.Throws<InvalidOperationException>(() => value.Should().BeEquivalentTo("abc", StringComparison.Ordinal));
    }

    [Fact]
    public void BeEquivalentTo_InsideBatch_DoesNotThrowAtAssertionCallSite()
    {
        const string value = "ABC";

        using var batch = new Axiom.Core.Batch();
        var callEx = Record.Exception(() => value.Should().BeEquivalentTo("abc", StringComparison.Ordinal));

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

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNullOrEmptyAndEquivalentAssertions()
    {
        const string valueA = "test";
        const string valueB = "";
        const string valueC = "ABC";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("string-empty-equivalent");
            valueA.Should().BeNullOrEmpty();
            valueB.Should().NotBeNullOrEmpty();
            valueC.Should().BeEquivalentTo("abc", StringComparison.Ordinal);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'string-empty-equivalent' failed with 3 assertion failure(s):", message);
        Assert.Contains("1) Expected valueA to be null or empty, but found \"test\".", message);
        Assert.Contains("2) Expected valueB to not be null or empty, but found \"\".", message);
        Assert.Contains("3) Expected valueC to be equivalent to \"abc\", but found \"ABC\".", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromNullOrWhiteSpaceAssertions()
    {
        const string valueA = "test";
        const string valueB = " ";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("string-whitespace");
            valueA.Should().BeNullOrWhiteSpace();
            valueB.Should().NotBeNullOrWhiteSpace();
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'string-whitespace' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected valueA to be null or white-space, but found \"test\".", message);
        Assert.Contains("2) Expected valueB to not be null or white-space, but found \" \".", message);
    }

    [Fact]
    public void Batch_Dispose_ThrowsCombinedFailures_FromRegexAssertions()
    {
        const string valueA = "AB-12";
        const string valueB = "AB-123";

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            using var batch = new Axiom.Core.Batch("string-regex");
            valueA.Should().Match(@"^[A-Z]{2}-\d{3}$");
            valueB.Should().NotMatch(@"^[A-Z]{2}-\d{3}$");
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'string-regex' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected valueA to match regex \"^[A-Z]{2}-\\d{3}$\", but found \"AB-12\".", message);
        Assert.Contains("2) Expected valueB to not match regex \"^[A-Z]{2}-\\d{3}$\", but found \"AB-123\".", message);
    }
}
