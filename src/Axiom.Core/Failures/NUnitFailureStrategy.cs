namespace Axiom.Core.Failures;

public sealed class NUnitFailureStrategy : IFailureStrategy
{
    public static NUnitFailureStrategy Instance { get; } = new();

    private static readonly Lazy<Func<string, Exception>> ExceptionFactory =
        new(static () => FrameworkFailureExceptionFactory.Create(FrameworkFailureStrategyDefinitions.NUnit));

    private NUnitFailureStrategy()
    {
    }

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);
        throw ExceptionFactory.Value(message);
    }
}
