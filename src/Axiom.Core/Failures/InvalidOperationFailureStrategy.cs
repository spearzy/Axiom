namespace Axiom.Core.Failures;

public sealed class InvalidOperationFailureStrategy : IFailureStrategy
{
    public static InvalidOperationFailureStrategy Instance { get; } = new();

    public void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        throw new InvalidOperationException(message);
    }
}
