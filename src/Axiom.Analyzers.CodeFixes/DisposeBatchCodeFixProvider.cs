using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Analyzers.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DisposeBatchCodeFixProvider)), Shared]
public sealed class DisposeBatchCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [AxiomAnalyzerIds.UndisposedBatch];

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

        var node = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var statement = node.FirstAncestorOrSelf<LocalDeclarationStatementSyntax>();
        if (statement is null || statement.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use 'using var' for Batch",
                cancellationToken => AddUsingAsync(context.Document, statement, cancellationToken),
                equivalenceKey: "UseUsingVarForBatch"),
            context.Diagnostics);
    }

    private static async Task<Document> AddUsingAsync(
        Document document,
        LocalDeclarationStatementSyntax statement,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var replacement = statement.WithUsingKeyword(
            SyntaxFactory.Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(SyntaxFactory.Space));

        var newRoot = root.ReplaceNode(statement, replacement);
        return document.WithSyntaxRoot(newRoot);
    }
}
