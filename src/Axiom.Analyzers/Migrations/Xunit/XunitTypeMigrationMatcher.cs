using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitTypeMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationKind kind,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        if (resultIsConsumed)
        {
            return false;
        }

        return kind switch
        {
            XunitAssertMigrationKind.BeOfType => IsSupportedGenericTypeAssertionOverload(invocation, symbols),
            XunitAssertMigrationKind.BeAssignableTo => IsSupportedGenericTypeAssertionOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedGenericTypeAssertionOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 1 ||
            method.Parameters.Length != 1 ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        var subjectType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[0]);
        return subjectType is not null && symbols.SupportsTypeAssertionMigrationReceiver(subjectType);
    }
}
