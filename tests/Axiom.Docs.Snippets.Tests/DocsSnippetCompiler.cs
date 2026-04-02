using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Axiom.Docs.Snippets.Tests;

internal sealed class DocsSnippetCompiler
{
    private readonly ImmutableArray<MetadataReference> _references;
    private readonly ImmutableArray<SyntaxTree> _generalSupportTrees;
    private readonly ImmutableArray<SyntaxTree> _migrationStubTrees;

    public DocsSnippetCompiler()
    {
        _references = BuildReferences();
        _generalSupportTrees = LoadSupportTrees("DocsSnippetGeneralSupport.cs");
        _migrationStubTrees = LoadSupportTrees("DocsSnippetMigrationStubs.cs");
    }

    public DocsSnippetCompileResult Compile(DocsSnippet snippet)
    {
        var failures = ExpandCompileUnits(snippet)
            .Select(unit => CompileSingle(unit.Snippet, unit.Label))
            .Where(result => !result.Success)
            .ToArray();

        if (failures.Length == 0)
        {
            return DocsSnippetCompileResult.Passed(snippet);
        }

        var message = new StringBuilder();
        foreach (var failure in failures)
        {
            message.AppendLine(failure.FailureMessage);
        }

        return DocsSnippetCompileResult.Failure(snippet, message.ToString().TrimEnd());
    }

    private DocsSnippetCompileResult CompileSingle(DocsSnippet snippet, string? label)
    {
        var syntaxTrees = new List<SyntaxTree>();
        // Shared fixture code gives docs examples a stable compile context without copying them out of markdown.
        syntaxTrees.AddRange(_generalSupportTrees);
        if (snippet.NeedsXunit || snippet.NeedsNunit || snippet.NeedsMstest)
        {
            // Migration docs compile against lightweight framework stubs, not the real test frameworks.
            syntaxTrees.AddRange(_migrationStubTrees);
        }

        syntaxTrees.AddRange(GetRelatedDeclarationTrees(snippet));
        var snippetTree = BuildSnippetTree(snippet);
        syntaxTrees.Add(snippetTree);

        var compilation = CSharpCompilation.Create(
            assemblyName: SanitizeAssemblyName(snippet.DisplayName + label),
            syntaxTrees: syntaxTrees,
            references: _references,
            options: new CSharpCompilationOptions(
                snippet.Shape is DocsSnippetShape.TopLevelStatements
                    ? OutputKind.ConsoleApplication
                    : OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Enable,
                concurrentBuild: false));

        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        if (emitResult.Success)
        {
            return DocsSnippetCompileResult.Passed(snippet);
        }

        var message = new StringBuilder();
        message.AppendLine($"Docs snippet failed to compile: {snippet.DisplayName}{label}");
        message.AppendLine($"Context: {snippet.Context}");
        foreach (var diagnostic in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).OrderBy(d => d.Location.GetLineSpan().Path, StringComparer.Ordinal).ThenBy(d => d.Location.GetLineSpan().StartLinePosition.Line))
        {
            message.AppendLine(diagnostic.ToString());
        }

        message.AppendLine("Generated verification source:");
        message.AppendLine(snippetTree.ToString());

