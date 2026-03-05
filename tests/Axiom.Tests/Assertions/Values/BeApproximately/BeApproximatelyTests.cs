using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Values.BeApproximately;

public sealed class BeApproximatelyTests
{
    [Fact]
    public void BeApproximately_ReturnsContinuation_WhenDoubleIsWithinTolerance()
    {
        const double value = 10.05d;

        var baseAssertions = value.Should();
        var continuation = baseAssertions.BeApproximately(10d, 0.1d);

        Assert.Same(baseAssertions, continuation.And);
    }

    [Fact]
    public void BeApproximately_Throws_WhenDoubleIsOutsideTolerance()
    {
        const double value = 10.25d;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeApproximately(10d, 0.1d));

        const string expected = "Expected value to be within 0.1 of 10, but found 10.25.";
        Assert.Equal(expected, ex.Message);
    }

    [Fact]
    public void BeApproximately_Throws_WithReason_WhenProvided()
    {
        const decimal value = 12.5m;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeApproximately(10m, 0.2m, "payments must match within fee tolerance"));

        Assert.Contains("because payments must match within fee tolerance", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeApproximately_ThrowsArgumentOutOfRangeException_WhenDoubleToleranceIsNegative()
    {
        const double value = 10d;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().BeApproximately(10d, -0.01d));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeApproximately_ThrowsArgumentOutOfRangeException_WhenFloatToleranceIsNaN()
    {
        const float value = 10f;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().BeApproximately(10f, float.NaN));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeApproximately_ThrowsArgumentOutOfRangeException_WhenDecimalToleranceIsNegative()
    {
        const decimal value = 10m;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => value.Should().BeApproximately(10m, -0.1m));

        Assert.Equal("tolerance", ex.ParamName);
    }

    [Fact]
    public void BeApproximately_Passes_WhenDoubleValuesAreBothNaN()
    {
        const double value = double.NaN;

        var ex = Record.Exception(() => value.Should().BeApproximately(double.NaN, 0.1d));

        Assert.Null(ex);
    }

    [Fact]
    public void BeApproximately_Throws_WhenOnlyOneDoubleValueIsNaN()
    {
        const double value = 10d;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().BeApproximately(double.NaN, 0.1d));

        Assert.Contains("to be within 0.1 of NaN", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeApproximately_Passes_WhenDoubleValuesAreSameInfinity()
    {
        const double value = double.PositiveInfinity;

        var ex = Record.Exception(() => value.Should().BeApproximately(double.PositiveInfinity, 0.1d));

        Assert.Null(ex);
    }

    [Fact]
    public void BeApproximately_Throws_WhenDoubleValuesAreDifferentInfinitySigns()
    {
        const double value = double.NegativeInfinity;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            value.Should().BeApproximately(double.PositiveInfinity, 0.1d));

        Assert.Contains("to be within 0.1 of Infinity", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void BeApproximately_DoesNotThrow_WhenFloatIsWithinTolerance()
    {
        const float value = 10.05f;

        var ex = Record.Exception(() => value.Should().BeApproximately(10f, 0.1f));

        Assert.Null(ex);
    }

    [Fact]
    public void BeApproximately_DoesNotThrow_WhenDecimalIsWithinTolerance()
    {
        const decimal value = 10.05m;

        var ex = Record.Exception(() => value.Should().BeApproximately(10m, 0.1m));

        Assert.Null(ex);
    }
}
