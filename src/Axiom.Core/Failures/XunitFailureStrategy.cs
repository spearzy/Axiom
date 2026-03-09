namespace Axiom.Core.Failures;

public sealed class XunitFailureStrategy : IFailureStrategy
{
    public static XunitFailureStrategy Instance { get; } = new();

    private static readonly Lazy<Func<string, Exception>> ExceptionFactory =
        new(static () => FrameworkFailureExceptionFactory.Create(FrameworkFailureStrategyDefinitions.Xunit));

    private XunitFailureStrategy()
    {
    }

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);
        throw ExceptionFactory.Value(message);
    }
}
