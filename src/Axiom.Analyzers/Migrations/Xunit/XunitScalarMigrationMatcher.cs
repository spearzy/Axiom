using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitScalarMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationKind kind,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        var method = invocation.TargetMethod;

        return kind switch
        {
            XunitAssertMigrationKind.Be or XunitAssertMigrationKind.NotBe
                => !resultIsConsumed && IsSupportedEqualityOverload(invocation, symbols),
            XunitAssertMigrationKind.BeNull or XunitAssertMigrationKind.NotBeNull
                => !resultIsConsumed && IsSupportedNullOverload(invocation, symbols),
            XunitAssertMigrationKind.BeTrue or XunitAssertMigrationKind.BeFalse
                => !resultIsConsumed && method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean,
            XunitAssertMigrationKind.BeEmpty or XunitAssertMigrationKind.NotBeEmpty
                => !resultIsConsumed && !symbols.IsAsyncEnumerableLike(method.Parameters[0].Type),
            XunitAssertMigrationKind.BeSameAs or XunitAssertMigrationKind.NotBeSameAs
                => !resultIsConsumed && IsSupportedReferenceEqualityOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedEqualityOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length != 2)
        {
            return false;
        }

        var expectedType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[0]);
        var actualType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[1]);
        if (expectedType is null || actualType is null)
        {
            return false;
        }

        return !IsUnsupportedEqualityType(expectedType, symbols) &&
               !IsUnsupportedEqualityType(actualType, symbols) &&
               symbols.SupportsEqualityMigrationReceiver(actualType);
    }

    private static bool IsUnsupportedEqualityType(
        ITypeSymbol type,
        XunitAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static bool IsSupportedNullOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var subjectType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[0]);
        return subjectType is not null && symbols.SupportsNullMigrationReceiver(subjectType);
    }

    private static bool IsSupportedReferenceEqualityOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length != 2)
        {
            return false;
        }

        var actualType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[1]);
        return actualType is not null && symbols.SupportsReferenceEqualityMigrationReceiver(actualType);
    }
}
