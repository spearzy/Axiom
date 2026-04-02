using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal static class XunitContainmentMigrationMatcher
{
    public static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationKind kind,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Contain or XunitAssertMigrationKind.NotContain
                => !resultIsConsumed && IsSupportedCollectionContainmentOverload(invocation.TargetMethod, symbols),
            XunitAssertMigrationKind.ContainSubstring or XunitAssertMigrationKind.NotContainSubstring
                => !resultIsConsumed && IsSupportedStringContainmentOverload(invocation, symbols),
            XunitAssertMigrationKind.StartWith or XunitAssertMigrationKind.EndWith
                => !resultIsConsumed && IsSupportedStringPrefixSuffixOverload(invocation, symbols),
            XunitAssertMigrationKind.ContainKey
                => IsSupportedDictionaryKeyContainmentOverload(invocation, symbols, resultIsConsumed),
            XunitAssertMigrationKind.NotContainKey
                => !resultIsConsumed && IsSupportedDictionaryKeyContainmentOverload(invocation, symbols, resultIsConsumed: false),
            _ => false,
        };
    }

    private static bool IsSupportedCollectionContainmentOverload(
        IMethodSymbol method,
        XunitAssertMigrationSymbols symbols)
    {
        if (method.Parameters.Length != 2)
        {
            return false;
        }

        var collectionType = method.Parameters[1].Type;
        return symbols.IsGenericEnumerableLike(collectionType) &&
               !symbols.IsAsyncEnumerableLike(collectionType) &&
               !symbols.IsDictionaryLike(collectionType);
    }

    private static bool IsSupportedStringContainmentOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        if (method.Parameters.Length != 2 ||
            method.Parameters[0].Type.SpecialType != SpecialType.System_String ||
            method.Parameters[1].Type.SpecialType != SpecialType.System_String ||
            invocation.Arguments.Length != 2)
        {
            return false;
        }

        return IsSupportedStringReceiverExpression(invocation.Arguments[1], symbols);
    }

    private static bool IsSupportedStringPrefixSuffixOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        if (method.Parameters.Length != 2 ||
            method.Parameters[0].Type.SpecialType != SpecialType.System_String ||
            method.Parameters[1].Type.SpecialType != SpecialType.System_String ||
            invocation.Arguments.Length != 2)
        {
            return false;
        }

        return XunitAssertMigrationMatcher.IsSupportedExpectedConstantStringExpression(invocation.Arguments[0]) &&
               IsSupportedStringReceiverExpression(invocation.Arguments[1], symbols);
    }

    private static bool IsSupportedStringReceiverExpression(
        IArgumentOperation argument,
        XunitAssertMigrationSymbols symbols)
    {
        var operation = XunitAssertMigrationMatcher.UnwrapConversions(argument.Value);
        if (operation.Syntax is LiteralExpressionSyntax literalExpression &&
            (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
             literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            return false;
        }

        return operation.Type is not null &&
               symbols.SupportsStringContainmentMigrationReceiver(operation.Type);
    }

    private static bool IsSupportedDictionaryKeyContainmentOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        var method = invocation.TargetMethod;
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 2 ||
            method.Parameters.Length != 2 ||
            invocation.Arguments.Length != 2 ||
            method.Parameters[1].Type is not INamedTypeSymbol dictionaryParameter ||
            !symbols.IsDictionaryParameterType(dictionaryParameter))
        {
            return false;
        }

        if (resultIsConsumed && method.ReturnsVoid)
        {
            return false;
        }

        var subjectType = XunitAssertMigrationMatcher.GetArgumentType(invocation.Arguments[1]);
        return subjectType is not null && symbols.SupportsDictionaryKeyContainmentMigrationReceiver(subjectType);
    }
}
