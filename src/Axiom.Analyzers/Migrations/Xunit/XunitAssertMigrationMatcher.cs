using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.XunitMigration;

internal sealed class XunitAssertMigrationMatch
{
    public XunitAssertMigrationMatch(
        XunitAssertMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
    }

    public XunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
}

internal static class XunitAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        out XunitAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !symbols.IsXunitAssert(invocation.TargetMethod.ContainingType) ||
            !XunitAssertMigrationSpecs.TryGetByMethodName(invocation.TargetMethod.Name, out var spec))
        {
            return false;
        }

        if (invocation.TargetMethod.Parameters.Length != spec.RequiredArgumentCount ||
            invocation.Arguments.Length != spec.RequiredArgumentCount ||
            invocationSyntax.ArgumentList.Arguments.Count != spec.RequiredArgumentCount ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        if (!IsSafeSupportedOverload(invocation, spec, symbols))
        {
            return false;
        }

        var arguments = invocationSyntax.ArgumentList.Arguments;
        var subjectExpression = GetSubjectExpression(spec.Kind, arguments);
        var expectedExpression = GetExpectedExpression(spec.Kind, arguments);
        var typeArgumentSyntax = GetTypeArgumentSyntax(spec.Kind, invocationSyntax);

        if (spec.Kind is XunitAssertMigrationKind.Throw && typeArgumentSyntax is null)
        {
            return false;
        }

        match = new XunitAssertMigrationMatch(
            spec,
            invocationSyntax,
            subjectExpression,
            expectedExpression,
            typeArgumentSyntax,
            RequiresAssertionsExtensionsNamespace(spec.Kind, invocation.TargetMethod.Parameters[0].Type));

        return true;
    }

    private static bool HasOnlyPositionalArguments(InvocationExpressionSyntax invocationSyntax)
    {
        foreach (var argument in invocationSyntax.ArgumentList.Arguments)
        {
            if (argument.NameColon is not null ||
                !argument.RefKindKeyword.IsKind(SyntaxKind.None))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSpec spec,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;

        switch (spec.Kind)
        {
            case XunitAssertMigrationKind.Be:
            case XunitAssertMigrationKind.NotBe:
                return IsSupportedEqualityOverload(invocation, symbols);

            case XunitAssertMigrationKind.BeTrue:
            case XunitAssertMigrationKind.BeFalse:
                return method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean;

            case XunitAssertMigrationKind.BeEmpty:
            case XunitAssertMigrationKind.NotBeEmpty:
                return !symbols.IsAsyncEnumerableLike(method.Parameters[0].Type);

            case XunitAssertMigrationKind.BeNull:
            case XunitAssertMigrationKind.NotBeNull:
                return IsSupportedNullOverload(invocation, symbols);

            case XunitAssertMigrationKind.Contain:
            case XunitAssertMigrationKind.NotContain:
                return IsSupportedCollectionContainmentOverload(method, symbols);

            case XunitAssertMigrationKind.ContainSingle:
                return IsSupportedSingleOverload(method, symbols);

            case XunitAssertMigrationKind.BeSameAs:
            case XunitAssertMigrationKind.NotBeSameAs:
                return IsSupportedReferenceEqualityOverload(invocation, symbols);

            case XunitAssertMigrationKind.Throw:
                return IsSupportedThrowsOverload(method, symbols);

            default:
                return false;
        }
    }

    private static bool IsSupportedEqualityOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        if (invocation.Arguments.Length != 2)
        {
            return false;
        }

        var expectedType = GetArgumentType(invocation.Arguments[0]);
        var actualType = GetArgumentType(invocation.Arguments[1]);
        if (expectedType is null || actualType is null)
        {
            return false;
        }

        return !IsUnsupportedEqualityType(expectedType, symbols) &&
               !IsUnsupportedEqualityType(actualType, symbols) &&
               symbols.SupportsEqualityMigrationReceiver(actualType);
    }

    private static ITypeSymbol? GetArgumentType(IArgumentOperation argument)
    {
        var operation = argument.Value;
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation.Type ?? argument.Parameter?.Type;
    }

    private static bool IsSupportedNullOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var subjectType = GetArgumentType(invocation.Arguments[0]);
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

        var actualType = GetArgumentType(invocation.Arguments[1]);
        return actualType is not null && symbols.SupportsReferenceEqualityMigrationReceiver(actualType);
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

    private static bool IsSupportedThrowsOverload(
        IMethodSymbol method,
        XunitAssertMigrationSymbols symbols)
    {
        return method.IsGenericMethod &&
               method.TypeArguments.Length == 1 &&
               method.Parameters.Length == 1 &&
               symbols.ActionType is not null &&
               SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, symbols.ActionType);
    }

    private static ExpressionSyntax GetSubjectExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is XunitAssertMigrationKind.Be or
                 XunitAssertMigrationKind.NotBe or
                 XunitAssertMigrationKind.Contain or
                 XunitAssertMigrationKind.NotContain or
                 XunitAssertMigrationKind.BeSameAs or
                 XunitAssertMigrationKind.NotBeSameAs
            ? arguments[1].Expression
            : arguments[0].Expression;

    private static ExpressionSyntax? GetExpectedExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is XunitAssertMigrationKind.Be or
                 XunitAssertMigrationKind.NotBe or
                 XunitAssertMigrationKind.BeSameAs or
                 XunitAssertMigrationKind.NotBeSameAs or
                 XunitAssertMigrationKind.Contain or
                 XunitAssertMigrationKind.NotContain
            ? arguments[0].Expression
            : null;

    private static TypeSyntax? GetTypeArgumentSyntax(
        XunitAssertMigrationKind kind,
        InvocationExpressionSyntax invocationSyntax)
    {
        if (kind is not XunitAssertMigrationKind.Throw)
        {
            return null;
        }

        return invocationSyntax.Expression switch
        {
            GenericNameSyntax genericName when genericName.TypeArgumentList.Arguments.Count == 1
                => genericName.TypeArgumentList.Arguments[0],
            MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax genericName
            } when genericName.TypeArgumentList.Arguments.Count == 1
                => genericName.TypeArgumentList.Arguments[0],
            _ => null,
        };
    }

    private static bool RequiresAssertionsExtensionsNamespace(
        XunitAssertMigrationKind kind,
        ITypeSymbol subjectType)
    {
        return kind switch
        {
            XunitAssertMigrationKind.BeTrue => true,
            XunitAssertMigrationKind.BeFalse => true,
            XunitAssertMigrationKind.BeEmpty => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.NotBeEmpty => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.Contain => true,
            XunitAssertMigrationKind.NotContain => true,
            XunitAssertMigrationKind.ContainSingle => true,
            _ => false,
        };
    }
}
