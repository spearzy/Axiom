using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeOnOrAfter;

public sealed class BeOnOrAfterTests
{
    [Fact]
    public void BeOnOrAfter_ReturnsContinuation_WhenDateTimeEqualsExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeOnOrAfter(actual);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeOnOrAfter_Throws_WhenDateTimeIsBeforeExpected()
    {
        var actual = new DateTime(2026, 03, 03, 09, 59, 59, DateTimeKind.Utc);
        var expected = actual.AddSeconds(1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeOnOrAfter(expected));

        Assert.Contains("to be on or after", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeOnOrAfter_DoesNotThrow_WhenDateTimeOffsetEqualsExpected()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        var ex = Record.Exception(() => actual.Should().BeOnOrAfter(actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOnOrAfter_DoesNotThrow_WhenDateOnlyEqualsExpected()
    {
        var actual = new DateOnly(2026, 03, 03);

        var ex = Record.Exception(() => actual.Should().BeOnOrAfter(actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOnOrAfter_DoesNotThrow_WhenTimeOnlyEqualsExpected()
    {
        var actual = new TimeOnly(10, 00, 00);

        var ex = Record.Exception(() => actual.Should().BeOnOrAfter(actual));

        Assert.Null(ex);
    }
}
