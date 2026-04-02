using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Axiom.Analyzers.Tests.Helpers;

internal static class AnalyzerVerifier
{
    public static async Task VerifyAnalyzerAsync<TAnalyzer>(string source)
        where TAnalyzer : Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer, new()
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
        {
            TestCode = source,
        };

        test.TestState.Sources.Add(AxiomAssertionStubs.Source);
        test.TestState.Sources.Add(XunitAssertStubs.Source);
        test.TestState.Sources.Add(NunitAssertStubs.Source);
        await test.RunAsync();
    }

    public static async Task VerifyCodeFixAsync<TAnalyzer, TCodeFixProvider>(string source, string fixedSource)
        where TAnalyzer : Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        var test = new CSharpCodeFixTest<TAnalyzer, TCodeFixProvider, XUnitVerifier>
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.TestState.Sources.Add(AxiomAssertionStubs.Source);
        test.TestState.Sources.Add(XunitAssertStubs.Source);
        test.TestState.Sources.Add(NunitAssertStubs.Source);
        test.FixedState.Sources.Add(AxiomAssertionStubs.Source);
        test.FixedState.Sources.Add(XunitAssertStubs.Source);
        test.FixedState.Sources.Add(NunitAssertStubs.Source);
        await test.RunAsync();
    }

    public static async Task VerifyAppliedCodeFixAsync<TAnalyzer, TCodeFixProvider>(string source, string expectedFixedSource)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        // Some code-fix tests are easier to verify by applying the first fix directly
        // and comparing the rewritten source text.
        var actualFixedSource = await ApplyFirstCodeFixAsync<TAnalyzer, TCodeFixProvider>(source);
        Assert.Equal(NormalizeCode(expectedFixedSource), NormalizeCode(actualFixedSource));
    }

    private static async Task<string> ApplyFirstCodeFixAsync<TAnalyzer, TCodeFixProvider>(string source)
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFixProvider : CodeFixProvider, new()
    {
        using var workspace = new AdhocWorkspace();

        var projectId = ProjectId.CreateNewId();
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "AnalyzerVerifier", "AnalyzerVerifier", LanguageNames.CSharp)
            .AddMetadataReferences(projectId, GetMetadataReferences());

        var primaryDocumentId = DocumentId.CreateNewId(projectId);
        solution = solution.AddDocument(primaryDocumentId, "Test0.cs", SourceText.From(source));
        solution = solution.AddDocument(DocumentId.CreateNewId(projectId), "AxiomAssertionStubs.cs", SourceText.From(AxiomAssertionStubs.Source));
        solution = solution.AddDocument(DocumentId.CreateNewId(projectId), "XunitAssertStubs.cs", SourceText.From(XunitAssertStubs.Source));
        solution = solution.AddDocument(DocumentId.CreateNewId(projectId), "NunitAssertStubs.cs", SourceText.From(NunitAssertStubs.Source));

        var project = solution.GetProject(projectId)!
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithParseOptions(CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview));

        var document = project.GetDocument(primaryDocumentId)!;
        var compilation = await document.Project.GetCompilationAsync().ConfigureAwait(false);
        Assert.NotNull(compilation);

        var analyzer = new TAnalyzer();
        var diagnostics = await compilation!
            .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer))
            .GetAnalyzerDiagnosticsAsync()
            .ConfigureAwait(false);

        var primaryTree = await document.GetSyntaxTreeAsync().ConfigureAwait(false);
        // These helper tests feed in one source file and expect one diagnostic/fix target.
        var diagnostic = Assert.Single(diagnostics.Where(d => d.Location.SourceTree == primaryTree));

        var codeActions = new List<CodeAction>();
        var provider = new TCodeFixProvider();
        var context = new CodeFixContext(
            document,
            diagnostic,
            (action, _) => codeActions.Add(action),
            CancellationToken.None);

        await provider.RegisterCodeFixesAsync(context).ConfigureAwait(false);
        var codeAction = Assert.Single(codeActions);
        var operations = await codeAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var applyChanges = Assert.Single(operations.OfType<ApplyChangesOperation>());
        var changedDocument = applyChanges.ChangedSolution.GetDocument(primaryDocumentId)!;
        var changedText = await changedDocument.GetTextAsync().ConfigureAwait(false);

        return changedText.ToString();
    }

    private static string NormalizeCode(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        return syntaxTree.GetRoot().NormalizeWhitespace().ToFullString();
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        Assert.NotNull(trustedPlatformAssemblies);

        return trustedPlatformAssemblies!
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path));
    }
}
