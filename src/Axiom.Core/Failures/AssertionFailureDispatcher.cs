using System.Diagnostics.CodeAnalysis;
using Axiom.Core.Configuration;

namespace Axiom.Core.Failures;

public static class AssertionFailureDispatcher
{
    public static void Fail(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);

        var batch = Batch.Current;
        if (batch is not null)
        {
            batch.AddFailure(message);
            return;
        }

        Throw(message, callerFilePath, callerLineNumber);
    }

    public static void Throw(string message, string? callerFilePath = null, int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(message);
        AxiomServices.Configuration.FailureStrategy.Fail(message, callerFilePath, callerLineNumber);
        throw new InvalidOperationException(FailureStrategyMessages.NonThrowingStrategyGuard);
    }
}
