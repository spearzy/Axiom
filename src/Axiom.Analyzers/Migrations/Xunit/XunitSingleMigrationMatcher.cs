using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitSingleMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationKind kind,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        return kind switch
        {
            XunitAssertMigrationKind.ContainSingle
                => resultIsConsumed
                    ? IsSupportedConsumedSingleOverload(invocation, symbols)
                    : IsSupportedSingleOverload(invocation.TargetMethod, symbols),
            XunitAssertMigrationKind.ContainSingleMatching
                => IsSupportedSinglePredicateOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedSingleOverload(
        IMethodSymbol method,
        XunitAssertMigrationSymbols symbols)
    {
        if (method.Parameters.Length != 1)
        {
            return false;
        }

        var subjectType = method.Parameters[0].Type;
        return subjectType.SpecialType != SpecialType.System_String &&
               symbols.IsEnumerableLike(subjectType) &&
               !symbols.IsAsyncEnumerableLike(subjectType) &&
               !symbols.IsSpanOrMemoryLike(subjectType);
    }

    private static bool IsSupportedConsumedSingleOverload(
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
        return subjectType is not null &&
               subjectType.SpecialType != SpecialType.System_String &&
               symbols.IsGenericEnumerableLike(subjectType) &&
               !symbols.IsAsyncEnumerableLike(subjectType) &&
               !symbols.IsSpanOrMemoryLike(subjectType);
    }

    private static bool IsSupportedSinglePredicateOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 1 ||
            method.Parameters.Length != 2 ||
            invocation.Arguments.Length != 2)
        {
            return false;
        }

        var subjectType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[0]);
        if (subjectType is null ||
            subjectType.SpecialType == SpecialType.System_String ||
            !symbols.IsGenericEnumerableLike(subjectType) ||
            symbols.IsAsyncEnumerableLike(subjectType) ||
            symbols.IsSpanOrMemoryLike(subjectType))
        {
            return false;
        }

        return symbols.IsPredicateType(method.Parameters[1].Type) &&
               IsSupportedSinglePredicateExpression(invocation.Arguments[1]);
    }

    private static bool IsSupportedSinglePredicateExpression(IArgumentOperation predicateArgument)
    {
        var operation = UnwrapSinglePredicateExpression(predicateArgument.Value);
        return operation is IAnonymousFunctionOperation or IMethodReferenceOperation;
    }

    private static IOperation UnwrapSinglePredicateExpression(IOperation operation)
    {
        while (true)
        {
            switch (operation)
            {
                case IConversionOperation conversion:
                    operation = conversion.Operand;
                    continue;
                case IDelegateCreationOperation delegateCreation:
                    operation = delegateCreation.Target;
                    continue;
                default:
                    return operation;
            }
        }
    }
}
