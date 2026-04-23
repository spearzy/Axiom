using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitThrowsMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationKind kind,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed,
        bool isAwaited)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Throw => IsSafeSupportedSyncOverload(invocation, symbols, resultIsConsumed),
            XunitAssertMigrationKind.ThrowExactlyAsync => IsSafeSupportedThrowsAsyncOverload(invocation, symbols, isAwaited),
            XunitAssertMigrationKind.ThrowAsync => IsSafeSupportedThrowsAnyAsyncOverload(invocation, symbols, isAwaited),
            _ => false,
        };
    }

    private static bool IsSafeSupportedSyncOverload(
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

    private static bool IsSafeSupportedThrowsAsyncOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        bool isAwaited)
    {
        return isAwaited && IsSupportedAsyncThrowShape(invocation, symbols, allowsParamName: true);
    }

    private static bool IsSafeSupportedThrowsAnyAsyncOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        bool isAwaited)
    {
        return isAwaited &&
               invocation.TargetMethod.Parameters.Length == 1 &&
               invocation.Arguments.Length == 1 &&
               IsSupportedAsyncThrowShape(invocation, symbols, allowsParamName: false);
    }

    private static bool IsSupportedAsyncThrowShape(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        bool allowsParamName)
    {
        var method = invocation.TargetMethod;
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 1)
        {
            return false;
        }

        if (method.Parameters.Length == 1 && invocation.Arguments.Length == 1)
        {
            return symbols.IsFuncReturningTask(method.Parameters[0].Type);
        }

        return allowsParamName &&
               method.Parameters.Length == 2 &&
               invocation.Arguments.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               symbols.IsFuncReturningTask(method.Parameters[1].Type) &&
               XunitAssertMigrationMatcher.IsSupportedExpectedConstantStringExpression(invocation.Arguments[0]);
    }
}
