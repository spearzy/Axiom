using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.NunitMigration;

internal sealed class NunitAssertMigrationMatch
{
    public NunitAssertMigrationMatch(
        NunitAssertMigrationSpec spec,
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

    public NunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
}

internal static class NunitAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        NunitAssertMigrationSymbols symbols,
        out NunitAssertMigrationMatch match)
    {
        match = null!;

        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "That" ||
            !symbols.IsNunitAssert(invocation.TargetMethod.ContainingType) ||
            invocation.Arguments.Length != 2 ||
            invocation.TargetMethod.Parameters.Length < 2 ||
            !symbols.IsResolveConstraint(invocation.TargetMethod.Parameters[1].Type) ||
            !HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        var constraintOperation = UnwrapConversions(invocation.Arguments[1].Value);
        foreach (var spec in NunitAssertMigrationSpecs.All)
        {
            if (!TryMatchConstraint(constraintOperation, spec.Kind, symbols, out var expectedExpression, out var expectedArgument))
            {
                continue;
            }

            var subjectType = GetReceiverType(invocation.Arguments[0]);
            if (subjectType is null ||
                !IsSupportedSubject(invocation.Arguments[0], subjectType, expectedArgument, spec.Kind, symbols))
            {
                continue;
            }

            match = new NunitAssertMigrationMatch(
                spec,
                invocationSyntax,
                invocationSyntax.ArgumentList.Arguments[0].Expression,
                expectedExpression,
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

    private static bool TryMatchConstraint(
        IOperation constraintOperation,
        NunitAssertMigrationKind kind,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        return kind switch
        {
            NunitAssertMigrationKind.Be => TryMatchEqualTo(constraintOperation, symbols, negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotBe => TryMatchEqualTo(constraintOperation, symbols, negated: true, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeNull => TryMatchPropertyConstraint(constraintOperation, symbols, "Null", negated: false),
            NunitAssertMigrationKind.NotBeNull => TryMatchPropertyConstraint(constraintOperation, symbols, "Null", negated: true),
            NunitAssertMigrationKind.BeTrue => TryMatchPropertyConstraint(constraintOperation, symbols, "True", negated: false),
            NunitAssertMigrationKind.BeFalse => TryMatchPropertyConstraint(constraintOperation, symbols, "False", negated: false),
            NunitAssertMigrationKind.BeEmpty => TryMatchPropertyConstraint(constraintOperation, symbols, "Empty", negated: false),
            NunitAssertMigrationKind.NotBeEmpty => TryMatchPropertyConstraint(constraintOperation, symbols, "Empty", negated: true),
            NunitAssertMigrationKind.ContainSubstring => TryMatchDoesStringMethod(constraintOperation, symbols, "Contain", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotContainSubstring => TryMatchDoesStringMethod(constraintOperation, symbols, "Contain", negated: true, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.StartWith => TryMatchDoesStringMethod(constraintOperation, symbols, "StartWith", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.EndWith => TryMatchDoesStringMethod(constraintOperation, symbols, "EndWith", negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.HaveCount => TryMatchCountEqualTo(constraintOperation, symbols, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.BeSameAs => TryMatchSameAs(constraintOperation, symbols, negated: false, out expectedExpression, out expectedArgument),
            NunitAssertMigrationKind.NotBeSameAs => TryMatchSameAs(constraintOperation, symbols, negated: true, out expectedExpression, out expectedArgument),
            _ => false,
        };
    }

    private static bool TryMatchEqualTo(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "EqualTo" ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsIsType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchPropertyConstraint(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string propertyName,
        bool negated)
    {
        if (constraintOperation is not IPropertyReferenceOperation propertyReference ||
            propertyReference.Property.Name != propertyName)
        {
            return false;
        }

        if (negated)
        {
            return symbols.IsConstraintExpressionType(propertyReference.Property.ContainingType) &&
                   IsDirectNotExpression(propertyReference.Instance, symbols);
        }

        return propertyReference.Instance is null &&
               symbols.IsIsType(propertyReference.Property.ContainingType);
    }

    private static bool TryMatchDoesStringMethod(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        string methodName,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != methodName ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectDoesNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsDoesType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchCountEqualTo(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "EqualTo" ||
            invocation.Arguments.Length != 1 ||
            !symbols.IsCountConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
            !IsDirectHasCountExpression(invocation.Instance, symbols))
        {
            return false;
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool TryMatchSameAs(
        IOperation constraintOperation,
        NunitAssertMigrationSymbols symbols,
        bool negated,
        out ExpressionSyntax? expectedExpression,
        out IArgumentOperation? expectedArgument)
    {
        expectedExpression = null;
        expectedArgument = null;

        if (constraintOperation is not IInvocationOperation invocation ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            invocation.TargetMethod.Name != "SameAs" ||
            invocation.Arguments.Length != 1)
        {
            return false;
        }

        if (negated)
        {
            if (!symbols.IsConstraintExpressionType(invocation.TargetMethod.ContainingType) ||
                !IsDirectNotExpression(invocation.Instance, symbols))
            {
                return false;
            }
        }
        else
        {
            if (invocation.Instance is not null ||
                !symbols.IsIsType(invocation.TargetMethod.ContainingType))
            {
                return false;
            }
        }

        expectedExpression = invocationSyntax.ArgumentList.Arguments[0].Expression;
        expectedArgument = invocation.Arguments[0];
        return true;
    }

    private static bool IsDirectNotExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Not" &&
               propertyReference.Instance is null &&
               symbols.IsIsType(propertyReference.Property.ContainingType) &&
               symbols.IsConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsDirectDoesNotExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Not" &&
               propertyReference.Instance is null &&
               symbols.IsDoesType(propertyReference.Property.ContainingType) &&
               symbols.IsConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsDirectHasCountExpression(IOperation? operation, NunitAssertMigrationSymbols symbols)
    {
        var unwrapped = operation is null ? null : UnwrapConversions(operation);
        return unwrapped is IPropertyReferenceOperation propertyReference &&
               propertyReference.Property.Name == "Count" &&
               propertyReference.Instance is null &&
               symbols.IsHasType(propertyReference.Property.ContainingType) &&
               symbols.IsCountConstraintExpressionType(propertyReference.Type);
    }

    private static bool IsSupportedSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationKind kind,
        NunitAssertMigrationSymbols symbols)
    {
        return kind switch
        {
            NunitAssertMigrationKind.Be or NunitAssertMigrationKind.NotBe
                => IsSupportedEqualitySubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeNull or NunitAssertMigrationKind.NotBeNull
                => IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsNullMigrationReceiver),
            NunitAssertMigrationKind.BeTrue or NunitAssertMigrationKind.BeFalse
                => IsSupportedReceiverExpression(subjectArgument, subjectType, static type => type.SpecialType == SpecialType.System_Boolean),
            NunitAssertMigrationKind.BeEmpty or NunitAssertMigrationKind.NotBeEmpty
                => IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEmptyMigrationReceiver),
            NunitAssertMigrationKind.ContainSubstring or NunitAssertMigrationKind.NotContainSubstring
                => IsSupportedStringConstraintSubject(subjectArgument, subjectType, expectedArgument, symbols, requireConstantExpected: false),
            NunitAssertMigrationKind.StartWith or NunitAssertMigrationKind.EndWith
                => IsSupportedStringConstraintSubject(subjectArgument, subjectType, expectedArgument, symbols, requireConstantExpected: true),
            NunitAssertMigrationKind.HaveCount
                => IsSupportedCountSubject(subjectArgument, subjectType, expectedArgument, symbols),
            NunitAssertMigrationKind.BeSameAs or NunitAssertMigrationKind.NotBeSameAs
                => IsSupportedReferenceEqualitySubject(subjectArgument, subjectType, expectedArgument, symbols),
            _ => false,
        };
    }

    private static bool IsSupportedEqualitySubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        return expectedArgument is not null &&
               IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEqualityMigrationReceiver) &&
               !IsUnsupportedEqualityType(subjectType, symbols) &&
               IsSupportedEqualityExpectedExpression(expectedArgument, subjectType, symbols);
    }

    private static bool IsSupportedEqualityExpectedExpression(
        IArgumentOperation expectedArgument,
        ITypeSymbol subjectType,
        NunitAssertMigrationSymbols symbols)
    {
        var operation = UnwrapConversions(expectedArgument.Value);
        if (operation.Type is null)
        {
            if (IsNullLikeExpectedExpression(operation))
            {
                return NunitAssertMigrationSymbols.IsNullableOrReferenceType(subjectType);
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

    private static bool IsUnsupportedEqualityType(ITypeSymbol type, NunitAssertMigrationSymbols symbols)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        return symbols.IsEnumerableLike(type) ||
               symbols.IsAsyncEnumerableLike(type) ||
               symbols.IsSpanOrMemoryLike(type);
    }

    private static bool IsSupportedStringConstraintSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols,
        bool requireConstantExpected)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsEqualityMigrationReceiver) ||
            subjectType.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        var expectedOperation = UnwrapConversions(expectedArgument.Value);
        if (expectedOperation.Type?.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        if (!requireConstantExpected)
        {
            return true;
        }

        return expectedOperation.ConstantValue is { HasValue: true, Value: string };
    }

    private static bool IsSupportedCountSubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsCountMigrationReceiver))
        {
            return false;
        }

        var expectedType = UnwrapConversions(expectedArgument.Value).Type;
        return expectedType?.SpecialType == SpecialType.System_Int32;
    }

    private static bool IsSupportedReferenceEqualitySubject(
        IArgumentOperation subjectArgument,
        ITypeSymbol subjectType,
        IArgumentOperation? expectedArgument,
        NunitAssertMigrationSymbols symbols)
    {
        if (expectedArgument is null ||
            !IsSupportedReceiverExpression(subjectArgument, subjectType, symbols.SupportsReferenceEqualityMigrationReceiver))
        {
            return false;
        }

        var expectedType = UnwrapConversions(expectedArgument.Value).Type;
        if (expectedType is null)
        {
            return false;
        }

        var conversion = symbols.Compilation.ClassifyConversion(expectedType, subjectType);
        return conversion.Exists && conversion.IsImplicit;
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

    internal static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static bool RequiresAssertionsExtensionsNamespace(NunitAssertMigrationKind kind)
        => kind is NunitAssertMigrationKind.BeTrue or
                    NunitAssertMigrationKind.BeFalse or
                    NunitAssertMigrationKind.BeEmpty or
                    NunitAssertMigrationKind.NotBeEmpty or
                    NunitAssertMigrationKind.HaveCount;
}
