using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
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
        test.FixedState.Sources.Add(AxiomAssertionStubs.Source);
        await test.RunAsync();
    }
}
