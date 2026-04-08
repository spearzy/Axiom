using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.Temporal.BeOnOrBefore;

public sealed class BeOnOrBeforeTests
{
    [Fact]
    public void BeOnOrBefore_ReturnsContinuation_WhenDateTimeEqualsExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 00, DateTimeKind.Utc);

        var baseAssertions = actual.Should();
        var continuation = baseAssertions.BeOnOrBefore(actual);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeOnOrBefore_Throws_WhenDateTimeIsAfterExpected()
    {
        var actual = new DateTime(2026, 03, 03, 10, 00, 01, DateTimeKind.Utc);
        var expected = actual.AddSeconds(-1);

        var ex = Assert.Throws<InvalidOperationException>(() => actual.Should().BeOnOrBefore(expected));

        Assert.Contains("to be on or before", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeOnOrBefore_DoesNotThrow_WhenDateTimeOffsetEqualsExpected()
    {
        var actual = new DateTimeOffset(2026, 03, 03, 10, 00, 00, TimeSpan.Zero);

        var ex = Record.Exception(() => actual.Should().BeOnOrBefore(actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOnOrBefore_DoesNotThrow_WhenDateOnlyEqualsExpected()
    {
        var actual = new DateOnly(2026, 03, 03);

        var ex = Record.Exception(() => actual.Should().BeOnOrBefore(actual));

        Assert.Null(ex);
    }

    [Fact]
    public void BeOnOrBefore_DoesNotThrow_WhenTimeOnlyEqualsExpected()
    {
        var actual = new TimeOnly(10, 00, 00);

        var ex = Record.Exception(() => actual.Should().BeOnOrBefore(actual));

        Assert.Null(ex);
    }
}
