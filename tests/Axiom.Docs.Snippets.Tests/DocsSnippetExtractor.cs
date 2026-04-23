using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Docs.Snippets.Tests;

internal static class DocsSnippetExtractor
{
    // Markdown fences can opt into a specific verification setup when inference is not enough.
    private const string ContextKey = "axiom-context=";
    private const string HtmlCommentOpen = "<!--";
    private const string HtmlCommentClose = "-->";

    private static readonly Lazy<IReadOnlyList<DocsSnippet>> AllSnippetsCache = new(LoadAllSnippets);

    public static IReadOnlyList<DocsSnippet> AllSnippets => AllSnippetsCache.Value;

    public static IReadOnlyList<DocsSnippet> RelevantSnippets =>
        AllSnippets.Where(snippet => snippet.Shape is not DocsSnippetShape.SignatureCatalog).ToArray();

    private static IReadOnlyList<DocsSnippet> LoadAllSnippets()
    {
        var snippets = new List<DocsSnippet>();

        foreach (var filePath in Directory.EnumerateFiles(RepositoryPaths.DocsDirectory, "*.md", SearchOption.AllDirectories).OrderBy(path => path, StringComparer.Ordinal))
        {
            snippets.AddRange(ExtractFromFile(filePath));
        }

        return snippets;
    }

    private static IEnumerable<DocsSnippet> ExtractFromFile(string filePath)
    {
        var relativePath = Path.GetRelativePath(RepositoryPaths.Root, filePath).Replace('\\', '/');
        var lines = File.ReadAllLines(filePath);
        var insideFence = false;
        var infoString = string.Empty;
        var snippetStartLine = 0;
        var snippetLines = new List<string>();
        var snippetIndex = 0;
        string? pendingContextToken = null;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (!insideFence)
            {
                if (TryExtractContextToken(line, out var contextToken))
                {
                    pendingContextToken = contextToken;
                    continue;
                }

                if (!line.StartsWith("```", StringComparison.Ordinal))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        pendingContextToken = null;
                    }

                    continue;
                }

                infoString = line[3..].Trim();
                if (!IsCSharpFence(infoString))
                {
                    pendingContextToken = null;
                    continue;
                }

                if (pendingContextToken is not null)
                {
                    infoString = string.IsNullOrEmpty(infoString)
                        ? pendingContextToken
                        : $"{infoString} {pendingContextToken}";
                }

