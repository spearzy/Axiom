namespace Axiom.Tests.Core.Configuration;

public sealed class FailureStrategyTests : IDisposable
{
    public void Dispose()
    {
        AxiomServices.Reset();
    }

    [Fact]
    public void Assertions_UseInvalidOperationException_ByDefault()
    {
        const int value = 42;

        var ex = Assert.Throws<InvalidOperationException>(() => value.Should().Be(7));

        Assert.Contains("Expected value to be 7, but found 42.", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Assertions_UseConfiguredFailureStrategy_OutsideBatch()
    {
        var strategy = new TestFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        const int value = 42;
        var ex = Assert.Throws<TestFailureException>(() => value.Should().Be(7));

        Assert.Equal("Expected value to be 7, but found 42.", ex.Message);
        Assert.Contains(ex, strategy.Failures);
    }

    [Fact]
    public void BatchDispose_UsesConfiguredFailureStrategy_ForCombinedFailure()
    {
        var strategy = new TestFailureStrategy();
        AxiomServices.Configure(c => c.FailureStrategy = strategy);

        var ex = Assert.Throws<TestFailureException>(() =>
        {
            using var batch = Axiom.Core.Assert.Batch("numbers");
            42.Should().Be(7);
            99.Should().Be(100);
        });

        var message = ex.Message.Replace("\r\n", "\n", StringComparison.Ordinal);
        Assert.Contains("Batch 'numbers' failed with 2 assertion failure(s):", message);
        Assert.Contains("1) Expected 42 to be 7, but found 42.", message);
        Assert.Contains("2) Expected 99 to be 100, but found 99.", message);
        Assert.Contains(ex, strategy.Failures);
    }

    private sealed class TestFailureStrategy : IFailureStrategy
    {
        public List<TestFailureException> Failures { get; } = new();

        public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
        {
            var ex = new TestFailureException(message, callerFilePath, callerLineNumber);
            Failures.Add(ex);
            throw ex;
        }
    }

    private sealed class TestFailureException : Exception
    {
        public TestFailureException(string message, string? callerFilePath, int callerLineNumber)
            : base(message)
        {
            CallerFilePath = callerFilePath;
            CallerLineNumber = callerLineNumber;
        }

        public string? CallerFilePath { get; }
        public int CallerLineNumber { get; }
    }
}