        return DocsSnippetCompileResult.Failure(snippet, message.ToString().TrimEnd());
    }

    private static string SanitizeAssemblyName(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            builder.Append(char.IsLetterOrDigit(character) ? character : '_');
        }

        return builder.ToString();
    }

    private static IEnumerable<(DocsSnippet Snippet, string? Label)> ExpandCompileUnits(DocsSnippet snippet)
    {
        if (snippet.Context is not DocsSnippetContext.MigrationGallery || snippet.Shape is not DocsSnippetShape.TopLevelStatements)
        {
            yield return (snippet, null);
            yield break;
        }

        // Gallery blocks are independent before/after statements, so compile them one at a time.
        var syntaxTree = CSharpSyntaxTree.ParseText(snippet.Code, new CSharpParseOptions(LanguageVersion.Preview));
        var root = syntaxTree.GetCompilationUnitRoot();
        var partIndex = 0;
        foreach (var statement in root.Members.OfType<GlobalStatementSyntax>())
        {
            partIndex++;
            var span = statement.GetLocation().GetLineSpan();
            var statementStartLine = span.StartLinePosition.Line + snippet.StartLine;
            yield return (
                snippet with
                {
                    Code = statement.ToFullString().Trim(),
                    StartLine = statementStartLine,
                },
                $" (statement {partIndex})");
        }
    }

    private SyntaxTree BuildSnippetTree(DocsSnippet snippet)
    {
        var source = snippet.Shape switch
        {
            DocsSnippetShape.TopLevelStatements => RenderTopLevelSnippet(snippet),
            DocsSnippetShape.DeclarationOnly => RenderDeclarationSnippet(snippet),
            _ => throw new InvalidOperationException($"Cannot compile skipped snippet {snippet.DisplayName}."),
        };

        return CSharpSyntaxTree.ParseText(
            source,
            new CSharpParseOptions(LanguageVersion.Preview),
            path: snippet.DisplayName + ".g.cs");
    }

    private string RenderTopLevelSnippet(DocsSnippet snippet)
    {
        var codeLines = snippet.Code.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var usingLines = new List<string>();
        var bodyLines = new List<string>();
        var seenBody = false;
        var bodyStartOffset = 0;

        foreach (var line in codeLines)
        {
            if (!seenBody && line.TrimStart().StartsWith("using ", StringComparison.Ordinal))
            {
                usingLines.Add(line);
                bodyStartOffset++;
                continue;
            }

            seenBody = true;
            bodyLines.Add(line);
        }

        var bodyText = string.Join(Environment.NewLine, bodyLines);
        var bodyStartLine = snippet.StartLine + bodyStartOffset;
        var declaredNames = CollectDeclaredNames(snippet.Code);

        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Collections.Concurrent;");
        builder.AppendLine("using System.Collections.Immutable;");
        builder.AppendLine("using System.Collections.ObjectModel;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using Axiom.Assertions;");
        builder.AppendLine("using Axiom.Assertions.AssertionTypes;");
        builder.AppendLine("using Axiom.Assertions.Authoring;");
        builder.AppendLine("using Axiom.Assertions.Chaining;");
        builder.AppendLine("using Axiom.Assertions.Configuration;");
        builder.AppendLine("using Axiom.Assertions.Equivalency;");
        builder.AppendLine("using Axiom.Assertions.Extensions;");

        if (snippet.Context is DocsSnippetContext.Vector)
        {
            builder.AppendLine("using Axiom.Vectors;");
        }

        if (NeedsAxiomCoreUsing(snippet.Code))
        {
            builder.AppendLine("using Axiom.Core;");
        }

        if (snippet.NeedsXunit)
        {
            builder.AppendLine("using Xunit;");
        }

        if (snippet.NeedsNunit)
        {
            builder.AppendLine("using NUnit.Framework;");
        }

        if (snippet.NeedsMstest)
        {
            builder.AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;");
        }

        foreach (var usingLine in usingLines)
        {
            builder.AppendLine(usingLine);
        }

        foreach (var preludeLine in GetPreludeLines(snippet.Context, snippet.Code, declaredNames))
        {
            builder.AppendLine(preludeLine);
        }

        // Map compiler errors back to the markdown file and line instead of the generated wrapper.
        builder.AppendLine($"#line {bodyStartLine} \"{snippet.RelativePath.Replace("\\", "/", StringComparison.Ordinal)}\"");
        builder.AppendLine(bodyText);
        builder.AppendLine("#line default");

        return builder.ToString();
    }

    private static string RenderDeclarationSnippet(DocsSnippet snippet)
    {
        var builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Collections.Concurrent;");
        builder.AppendLine("using System.Collections.Immutable;");
        builder.AppendLine("using System.Collections.ObjectModel;");
        builder.AppendLine("using System.Runtime.CompilerServices;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using Axiom.Assertions;");
        builder.AppendLine("using Axiom.Assertions.AssertionTypes;");
        builder.AppendLine("using Axiom.Assertions.Authoring;");
        builder.AppendLine("using Axiom.Assertions.Chaining;");
        builder.AppendLine("using Axiom.Assertions.Configuration;");
        builder.AppendLine("using Axiom.Assertions.Equivalency;");
        builder.AppendLine("using Axiom.Assertions.Extensions;");
        builder.AppendLine("using Axiom.Core;");
        builder.AppendLine("using Axiom.Core.Failures;");
        builder.AppendLine("using Axiom.Vectors;");
        builder.AppendLine($"#line {snippet.StartLine} \"{snippet.RelativePath.Replace("\\", "/", StringComparison.Ordinal)}\"");
        builder.AppendLine(snippet.Code);
        builder.AppendLine("#line default");
        return builder.ToString();
    }

    private static ImmutableHashSet<string> CollectDeclaredNames(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview));
        var root = syntaxTree.GetCompilationUnitRoot();
        var names = root.DescendantNodes()
            .OfType<VariableDeclaratorSyntax>()
            .Select(declarator => declarator.Identifier.ValueText)
            .Concat(root.DescendantNodes().OfType<LocalFunctionStatementSyntax>().Select(localFunction => localFunction.Identifier.ValueText))
            .ToImmutableHashSet(StringComparer.Ordinal);

        return names;
    }

    private static bool NeedsAxiomCoreUsing(string code)
    {
        return code.Contains("Assert.Batch(", StringComparison.Ordinal)
            || code.Contains("Axiom.Core.Assert.", StringComparison.Ordinal);
    }

    private IEnumerable<SyntaxTree> GetRelatedDeclarationTrees(DocsSnippet snippet)
    {
        if (snippet.Shape is not DocsSnippetShape.TopLevelStatements)
        {
            return [];
        }

        // Some docs pages show declarations first and usage later; pull in only the declarations the snippet calls.
        var invokedMembers = CollectInvokedMemberNames(snippet.Code);
        if (invokedMembers.Count == 0)
        {
            return [];
        }

        var relatedSnippets = DocsSnippetExtractor.AllSnippets
            .Where(candidate =>
                candidate.RelativePath == snippet.RelativePath &&
                candidate.Shape is DocsSnippetShape.DeclarationOnly &&
                candidate.Index != snippet.Index &&
                DeclaresAnyInvokedMember(candidate.Code, invokedMembers))
            .Select(candidate => BuildSnippetTree(candidate))
            .ToArray();

        return relatedSnippets;
    }

    private static ImmutableHashSet<string> CollectInvokedMemberNames(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview));
        var root = syntaxTree.GetCompilationUnitRoot();
        return root.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(invocation => invocation.Expression)
            .OfType<MemberAccessExpressionSyntax>()
            .Select(access => access.Name.Identifier.ValueText)
            .ToImmutableHashSet(StringComparer.Ordinal);
    }

    private static bool DeclaresAnyInvokedMember(string code, ImmutableHashSet<string> invokedMembers)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code, new CSharpParseOptions(LanguageVersion.Preview));
        var root = syntaxTree.GetCompilationUnitRoot();
        return root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Any(method => invokedMembers.Contains(method.Identifier.ValueText));
    }

    private static IEnumerable<string> GetPreludeLines(DocsSnippetContext context, string code, ImmutableHashSet<string> declaredNames)
    {
        // Each docs area gets only the locals it needs to keep examples short and readable.
        var lines = context switch
        {
            DocsSnippetContext.General => GeneralPrelude,
            DocsSnippetContext.Vector => VectorPrelude,
            DocsSnippetContext.MigrationGallery => GetMigrationPrelude(code),
            _ => Array.Empty<string>(),
        };

        foreach (var line in lines)
        {
            var name = line.Split('=', 2)[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.TrimEnd(';');
            if (name is not null && declaredNames.Contains(name))
            {
                continue;
            }

            yield return line;
        }
    }

    private static ImmutableArray<MetadataReference> BuildReferences()
    {
        var references = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);
        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            ?? throw new InvalidOperationException("Trusted platform assemblies were not available.");

        foreach (var assemblyPath in trustedAssemblies.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            references[assemblyPath] = MetadataReference.CreateFromFile(assemblyPath);
        }

        AddReference(references, typeof(object).Assembly.Location);
        AddReference(references, typeof(Axiom.Core.Assert).Assembly.Location);
        AddReference(references, typeof(Axiom.Assertions.ShouldExtensions).Assembly.Location);
        AddReference(references, typeof(Axiom.Assertions.AssertionTypes.ValueAssertions<>).Assembly.Location);
        AddReference(references, typeof(Axiom.Vectors.VectorAssertions<float>).Assembly.Location);
        AddReference(references, typeof(System.Collections.Immutable.ImmutableArray).Assembly.Location);

        return references.Values.ToImmutableArray();
    }

    private static void AddReference(IDictionary<string, MetadataReference> references, string assemblyPath)
    {
        references[assemblyPath] = MetadataReference.CreateFromFile(assemblyPath);
    }

    private static ImmutableArray<SyntaxTree> LoadSupportTrees(string fileName)
    {
        var supportPath = Path.Combine(RepositoryPaths.Root, "tests", "Axiom.Docs.Snippets.Tests", "Fixtures", fileName);
        var source = File.ReadAllText(supportPath);
        return ImmutableArray.Create(
            CSharpSyntaxTree.ParseText(
                source,
                new CSharpParseOptions(LanguageVersion.Preview),
                path: supportPath));
    }

    private static readonly string[] GeneralPrelude =
    [
        "var actual = DocsSnippetFixtures.ActualUserProfile;",
        "var expected = DocsSnippetFixtures.ExpectedUserProfile;",
        "var value = DocsSnippetFixtures.Value;",
        "var condition = true;",
        "var user = DocsSnippetFixtures.User;",
        "var invoice = DocsSnippetFixtures.Invoice;",
        "var today = DocsSnippetFixtures.Today;",
        "var response = DocsSnippetFixtures.Response;",
        "var failedResponse = DocsSnippetFixtures.FailedResponse;",
        "var order = DocsSnippetFixtures.Order;",
        "var status = OrderStatus.Submitted;",
        "Task loader = Task.CompletedTask;",
        "var rollout = Task.FromResult(\"pricing-api\");",
        "var orders = DocsSnippetFixtures.GetOrders();",
        "var users = DocsSnippetFixtures.GetUsers();",
        "var statuses = DocsSnippetFixtures.GetStatuses();",
        "var events = DocsSnippetFixtures.GetEvents();",
        "var stepIds = DocsSnippetFixtures.StepIds;",
        "var values = DocsSnippetFixtures.TextValues;",
    ];

    private static readonly string[] VectorPrelude =
    [
        "var embedding = DocsSnippetFixtures.Embedding;",
        "var expected = DocsSnippetFixtures.ExpectedEmbedding;",
        "var unrelated = DocsSnippetFixtures.UnrelatedEmbedding;",
    ];

    private static string[] GetMigrationPrelude(string code)
    {
        // Migration examples mix strings, collections, dictionaries, and framework calls in one gallery.
        var valuesLine = ((code.Contains("ContainSingle(", StringComparison.Ordinal) || code.Contains("Assert.Single(", StringComparison.Ordinal)) && code.Contains("> 0", StringComparison.Ordinal))
            || code.Contains("IsPositive", StringComparison.Ordinal)
            ? "var values = DocsSnippetFixtures.IntegerValues;"
            : "var values = DocsSnippetFixtures.TextValues;";

        var lines = new List<string>();

        if (code.Contains("actual", StringComparison.Ordinal))
        {
            lines.Add("var actual = \"prefix-sub-suffix\";");
        }

        if (code.Contains("expected", StringComparison.Ordinal))
        {
            lines.Add("var expected = \"expected\";");
        }

        if (code.Contains("Assert.That(value", StringComparison.Ordinal) ||
            code.Contains("Assert.IsNull(value", StringComparison.Ordinal) ||
            code.Contains("Assert.IsNotNull(value", StringComparison.Ordinal) ||
            code.Contains("value.Should()", StringComparison.Ordinal))
        {
            lines.Add("var value = new object();");
        }

        if (code.Contains("condition", StringComparison.Ordinal))
        {
            lines.Add("var condition = true;");
        }

        if (code.Contains("values", StringComparison.Ordinal))
        {
            lines.Add(valuesLine);
        }

        if (code.Contains("lookup", StringComparison.Ordinal))
        {
            lines.Add("var lookup = DocsSnippetFixtures.StringLookup;");
        }

        if (code.Contains("key", StringComparison.Ordinal))
        {
            lines.Add("var key = 1;");
        }

        if (code.Contains("work()", StringComparison.Ordinal))
        {
            lines.Add("void work() => throw new InvalidOperationException();");
        }

        if (code.Contains("IsPositive", StringComparison.Ordinal))
        {
            lines.Add("static bool IsPositive(int value) => value > 0;");
        }

        if (code.Contains("Use(", StringComparison.Ordinal))
        {
            lines.Add("static void Use<T>(T value) { }");
        }

        return [.. lines];
    }
}

internal sealed record DocsSnippetCompileResult(DocsSnippet Snippet, bool Success, string? FailureMessage)
{
    public static DocsSnippetCompileResult Passed(DocsSnippet snippet) => new(snippet, true, null);

    public static DocsSnippetCompileResult Failure(DocsSnippet snippet, string failureMessage) => new(snippet, false, failureMessage);
}
