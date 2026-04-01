using System.Collections.Immutable;
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
        bool requiresAssertionsExtensionsNamespace,
        bool appendSingleItem,
        bool appendWhoseValue,
        bool appendThrown)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
        AppendSingleItem = appendSingleItem;
        AppendWhoseValue = appendWhoseValue;
        AppendThrown = appendThrown;
    }

    public XunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public TypeSyntax? TypeArgumentSyntax { get; }
    public bool RequiresAssertionsExtensionsNamespace { get; }
    public bool AppendSingleItem { get; }
    public bool AppendWhoseValue { get; }
    public bool AppendThrown { get; }
}

internal static class XunitAssertMigrationMatcher
{
    public static bool TryMatch(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols,
        out XunitAssertMigrationMatch match)
    {
        match = null!;

        // Stop early unless this is a normal xUnit Assert call.
        if (!symbols.IsEnabled ||
            invocation.Syntax is not InvocationExpressionSyntax invocationSyntax ||
            !symbols.IsXunitAssert(invocation.TargetMethod.ContainingType))
        {
            return false;
        }

        // We only rewrite simple positional calls for now.
        if (!HasOnlyPositionalArguments(invocationSyntax))
        {
            return false;
        }

        var resultIsConsumed = IsResultConsumed(invocation);

        // One xUnit method name can map to more than one migration rule. `Contains(...)` is the
        // main example: collection, string, and dictionary-key containment share the same method name.
        foreach (var spec in XunitAssertMigrationSpecs.GetByMethodName(invocation.TargetMethod.Name))
        {
            // If the basic shape does not match this rule, try the next rule.
            if (!MatchesArgumentCount(spec, invocation.TargetMethod.Parameters.Length) ||
                !MatchesArgumentCount(spec, invocation.Arguments.Length) ||
                !MatchesArgumentCount(spec, invocationSyntax.ArgumentList.Arguments.Count))
            {
                continue;
            }

            // Each rule has its own safety check. If this call is not a clean fit, keep looking.
            if (!IsSafeSupportedOverload(invocation, spec, symbols, resultIsConsumed))
            {
                continue;
            }

            // Pull out the parts the code fix needs: the receiver, the expected value, and any
            // generic type argument.
            var arguments = invocationSyntax.ArgumentList.Arguments;
            var subjectExpression = GetSubjectExpression(spec.Kind, arguments);
            var expectedExpression = GetExpectedExpression(spec.Kind, arguments);
            var typeArgumentSyntax = GetTypeArgumentSyntax(spec.Kind, invocationSyntax);

            // Some fixes only make sense for generic source calls.
            if (spec.Kind is XunitAssertMigrationKind.Throw or XunitAssertMigrationKind.BeOfType or XunitAssertMigrationKind.BeAssignableTo &&
                typeArgumentSyntax is null)
            {
                continue;
            }

            match = new XunitAssertMigrationMatch(
                spec,
                invocationSyntax,
                subjectExpression,
                expectedExpression,
                typeArgumentSyntax,
                RequiresAssertionsExtensionsNamespace(spec.Kind, GetSubjectType(spec.Kind, invocation.TargetMethod.Parameters)),
                appendSingleItem: resultIsConsumed && spec.Kind is XunitAssertMigrationKind.ContainSingle or XunitAssertMigrationKind.ContainSingleMatching,
                appendWhoseValue: resultIsConsumed && spec.Kind is XunitAssertMigrationKind.ContainKey,
                appendThrown: resultIsConsumed && spec.Kind is XunitAssertMigrationKind.Throw && expectedExpression is not null);

            return true;
        }

        return false;
    }

    private static bool HasOnlyPositionalArguments(InvocationExpressionSyntax invocationSyntax)
    {
        foreach (var argument in invocationSyntax.ArgumentList.Arguments)
        {
            // Named arguments and ref/out arguments need a different rewrite path.
            if (argument.NameColon is not null ||
                !argument.RefKindKeyword.IsKind(SyntaxKind.None))
            {
                return false;
            }
        }

        return true;
    }

    private static bool MatchesArgumentCount(XunitAssertMigrationSpec spec, int count)
    {
        return count == spec.RequiredArgumentCount ||
               (spec.AlternateArgumentCount is int alternateArgumentCount && count == alternateArgumentCount);
    }