                insideFence = true;
                snippetStartLine = lineIndex + 2;
                snippetLines.Clear();
                pendingContextToken = null;
                continue;
            }

            if (line == "```")
            {
                snippetIndex++;
                var code = string.Join(Environment.NewLine, snippetLines);
                yield return CreateSnippet(relativePath, snippetIndex, snippetStartLine, infoString, code);
                insideFence = false;
                infoString = string.Empty;
                snippetLines.Clear();
                continue;
            }

            snippetLines.Add(line);
        }

        if (insideFence)
        {
            throw new InvalidOperationException($"Unterminated csharp fence in {relativePath}.");
        }
    }

    private static DocsSnippet CreateSnippet(string relativePath, int index, int startLine, string infoString, string code)
    {
        var trimmedCode = code.TrimEnd();
        var infoTokens = SplitInfoString(infoString);
        var context = ResolveContext(relativePath, trimmedCode, infoTokens);
        var needsXunit = NeedsXunitStubs(trimmedCode);
        var needsNunit = NeedsNunitStubs(trimmedCode);
        var needsMstest = NeedsMstestStubs(trimmedCode);
        var shape = ResolveShape(relativePath, index, trimmedCode, out var skipReason);

        return new DocsSnippet(
            relativePath,
            index,
            startLine,
            infoString,
            trimmedCode,
            context,
            shape,
            needsXunit,
            needsNunit,
            needsMstest,
            skipReason);
    }

    private static bool IsCSharpFence(string infoString)
    {
        // The docs verifier only compiles literal C# examples from markdown.
        var firstToken = infoString.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.Equals(firstToken, "csharp", StringComparison.OrdinalIgnoreCase);
    }

    private static DocsSnippetContext ResolveContext(string relativePath, string code, IReadOnlyList<string> infoTokens)
    {
        foreach (var token in infoTokens)
        {
            if (!token.StartsWith(ContextKey, StringComparison.Ordinal))
            {
                continue;
            }

            return token[ContextKey.Length..] switch
            {
                "general" => DocsSnippetContext.General,
                "vector" => DocsSnippetContext.Vector,
                // Migration galleries are statement collections, not one coherent program.
                "migration-gallery" => DocsSnippetContext.MigrationGallery,
                var unknown => throw new InvalidOperationException($"Unknown docs snippet context '{unknown}' in {relativePath}."),
            };
        }

        if (IsVectorSnippet(code))
        {
            return DocsSnippetContext.Vector;
        }

        return DocsSnippetContext.General;
    }

    private static DocsSnippetShape ResolveShape(string relativePath, int index, string code, out string? skipReason)
    {
        // Shape decides how the snippet gets wrapped before we compile it.
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview));
        var diagnostics = syntaxTree.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
        if (diagnostics.Length > 0)
        {
            if (LooksLikeSignatureCatalog(code))
            {
                // API catalogs are shown as C# for readability, but they are not meant to compile as-is.
                skipReason = "Non-literal API surface catalog";
                return DocsSnippetShape.SignatureCatalog;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Could not parse docs snippet {relativePath}#{index}.");
            foreach (var diagnostic in diagnostics)
            {
                messageBuilder.AppendLine(diagnostic.ToString());
            }

            throw new InvalidOperationException(messageBuilder.ToString());
        }

        var root = syntaxTree.GetCompilationUnitRoot();
        if (root.Members.OfType<GlobalStatementSyntax>().Any())
        {
            skipReason = null;
            return DocsSnippetShape.TopLevelStatements;
        }

        skipReason = null;
        return DocsSnippetShape.DeclarationOnly;
    }

    private static bool NeedsXunitStubs(string code)
    {
        return code.Contains("Xunit.Assert.", StringComparison.Ordinal)
            || code.Contains("using static Xunit.Assert", StringComparison.Ordinal)
            || code.Contains("Assert.Equal(", StringComparison.Ordinal)
            || code.Contains("Assert.NotEqual(", StringComparison.Ordinal)
            || code.Contains("Assert.Null(", StringComparison.Ordinal)
            || code.Contains("Assert.NotNull(", StringComparison.Ordinal)
            || code.Contains("Assert.True(", StringComparison.Ordinal)
            || code.Contains("Assert.False(", StringComparison.Ordinal)
            || code.Contains("Assert.Empty(", StringComparison.Ordinal)
            || code.Contains("Assert.NotEmpty(", StringComparison.Ordinal)
            || code.Contains("Assert.Contains(", StringComparison.Ordinal)
            || code.Contains("Assert.DoesNotContain(", StringComparison.Ordinal)
            || code.Contains("Assert.Single(", StringComparison.Ordinal)
            || code.Contains("Assert.Same(", StringComparison.Ordinal)
            || code.Contains("Assert.NotSame(", StringComparison.Ordinal)
            || code.Contains("Assert.Throws<", StringComparison.Ordinal)
            || code.Contains("Assert.StartsWith(", StringComparison.Ordinal)
            || code.Contains("Assert.EndsWith(", StringComparison.Ordinal)
            || code.Contains("Assert.IsType<", StringComparison.Ordinal)
            || code.Contains("Assert.IsAssignableFrom<", StringComparison.Ordinal);
    }

    private static bool NeedsNunitStubs(string code)
    {
        return code.Contains("NUnit.Framework.Assert.That", StringComparison.Ordinal)
            || code.Contains("using static NUnit.Framework.Assert", StringComparison.Ordinal)
            || code.Contains("Assert.That(", StringComparison.Ordinal)
            || code.Contains("Is.EqualTo(", StringComparison.Ordinal)
            || code.Contains("Is.Not.EqualTo(", StringComparison.Ordinal)
            || code.Contains("Is.Null", StringComparison.Ordinal)
            || code.Contains("Is.Not.Null", StringComparison.Ordinal)
            || code.Contains("Is.True", StringComparison.Ordinal)
            || code.Contains("Is.False", StringComparison.Ordinal)
            || code.Contains("Is.Empty", StringComparison.Ordinal)
            || code.Contains("Is.Not.Empty", StringComparison.Ordinal);
    }

    private static bool NeedsMstestStubs(string code)
    {
        return code.Contains("Microsoft.VisualStudio.TestTools.UnitTesting.Assert.", StringComparison.Ordinal)
            || code.Contains("using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert", StringComparison.Ordinal)
            || code.Contains("Assert.AreEqual(", StringComparison.Ordinal)
            || code.Contains("Assert.AreNotEqual(", StringComparison.Ordinal)
            || code.Contains("Assert.IsNull(", StringComparison.Ordinal)
            || code.Contains("Assert.IsNotNull(", StringComparison.Ordinal)
            || code.Contains("Assert.IsTrue(", StringComparison.Ordinal)
            || code.Contains("Assert.IsFalse(", StringComparison.Ordinal)
            || code.Contains("Assert.AreSame(", StringComparison.Ordinal)
            || code.Contains("Assert.AreNotSame(", StringComparison.Ordinal)
            || code.Contains("Assert.IsInstanceOfType(", StringComparison.Ordinal)
            || code.Contains("Assert.IsNotInstanceOfType(", StringComparison.Ordinal)
            || code.Contains("StringAssert.Contains(", StringComparison.Ordinal)
            || code.Contains("StringAssert.StartsWith(", StringComparison.Ordinal)
            || code.Contains("StringAssert.EndsWith(", StringComparison.Ordinal)
            || code.Contains("CollectionAssert.Contains(", StringComparison.Ordinal)
            || code.Contains("CollectionAssert.DoesNotContain(", StringComparison.Ordinal);
    }

    private static bool IsVectorSnippet(string code)
    {
        return code.Contains("using Axiom.Vectors", StringComparison.Ordinal)
            || code.Contains("RankingQuery<", StringComparison.Ordinal)
            || code.Contains("HaveDimension(", StringComparison.Ordinal)
            || code.Contains("NotContainNaNOrInfinity(", StringComparison.Ordinal)
            || code.Contains("BeApproximatelyEqualTo(", StringComparison.Ordinal)
            || code.Contains("HaveDotProductWith(", StringComparison.Ordinal)
            || code.Contains("HaveEuclideanDistanceTo(", StringComparison.Ordinal)
            || code.Contains("HaveCosineSimilarityWith(", StringComparison.Ordinal)
            || code.Contains("HaveCosineSimilarityTo(", StringComparison.Ordinal)
            || code.Contains("ActualSimilarity", StringComparison.Ordinal)
            || code.Contains("BeNormalized(", StringComparison.Ordinal)
            || code.Contains("BeZeroVector(", StringComparison.Ordinal)
            || code.Contains("NotBeZeroVector(", StringComparison.Ordinal)
            || code.Contains("ContainInTopK(", StringComparison.Ordinal)
            || code.Contains("HaveRank(", StringComparison.Ordinal)
            || code.Contains("HaveRecallAt(", StringComparison.Ordinal)
            || code.Contains("HavePrecisionAt(", StringComparison.Ordinal)
            || code.Contains("HaveMeanReciprocalRank(", StringComparison.Ordinal)
            || code.Contains("HaveReciprocalRank(", StringComparison.Ordinal)
            || code.Contains("HaveHitRateAt(", StringComparison.Ordinal);
    }

    private static bool LooksLikeSignatureCatalog(string code)
    {
        // These blocks list member signatures for reference and intentionally omit method bodies.
        var lines = code
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToArray();

        if (lines.Length == 0)
        {
            return false;
        }

        foreach (var line in lines)
        {
            if (line.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.Contains(';') || line.Contains('{') || line.Contains('}'))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<string> SplitInfoString(string infoString)
    {
        return infoString
            .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .ToArray();
    }

    private static bool TryExtractContextToken(string line, out string? contextToken)
    {
        var trimmed = line.Trim();
        if (!trimmed.StartsWith(HtmlCommentOpen, StringComparison.Ordinal) || !trimmed.EndsWith(HtmlCommentClose, StringComparison.Ordinal))
        {
            contextToken = null;
            return false;
        }

        var content = trimmed[HtmlCommentOpen.Length..^HtmlCommentClose.Length].Trim();
        if (!content.StartsWith(ContextKey, StringComparison.Ordinal))
        {
            contextToken = null;
            return false;
        }

        contextToken = content;
        return true;
    }
}
