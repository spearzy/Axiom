using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitThrowsMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        var method = invocation.TargetMethod;
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 1 ||
            symbols.ActionType is null)
        {
            return false;
        }

        if (method.Parameters.Length == 1 && invocation.Arguments.Length == 1)
        {
            return !resultIsConsumed &&
                   SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, symbols.ActionType);
        }

        return method.Parameters.Length == 2 &&
               invocation.Arguments.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, symbols.ActionType) &&
               XunitAssertMigrationMatcher.IsSupportedExpectedConstantStringExpression(invocation.Arguments[0]);
    }
}
