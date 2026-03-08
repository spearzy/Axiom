namespace Axiom.Core.Failures;

public interface IFailureStrategy
{
    void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0);
}
