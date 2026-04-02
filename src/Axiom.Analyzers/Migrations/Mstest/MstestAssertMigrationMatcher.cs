using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.MstestMigration;

internal sealed class MstestAssertMigrationMatch
{
    public MstestAssertMigrationMatch(
        MstestAssertMigrationSpec spec,
        InvocationExpressionSyntax invocationSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        bool requiresAssertionsExtensionsNamespace)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
    }

    public MstestAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
}

internal static class MstestAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols,
        out MstestAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !symbols.IsMstestAssert(invocation.TargetMethod.ContainingType) ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        foreach (var spec in MstestAssertMigrationSpecs.GetByMethodName(invocation.TargetMethod.Name))
        {
            if (invocation.TargetMethod.Parameters.Length != spec.RequiredArgumentCount ||
                invocation.Arguments.Length != spec.RequiredArgumentCount ||
                invocationSyntax.ArgumentList.Arguments.Count != spec.RequiredArgumentCount)
            {
                continue;
            }

            if (!IsSupportedOverload(invocation, spec.Kind, symbols))
            {
                continue;
            }

            match = new MstestAssertMigrationMatch(
                spec,
                invocationSyntax,
                GetSubjectExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                GetExpectedExpression(spec.Kind, invocationSyntax.ArgumentList.Arguments),
                RequiresAssertionsExtensionsNamespace(spec.Kind));

            return true;
        }

        return false;
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

    private static bool IsSupportedOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationKind kind,
        MstestAssertMigrationSymbols symbols)
    {
        return kind switch
        {
            MstestAssertMigrationKind.Be or MstestAssertMigrationKind.NotBe
                => IsSupportedEqualityOverload(invocation, symbols),
            MstestAssertMigrationKind.BeNull or MstestAssertMigrationKind.NotBeNull
                => IsSupportedNullOverload(invocation, symbols),
            MstestAssertMigrationKind.BeTrue or MstestAssertMigrationKind.BeFalse
                => IsSupportedBooleanOverload(invocation),
            MstestAssertMigrationKind.BeSameAs or MstestAssertMigrationKind.NotBeSameAs
                => IsSupportedReferenceEqualityOverload(invocation, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedEqualityOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsEqualityMigrationReceiver) &&
               !IsUnsupportedEqualityType(subjectType, symbols) &&
               IsSupportedEqualityExpectedExpression(invocation.Arguments[0], subjectType, symbols);
    }

    private static bool IsSupportedNullOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, symbols.SupportsNullMigrationReceiver);
    }

    private static bool IsSupportedBooleanOverload(IInvocationOperation invocation)
    {
        var subjectType = GetReceiverType(invocation.Arguments[0]);
        return subjectType is not null &&
               subjectType.SpecialType == SpecialType.System_Boolean &&
               IsSupportedReceiverExpression(invocation.Arguments[0], subjectType, static type => type.SpecialType == SpecialType.System_Boolean);
    }

    private static bool IsSupportedReferenceEqualityOverload(
        IInvocationOperation invocation,
        MstestAssertMigrationSymbols symbols)
    {
        var subjectType = GetReceiverType(invocation.Arguments[1]);
        return subjectType is not null &&
               IsSupportedReceiverExpression(invocation.Arguments[1], subjectType, symbols.SupportsReferenceEqualityMigrationReceiver);
    }

    private static bool IsSupportedEqualityExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        MstestAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            if (IsNullLikeExpectedExpression(operation))
            {
                return MstestAssertMigrationSymbols.IsNullableOrReferenceType(subjectType);
            }

            return false;
        }

        if (IsUnsupportedEqualityType(operation.Type, symbols))
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(operation.Type, subjectType);
        return conversion.Exists && conversion.IsImplicit;
    }

    private static bool IsUnsupportedEqualityType(
        ITypeSymbol type,
        MstestAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static bool IsSupportedReceiverExpression(
        IArgumentOperation argument,
        ITypeSymbol subjectType,
        Func<ITypeSymbol, bool> receiverPredicate)
    {
        var operation = UnwrapConversions(argument.Value);
        if (operation.Syntax is LiteralExpressionSyntax literalExpression &&
            (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
             literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)))
        {
            return false;
        }

        return operation.Type is not null && receiverPredicate(subjectType);
    }

    private static bool IsNullLikeExpectedExpression(IOperation operation)
    {
        return operation.Syntax is LiteralExpressionSyntax literalExpression &&
               (literalExpression.IsKind(SyntaxKind.NullLiteralExpression) ||
                literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression)) ||
               operation.ConstantValue is { HasValue: true, Value: null };
    }

    private static ITypeSymbol? GetReceiverType(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        return operation.Type;
    }

    private static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static ExpressionSyntax GetSubjectExpression(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is MstestAssertMigrationKind.Be or
                 MstestAssertMigrationKind.NotBe or
                 MstestAssertMigrationKind.BeSameAs or
                 MstestAssertMigrationKind.NotBeSameAs
            ? arguments[1].Expression
            : arguments[0].Expression;

    private static ExpressionSyntax? GetExpectedExpression(
        MstestAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind is MstestAssertMigrationKind.Be or
                 MstestAssertMigrationKind.NotBe or
                 MstestAssertMigrationKind.BeSameAs or
                 MstestAssertMigrationKind.NotBeSameAs
            ? arguments[0].Expression
            : null;

    private static bool RequiresAssertionsExtensionsNamespace(MstestAssertMigrationKind kind)
        => kind is MstestAssertMigrationKind.BeTrue or MstestAssertMigrationKind.BeFalse;
}
