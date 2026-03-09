namespace Axiom.Core.Failures;

public sealed class MSTestFailureStrategy : IFailureStrategy
{
    public static MSTestFailureStrategy Instance { get; } = new();

    private static readonly Lazy<Func<string, Exception>> ExceptionFactory =
        new(static () => FrameworkFailureExceptionFactory.Create(FrameworkFailureStrategyDefinitions.MSTest));

    private MSTestFailureStrategy()
    {
    }

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);
        throw ExceptionFactory.Value(message);
    }
}
