using System.Collections.Immutable;
using System.Composition;
using Axiom.Analyzers.XunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XunitAssertMigrationCodeFixProvider)), Shared]
public sealed class XunitAssertMigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        XunitAssertMigrationSpecs.All.Select(static spec => spec.DiagnosticId).ToImmutableArray();

    public override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        if (root.FindNode(context.Span, getInnermostNodeForTie: true) is not InvocationExpressionSyntax invocationSyntax)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel is null ||
            semanticModel.GetOperation(invocationSyntax, context.CancellationToken) is not IInvocationOperation invocationOperation)
        {
            return;
        }

        var symbols = XunitAssertMigrationSymbols.Create(semanticModel.Compilation);
        if (!XunitAssertMigrationMatcher.TryMatch(invocationOperation, symbols, out var match))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                GetCodeFixTitle(match),
                cancellationToken => ApplyFixAsync(context.Document, match, cancellationToken),
                equivalenceKey: match.Spec.DiagnosticId),
            context.Diagnostics);
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        XunitAssertMigrationMatch match,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return document;
        }

        var replacementExpression = BuildReplacementExpression(match, semanticModel)
            .WithTriviaFrom(match.InvocationSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var rewrittenRoot = compilationUnit.ReplaceNode(match.InvocationSyntax, replacementExpression);
        rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions");

        if (RequiresSystemNamespace(match, semanticModel))
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "System");
        }

        if (match.RequiresAssertionsExtensionsNamespace)
        {
            rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions.Extensions");
        }

        return document.WithSyntaxRoot(rewrittenRoot);
    }

    private static CompilationUnitSyntax AddUsingIfMissing(
        CompilationUnitSyntax compilationUnit,
        string namespaceName)
    {
        foreach (var usingDirective in compilationUnit.Usings)
        {
            if (usingDirective.Alias is null &&
                !usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword) &&
                usingDirective.Name?.ToString() == namespaceName)
            {
                return compilationUnit;
            }
        }

        return compilationUnit.AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName))
                .WithAdditionalAnnotations(Formatter.Annotation));
    }

    private static ExpressionSyntax BuildReplacementExpression(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        if (match.Spec.Kind is XunitAssertMigrationKind.Throw)
        {
            return BuildThrowReplacementExpression(match, semanticModel);
        }

        var subjectExpression = PrepareSubjectExpression(match.SubjectExpression);
        var shouldInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                subjectExpression,
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList());

        var assertionMethodName = match.TypeArgumentSyntax is null
            ? (SimpleNameSyntax)SyntaxFactory.IdentifierName(GetAxiomMethodName(match.Spec.Kind))
            : SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(GetAxiomMethodName(match.Spec.Kind)),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(match.TypeArgumentSyntax.WithoutTrivia())));

        var assertionMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            assertionMethodName);

        ExpressionSyntax rewrittenExpression = match.ExpectedExpression is null
            ? SyntaxFactory.InvocationExpression(assertionMethod, SyntaxFactory.ArgumentList())
            : SyntaxFactory.InvocationExpression(
                assertionMethod,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(GetAssertionArgumentExpression(match).WithoutTrivia()))));

        if (match.AppendSingleItem)
        {
            rewrittenExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                rewrittenExpression,
                SyntaxFactory.IdentifierName("SingleItem"));
        }

        if (match.AppendWhoseValue)
        {
            rewrittenExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                rewrittenExpression,
                SyntaxFactory.IdentifierName("WhoseValue"));
        }

        return rewrittenExpression;
    }

    private static ExpressionSyntax BuildThrowReplacementExpression(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        var actionExpression = CanUseDirectThrowReceiver(match.SubjectExpression, semanticModel)
            ? PrepareSubjectExpression(match.SubjectExpression)
            : SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.IdentifierName("Action"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.SubjectExpression.WithoutTrivia()))),
                initializer: null);

        var shouldInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                actionExpression,
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList());

        var throwMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            SyntaxFactory.GenericName(
                SyntaxFactory.Identifier("Throw"),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(match.TypeArgumentSyntax!.WithoutTrivia()))));

        ExpressionSyntax rewrittenExpression =
            SyntaxFactory.InvocationExpression(throwMethod, SyntaxFactory.ArgumentList());

        if (match.ExpectedExpression is not null)
        {
            var withParamNameMethod = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                rewrittenExpression,
                SyntaxFactory.IdentifierName("WithParamName"));

            rewrittenExpression = SyntaxFactory.InvocationExpression(
                withParamNameMethod,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(match.ExpectedExpression.WithoutTrivia()))));
        }

        if (match.AppendThrown)
        {
            rewrittenExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                rewrittenExpression,
                SyntaxFactory.IdentifierName("Thrown"));
        }

        return rewrittenExpression;
    }

    private static bool CanUseDirectThrowReceiver(
        ExpressionSyntax subjectExpression,
        SemanticModel semanticModel)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(subjectExpression);
        if (symbolInfo.Symbol is IMethodSymbol ||
            symbolInfo.CandidateSymbols.Any(static symbol => symbol is IMethodSymbol))
        {
            return false;
        }

        var typeInfo = semanticModel.GetTypeInfo(subjectExpression);
        var actionType = semanticModel.Compilation.GetTypeByMetadataName("System.Action");
        if (actionType is null)
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(typeInfo.Type, actionType) ||
               SymbolEqualityComparer.Default.Equals(typeInfo.ConvertedType, actionType);
    }

    private static bool RequiresSystemNamespace(
        XunitAssertMigrationMatch match,
        SemanticModel semanticModel)
    {
        return match.Spec.Kind is XunitAssertMigrationKind.Throw &&
               !CanUseDirectThrowReceiver(match.SubjectExpression, semanticModel);
    }

    private static ExpressionSyntax PrepareSubjectExpression(ExpressionSyntax subjectExpression)
    {
        var withoutTrivia = subjectExpression.WithoutTrivia();
        if (NeedsParentheses(withoutTrivia))
        {
            return SyntaxFactory.ParenthesizedExpression(withoutTrivia);
        }

        return withoutTrivia;
    }

    private static ExpressionSyntax GetAssertionArgumentExpression(XunitAssertMigrationMatch match)
    {
        // The matcher already filtered AXM1019 down to safe predicate shapes.
        // At this point we can forward the source expression as-is.
        return match.ExpectedExpression!;
    }

    private static string GetCodeFixTitle(XunitAssertMigrationMatch match)
    {
        if (match.AppendSingleItem && match.Spec.Kind is XunitAssertMigrationKind.ContainSingle)
        {
            return "Convert to 'subject.Should().ContainSingle().SingleItem'";
        }

        if (match.AppendSingleItem && match.Spec.Kind is XunitAssertMigrationKind.ContainSingleMatching)
        {
            return "Convert to 'subject.Should().ContainSingle(...).SingleItem'";
        }

        if (match.AppendWhoseValue && match.Spec.Kind is XunitAssertMigrationKind.ContainKey)
        {
            return "Convert to 'dictionary.Should().ContainKey(key).WhoseValue'";
        }

        if (match.AppendThrown && match.Spec.Kind is XunitAssertMigrationKind.Throw)
        {
            return "Convert to '.Should().Throw<TException>().WithParamName(...).Thrown'";
        }

        if (match.ExpectedExpression is not null && match.Spec.Kind is XunitAssertMigrationKind.Throw)
        {
            return "Convert to '.Should().Throw<TException>().WithParamName(...)'";
        }

        return match.Spec.CodeFixTitle;
    }

    private static bool NeedsParentheses(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax => false,
            GenericNameSyntax => false,
            MemberAccessExpressionSyntax => false,
            ElementAccessExpressionSyntax => false,
            InvocationExpressionSyntax => false,
            ObjectCreationExpressionSyntax => false,
            ImplicitObjectCreationExpressionSyntax => false,
            ThisExpressionSyntax => false,
            BaseExpressionSyntax => false,
            LiteralExpressionSyntax => false,
            ParenthesizedExpressionSyntax => false,
            _ => true,
        };
    }

    private static string GetAxiomMethodName(XunitAssertMigrationKind kind)
    {
        return kind switch
        {
            XunitAssertMigrationKind.Be => "Be",
            XunitAssertMigrationKind.NotBe => "NotBe",
            XunitAssertMigrationKind.BeNull => "BeNull",
            XunitAssertMigrationKind.NotBeNull => "NotBeNull",
            XunitAssertMigrationKind.BeTrue => "BeTrue",
            XunitAssertMigrationKind.BeFalse => "BeFalse",
            XunitAssertMigrationKind.BeEmpty => "BeEmpty",
            XunitAssertMigrationKind.NotBeEmpty => "NotBeEmpty",
            XunitAssertMigrationKind.Contain => "Contain",
            XunitAssertMigrationKind.NotContain => "NotContain",
            XunitAssertMigrationKind.ContainSubstring => "Contain",
            XunitAssertMigrationKind.NotContainSubstring => "NotContain",
            XunitAssertMigrationKind.ContainKey => "ContainKey",
            XunitAssertMigrationKind.NotContainKey => "NotContainKey",
            XunitAssertMigrationKind.ContainSingle => "ContainSingle",
            XunitAssertMigrationKind.ContainSingleMatching => "ContainSingle",
            XunitAssertMigrationKind.BeSameAs => "BeSameAs",
            XunitAssertMigrationKind.NotBeSameAs => "NotBeSameAs",
            XunitAssertMigrationKind.BeOfType => "BeOfType",
            XunitAssertMigrationKind.BeAssignableTo => "BeAssignableTo",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }
}
