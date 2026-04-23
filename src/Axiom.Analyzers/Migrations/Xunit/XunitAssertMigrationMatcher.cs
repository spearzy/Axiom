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
        AwaitExpressionSyntax? awaitExpressionSyntax,
        ExpressionSyntax subjectExpression,
        ExpressionSyntax? expectedExpression,
        ExpressionSyntax? additionalArgumentExpression,
        TypeSyntax? typeArgumentSyntax,
        bool requiresAssertionsExtensionsNamespace,
        bool appendSingleItem,
        bool appendWhoseValue,
        bool appendThrown)
    {
        Spec = spec;
        InvocationSyntax = invocationSyntax;
        AwaitExpressionSyntax = awaitExpressionSyntax;
        SubjectExpression = subjectExpression;
        ExpectedExpression = expectedExpression;
        AdditionalArgumentExpression = additionalArgumentExpression;
        TypeArgumentSyntax = typeArgumentSyntax;
        RequiresAssertionsExtensionsNamespace = requiresAssertionsExtensionsNamespace;
        AppendSingleItem = appendSingleItem;
        AppendWhoseValue = appendWhoseValue;
        AppendThrown = appendThrown;
    }

    public XunitAssertMigrationSpec Spec { get; }
    public InvocationExpressionSyntax InvocationSyntax { get; }
    public AwaitExpressionSyntax? AwaitExpressionSyntax { get; }
    public ExpressionSyntax SubjectExpression { get; }
    public ExpressionSyntax? ExpectedExpression { get; }
    public ExpressionSyntax? AdditionalArgumentExpression { get; }
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
        var awaitExpressionSyntax = GetAwaitExpressionSyntax(invocationSyntax);
        var awaitedResultIsConsumed = awaitExpressionSyntax is not null && IsAwaitedResultConsumed(awaitExpressionSyntax);

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
            var additionalArgumentExpression = GetAdditionalArgumentExpression(spec.Kind, arguments);
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
                awaitExpressionSyntax,
                subjectExpression,
                expectedExpression,
                additionalArgumentExpression,
                typeArgumentSyntax,
                RequiresAssertionsExtensionsNamespace(spec.Kind, GetSubjectType(spec.Kind, invocation.TargetMethod.Parameters)),
                appendSingleItem: resultIsConsumed && spec.Kind is XunitAssertMigrationKind.ContainSingle or XunitAssertMigrationKind.ContainSingleMatching,
                appendWhoseValue: resultIsConsumed && spec.Kind is XunitAssertMigrationKind.ContainKey,
                appendThrown: spec.Kind switch
                {
                    XunitAssertMigrationKind.Throw => resultIsConsumed && expectedExpression is not null,
                    XunitAssertMigrationKind.ThrowExactlyAsync or XunitAssertMigrationKind.ThrowAsync => awaitedResultIsConsumed,
                    _ => false,
                });

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
        return spec.Kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.BeNull or
            XunitAssertMigrationKind.NotBeNull or
            XunitAssertMigrationKind.BeTrue or
            XunitAssertMigrationKind.BeFalse or
            XunitAssertMigrationKind.BeEmpty or
            XunitAssertMigrationKind.NotBeEmpty or
            XunitAssertMigrationKind.BeSameAs or
            XunitAssertMigrationKind.NotBeSameAs
                => XunitScalarMigrationMatcher.IsSafeSupportedOverload(invocation, spec.Kind, symbols, resultIsConsumed),

            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey
                => XunitContainmentMigrationMatcher.IsSafeSupportedOverload(invocation, spec.Kind, symbols, resultIsConsumed),

            XunitAssertMigrationKind.ContainSingle or
            XunitAssertMigrationKind.ContainSingleMatching
                => XunitSingleMigrationMatcher.IsSafeSupportedOverload(invocation, spec.Kind, symbols, resultIsConsumed),

            XunitAssertMigrationKind.Throw or
            XunitAssertMigrationKind.ThrowExactlyAsync or
            XunitAssertMigrationKind.ThrowAsync
                => XunitThrowsMigrationMatcher.IsSafeSupportedOverload(
                    invocation,
                    spec.Kind,
                    symbols,
                    resultIsConsumed,
                    GetAwaitExpressionSyntax(invocation.Syntax) is not null),

            XunitAssertMigrationKind.BeOfType or
            XunitAssertMigrationKind.BeAssignableTo
                => XunitTypeMigrationMatcher.IsSafeSupportedOverload(invocation, spec.Kind, symbols, resultIsConsumed),

            _ => false,
        };
    }

    internal static ITypeSymbol? GetArgumentType(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        return operation.Type ?? argument.Parameter?.Type;
    }

    internal static IOperation UnwrapConversions(IOperation operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    internal static bool IsSupportedExpectedConstantStringExpression(IArgumentOperation argument)
    {
        var operation = UnwrapConversions(argument.Value);
        var constantValue = operation.ConstantValue;
        return constantValue.HasValue && constantValue.Value is string;
    }

    private static ExpressionSyntax GetSubjectExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey or
            XunitAssertMigrationKind.BeSameAs or
            XunitAssertMigrationKind.NotBeSameAs
                => arguments[1].Expression,
            XunitAssertMigrationKind.Throw when arguments.Count == 2
                => arguments[1].Expression,
            XunitAssertMigrationKind.ThrowExactlyAsync when arguments.Count == 2
                => arguments[1].Expression,
            _ => arguments[0].Expression,
        };
    }

    private static ITypeSymbol GetSubjectType(
        XunitAssertMigrationKind kind,
        ImmutableArray<IParameterSymbol> parameters)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.Contain or
            XunitAssertMigrationKind.NotContain or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey or
            XunitAssertMigrationKind.BeSameAs or
            XunitAssertMigrationKind.NotBeSameAs
                => parameters[1].Type,
            XunitAssertMigrationKind.Throw when parameters.Length == 2
                => parameters[1].Type,
            XunitAssertMigrationKind.ThrowExactlyAsync when parameters.Length == 2
                => parameters[1].Type,
            _ => parameters[0].Type,
        };
    }

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
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith or
            XunitAssertMigrationKind.ContainKey or
            XunitAssertMigrationKind.NotContainKey
                => arguments[0].Expression,
            XunitAssertMigrationKind.Throw when arguments.Count == 2
                => arguments[0].Expression,
            XunitAssertMigrationKind.ThrowExactlyAsync when arguments.Count == 2
                => arguments[0].Expression,
            XunitAssertMigrationKind.ContainSingleMatching
                => arguments[1].Expression,
            _ => null,
        };

    private static ExpressionSyntax? GetAdditionalArgumentExpression(
        XunitAssertMigrationKind kind,
        SeparatedSyntaxList<ArgumentSyntax> arguments)
    {
        if (arguments.Count != 3)
        {
            return null;
        }

        return kind switch
        {
            XunitAssertMigrationKind.Be or
            XunitAssertMigrationKind.NotBe or
            XunitAssertMigrationKind.ContainSubstring or
            XunitAssertMigrationKind.NotContainSubstring or
            XunitAssertMigrationKind.StartWith or
            XunitAssertMigrationKind.EndWith
                => arguments[2].Expression,
            _ => null,
        };
    }

    private static TypeSyntax? GetTypeArgumentSyntax(
        XunitAssertMigrationKind kind,
        InvocationExpressionSyntax invocationSyntax)
    {
        // Only generic source calls give us a type argument to copy into the Axiom call.
        if (kind is not XunitAssertMigrationKind.Throw and
            not XunitAssertMigrationKind.ThrowExactlyAsync and
            not XunitAssertMigrationKind.ThrowAsync and
            not XunitAssertMigrationKind.BeOfType and
            not XunitAssertMigrationKind.BeAssignableTo)
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
            XunitAssertMigrationKind.StartWith => false,
            XunitAssertMigrationKind.EndWith => false,
            XunitAssertMigrationKind.ContainKey => true,
            XunitAssertMigrationKind.NotContainKey => true,
            XunitAssertMigrationKind.ContainSingle => true,
            XunitAssertMigrationKind.ContainSingleMatching => true,
            _ => false,
        };
    }

    private static AwaitExpressionSyntax? GetAwaitExpressionSyntax(IOperation operation)
    {
        IOperation current = operation;
        while (current.Parent is IConversionOperation conversion)
        {
            current = conversion;
        }

        return current.Parent?.Syntax as AwaitExpressionSyntax;
    }

    private static AwaitExpressionSyntax? GetAwaitExpressionSyntax(SyntaxNode syntax)
    {
        return syntax.Parent as AwaitExpressionSyntax;
    }

    private static bool IsAwaitedResultConsumed(AwaitExpressionSyntax awaitExpressionSyntax)
        => awaitExpressionSyntax.Parent is not ExpressionStatementSyntax;

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