    private static bool IsSafeSupportedOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSpec spec,
        XunitAssertMigrationSymbols symbols,
        bool resultIsConsumed)
    {
        // Pick the safety check that matches the rule we are trying to apply.
        var method = invocation.TargetMethod;

        switch (spec.Kind)
        {
            case XunitAssertMigrationKind.Be:
            case XunitAssertMigrationKind.NotBe:
                return !resultIsConsumed && IsSupportedEqualityOverload(invocation, symbols);

            case XunitAssertMigrationKind.BeTrue:
            case XunitAssertMigrationKind.BeFalse:
                return !resultIsConsumed && method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean;

            case XunitAssertMigrationKind.BeEmpty:
            case XunitAssertMigrationKind.NotBeEmpty:
                return !resultIsConsumed && !symbols.IsAsyncEnumerableLike(method.Parameters[0].Type);

            case XunitAssertMigrationKind.BeNull:
            case XunitAssertMigrationKind.NotBeNull:
                return !resultIsConsumed && IsSupportedNullOverload(invocation, symbols);

            case XunitAssertMigrationKind.Contain:
            case XunitAssertMigrationKind.NotContain:
                return !resultIsConsumed && IsSupportedCollectionContainmentOverload(method, symbols);

            case XunitAssertMigrationKind.ContainSubstring:
            case XunitAssertMigrationKind.NotContainSubstring:
                return !resultIsConsumed && IsSupportedStringContainmentOverload(invocation, symbols);

            case XunitAssertMigrationKind.ContainKey:
                return IsSupportedDictionaryKeyContainmentOverload(invocation, symbols, resultIsConsumed);

            case XunitAssertMigrationKind.NotContainKey:
                return !resultIsConsumed && IsSupportedDictionaryKeyContainmentOverload(invocation, symbols, resultIsConsumed: false);

            case XunitAssertMigrationKind.ContainSingle:
                return resultIsConsumed
                    ? IsSupportedConsumedSingleOverload(invocation, symbols)
                    : IsSupportedSingleOverload(method, symbols);

            case XunitAssertMigrationKind.ContainSingleMatching:
                return IsSupportedSinglePredicateOverload(invocation, symbols);

            case XunitAssertMigrationKind.BeSameAs:
            case XunitAssertMigrationKind.NotBeSameAs:
                return !resultIsConsumed && IsSupportedReferenceEqualityOverload(invocation, symbols);

            case XunitAssertMigrationKind.Throw:
                return IsSupportedThrowsOverload(invocation, symbols, resultIsConsumed);

            case XunitAssertMigrationKind.BeOfType:
                return !resultIsConsumed && IsSupportedIsTypeOverload(invocation, symbols);

            case XunitAssertMigrationKind.BeAssignableTo:
                return !resultIsConsumed && IsSupportedIsAssignableFromOverload(invocation, symbols);

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
        // Roslyn keeps conversions as separate nodes. We usually want the original expression type,
        // not the final parameter type after conversion.
        var operation = UnwrapConversions(argument.Value);

        return operation.Type ?? argument.Parameter?.Type;
    }

    private static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
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

    private static bool IsSupportedStringContainmentOverload(
        IInvocationOperation invocation,
        XunitAssertMigrationSymbols symbols)
    {
        var method = invocation.TargetMethod;
        // First make sure xUnit picked the simple string/string overload.
        if (method.Parameters.Length != 2 ||
            method.Parameters[0].Type.SpecialType != SpecialType.System_String ||
            method.Parameters[1].Type.SpecialType != SpecialType.System_String ||
            invocation.Arguments.Length != 2)
        {
            return false;
        }

        // Then make sure the source expression itself is really a string. This avoids producing
        // wrapper.Should().Contain(...) when xUnit only got here through an implicit conversion.
        var subjectType = GetArgumentType(invocation.Arguments[1]);
        return subjectType is not null && symbols.SupportsStringContainmentMigrationReceiver(subjectType);
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

        var subjectType = GetArgumentType(invocation.Arguments[1]);
        return subjectType is not null && symbols.SupportsDictionaryKeyContainmentMigrationReceiver(subjectType);
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

        var subjectType = GetArgumentType(invocation.Arguments[0]);
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
        // This rule only covers the generic xUnit shape:
        // Assert.Single<T>(IEnumerable<T> collection, Predicate<T> predicate)
        if (!method.IsGenericMethod ||
            method.TypeArguments.Length != 1 ||
            method.Parameters.Length != 2 ||
            invocation.Arguments.Length != 2)
        {
            return false;
        }

        var subjectType = GetArgumentType(invocation.Arguments[0]);
        if (subjectType is null ||
            subjectType.SpecialType == SpecialType.System_String ||
            !symbols.IsGenericEnumerableLike(subjectType) ||
            symbols.IsAsyncEnumerableLike(subjectType) ||
            symbols.IsSpanOrMemoryLike(subjectType))
        {
            return false;
        }

        // The second argument must stand on its own as a callable expression.
        // We skip target-typed cases like null/default and delegate variables.
        return symbols.IsPredicateType(method.Parameters[1].Type) &&
               IsSupportedSinglePredicateExpression(invocation.Arguments[1]);
    }

    private static bool IsSupportedSinglePredicateExpression(IArgumentOperation predicateArgument)
    {
        // Roslyn wraps lambdas and method groups in delegate creation/conversion nodes.
        // Peel those away, then check the real expression underneath.
        var operation = UnwrapSinglePredicateExpression(predicateArgument.Value);
        return operation is IAnonymousFunctionOperation or IMethodReferenceOperation;
    }

    private static IOperation UnwrapSinglePredicateExpression(IOperation operation)
    {
        // Roslyn can wrap the user's predicate in conversion/delegate nodes.
        // Remove them so we can inspect the expression they actually wrote.
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

    private static bool IsSupportedThrowsOverload(
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

        if (method.Parameters.Length == 1 &&
            invocation.Arguments.Length == 1)
        {
            return !resultIsConsumed &&
                   SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, symbols.ActionType);
        }

        return method.Parameters.Length == 2 &&
               invocation.Arguments.Length == 2 &&
               method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
               SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, symbols.ActionType) &&
               IsSupportedThrowsParamNameExpression(invocation.Arguments[0]);
    }

    private static bool IsSupportedThrowsParamNameExpression(IArgumentOperation paramNameArgument)
    {
        // Keep this rewrite high-confidence. We only migrate param-name throws when the source
        // param name is an obvious non-null constant string like "name" or nameof(name).
        var operation = UnwrapConversions(paramNameArgument.Value);
        var constantValue = operation.ConstantValue;

        return constantValue.HasValue && constantValue.Value is string;
    }

    private static bool IsSupportedIsTypeOverload(
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

        var subjectType = GetArgumentType(invocation.Arguments[0]);
        return subjectType is not null && symbols.SupportsTypeAssertionMigrationReceiver(subjectType);
    }

    private static bool IsSupportedIsAssignableFromOverload(
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

        var subjectType = GetArgumentType(invocation.Arguments[0]);
        return subjectType is not null && symbols.SupportsTypeAssertionMigrationReceiver(subjectType);
    }

    private static ExpressionSyntax GetSubjectExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        // xUnit does not always put the future fluent receiver in the same argument position.
        // This helper hides that detail from the code fix.
        => kind is XunitAssertMigrationKind.Be or
                 XunitAssertMigrationKind.NotBe or
                 XunitAssertMigrationKind.Contain or
                 XunitAssertMigrationKind.NotContain or
                 XunitAssertMigrationKind.ContainSubstring or
                 XunitAssertMigrationKind.NotContainSubstring or
                 XunitAssertMigrationKind.ContainKey or
                 XunitAssertMigrationKind.NotContainKey or
                 XunitAssertMigrationKind.BeSameAs or
                 XunitAssertMigrationKind.NotBeSameAs
            ? arguments[1].Expression
            : kind is XunitAssertMigrationKind.Throw && arguments.Count == 2
                ? arguments[1].Expression
            : arguments[0].Expression;

    private static ITypeSymbol GetSubjectType(
        XunitAssertMigrationKind kind,
        ImmutableArray<IParameterSymbol> parameters)
        // This is only used when deciding which using directives the fix needs.
        => kind is XunitAssertMigrationKind.Be or
                 XunitAssertMigrationKind.NotBe or
                 XunitAssertMigrationKind.Contain or
                 XunitAssertMigrationKind.NotContain or
                 XunitAssertMigrationKind.ContainSubstring or
                 XunitAssertMigrationKind.NotContainSubstring or
                 XunitAssertMigrationKind.ContainKey or
                 XunitAssertMigrationKind.NotContainKey or
                 XunitAssertMigrationKind.BeSameAs or
                 XunitAssertMigrationKind.NotBeSameAs
            ? parameters[1].Type
            : kind is XunitAssertMigrationKind.Throw && parameters.Length == 2
                ? parameters[1].Type
            : parameters[0].Type;

    private static ExpressionSyntax? GetExpectedExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
        => kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.BeSameAs or
            XunitAssertMigrationKind.NotBeSameAs or
            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey
                => arguments[0].Expression,
            XunitAssertMigrationKind.Throw when arguments.Count == 2
                => arguments[0].Expression,
            XunitAssertMigrationKind.ContainSingleMatching
                => arguments[1].Expression,
            _ => null,
        };

    private static TypeSyntax? GetTypeArgumentSyntax(
        XunitAssertMigrationKind kind,
        InvocationExpressionSyntax invocationSyntax)
    {
        // Only generic source calls give us a type argument to copy into the Axiom call.
        if (kind is not XunitAssertMigrationKind.Throw and not XunitAssertMigrationKind.BeOfType and not XunitAssertMigrationKind.BeAssignableTo)
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
        // Some generated calls need `Axiom.Assertions.Extensions`; others do not. This tells the
        // code fix which using directive to add.
        return kind switch
        {
            XunitAssertMigrationKind.BeTrue => true,
            XunitAssertMigrationKind.BeFalse => true,
            XunitAssertMigrationKind.BeEmpty => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.NotBeEmpty => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.Contain => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.NotContain => subjectType.SpecialType != SpecialType.System_String,
            XunitAssertMigrationKind.ContainSubstring => false,
            XunitAssertMigrationKind.NotContainSubstring => false,
            XunitAssertMigrationKind.ContainKey => true,
            XunitAssertMigrationKind.NotContainKey => true,
            XunitAssertMigrationKind.ContainSingle => true,
            XunitAssertMigrationKind.ContainSingleMatching => true,
            _ => false,
        };
    }

    private static bool IsResultConsumed(IInvocationOperation invocation)
    {
        IOperation current = invocation;
        while (current.Parent is IConversionOperation conversion)
        {
            current = conversion;
        }

        return current.Parent is not IExpressionStatementOperation;
    }
}
