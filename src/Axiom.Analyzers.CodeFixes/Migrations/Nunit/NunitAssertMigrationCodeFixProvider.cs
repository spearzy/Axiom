using System.Collections.Immutable;
using System.Composition;
using Axiom.Analyzers.NunitMigration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NunitAssertMigrationCodeFixProvider)), Shared]
public sealed class NunitAssertMigrationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        NunitAssertMigrationSpecs.All.Select(static spec => spec.DiagnosticId).ToImmutableArray();

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

        var symbols = NunitAssertMigrationSymbols.Create(semanticModel.Compilation);
        if (!NunitAssertMigrationMatcher.TryMatch(invocationOperation, symbols, out var match))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                match.Spec.CodeFixTitle,
                cancellationToken => ApplyFixAsync(context.Document, match, cancellationToken),
                equivalenceKey: match.Spec.DiagnosticId),
            context.Diagnostics);
    }

    private static async Task<Document> ApplyFixAsync(
        Document document,
        NunitAssertMigrationMatch match,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var replacementExpression = NunitScalarMigrationRewriter.BuildReplacementExpression(match)
            .WithTriviaFrom(match.InvocationSyntax)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var rewrittenRoot = compilationUnit.ReplaceNode(match.InvocationSyntax, replacementExpression);
        rewrittenRoot = AddUsingIfMissing(rewrittenRoot, "Axiom.Assertions");

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

    internal static ExpressionSyntax BuildShouldCall(
        ExpressionSyntax subjectExpression,
        string methodName,
        ExpressionSyntax? argumentExpression = null)
    {
        var shouldInvocation = BuildShouldInvocation(subjectExpression);
        var assertionMethod = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            SyntaxFactory.IdentifierName(methodName));

        return BuildInvocation(assertionMethod, argumentExpression);
    }

    private static ExpressionSyntax BuildShouldInvocation(ExpressionSyntax subjectExpression)
    {
        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                PrepareSubjectExpression(subjectExpression),
                SyntaxFactory.IdentifierName("Should")),
            SyntaxFactory.ArgumentList());
    }

    private static ExpressionSyntax BuildInvocation(
        ExpressionSyntax targetExpression,
        ExpressionSyntax? argumentExpression = null)
    {
        if (argumentExpression is null)
        {
            return SyntaxFactory.InvocationExpression(targetExpression, SyntaxFactory.ArgumentList());
        }

        return SyntaxFactory.InvocationExpression(
            targetExpression,
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(argumentExpression.WithoutTrivia()))));
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

    private static bool NeedsParentheses(ExpressionSyntax expression)
    {
        return expression is BinaryExpressionSyntax or
               ConditionalExpressionSyntax or
               AssignmentExpressionSyntax or
               CastExpressionSyntax or
               SwitchExpressionSyntax or
               AwaitExpressionSyntax;
    }
}
