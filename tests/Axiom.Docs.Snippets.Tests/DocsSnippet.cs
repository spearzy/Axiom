namespace Axiom.Docs.Snippets.Tests;

public enum DocsSnippetContext
{
    General,
    Vector,
    MigrationGallery,
}

public enum DocsSnippetShape
{
    TopLevelStatements,
    DeclarationOnly,
    SignatureCatalog,
}

public sealed record DocsSnippet(
    string RelativePath,
    int Index,
    int StartLine,
    string InfoString,
    string Code,
    DocsSnippetContext Context,
    DocsSnippetShape Shape,
    bool NeedsXunit,
    bool NeedsNunit,
    bool NeedsMstest,
    string? SkipReason)
{
    public string DisplayName => $"{RelativePath}#snippet-{Index}";
}
