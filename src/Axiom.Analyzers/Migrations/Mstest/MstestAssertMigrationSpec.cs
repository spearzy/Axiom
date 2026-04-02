using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.MstestMigration;

internal enum MstestAssertMigrationKind
{
    Be,
    NotBe,
    BeNull,
    NotBeNull,
    BeTrue,
    BeFalse,
    BeSameAs,
    NotBeSameAs,
}

internal sealed class MstestAssertMigrationSpec
{
    public MstestAssertMigrationSpec(
        string diagnosticId,
        string methodName,
        int requiredArgumentCount,
        MstestAssertMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
        MethodName = methodName;
        RequiredArgumentCount = requiredArgumentCount;
        Kind = kind;
        Title = title;
        Message = message;
        CodeFixTitle = codeFixTitle;
        Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            Message,
            "Migration",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: "Suggests a high-confidence migration from a supported MSTest Assert.* call to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public string MethodName { get; }
    public int RequiredArgumentCount { get; }
    public MstestAssertMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }
    public DiagnosticDescriptor Rule { get; }
}

internal static class MstestAssertMigrationSpecs
{
    public static ImmutableArray<MstestAssertMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreEqual,
            "AreEqual",
            2,
            MstestAssertMigrationKind.Be,
            "Migrate MSTest Assert.AreEqual to Axiom",
            "MSTest Assert.AreEqual(expected, actual) can be migrated to 'actual.Should().Be(expected)'",
            "Convert to 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreNotEqual,
            "AreNotEqual",
            2,
            MstestAssertMigrationKind.NotBe,
            "Migrate MSTest Assert.AreNotEqual to Axiom",
            "MSTest Assert.AreNotEqual(expected, actual) can be migrated to 'actual.Should().NotBe(expected)'",
            "Convert to 'actual.Should().NotBe(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsNull,
            "IsNull",
            1,
            MstestAssertMigrationKind.BeNull,
            "Migrate MSTest Assert.IsNull to Axiom",
            "MSTest Assert.IsNull(value) can be migrated to 'value.Should().BeNull()'",
            "Convert to 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsNotNull,
            "IsNotNull",
            1,
            MstestAssertMigrationKind.NotBeNull,
            "Migrate MSTest Assert.IsNotNull to Axiom",
            "MSTest Assert.IsNotNull(value) can be migrated to 'value.Should().NotBeNull()'",
            "Convert to 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsTrue,
            "IsTrue",
            1,
            MstestAssertMigrationKind.BeTrue,
            "Migrate MSTest Assert.IsTrue to Axiom",
            "MSTest Assert.IsTrue(condition) can be migrated to 'condition.Should().BeTrue()'",
            "Convert to 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertIsFalse,
            "IsFalse",
            1,
            MstestAssertMigrationKind.BeFalse,
            "Migrate MSTest Assert.IsFalse to Axiom",
            "MSTest Assert.IsFalse(condition) can be migrated to 'condition.Should().BeFalse()'",
            "Convert to 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreSame,
            "AreSame",
            2,
            MstestAssertMigrationKind.BeSameAs,
            "Migrate MSTest Assert.AreSame to Axiom",
            "MSTest Assert.AreSame(expected, actual) can be migrated to 'actual.Should().BeSameAs(expected)'",
            "Convert to 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateMstestAssertAreNotSame,
            "AreNotSame",
            2,
            MstestAssertMigrationKind.NotBeSameAs,
            "Migrate MSTest Assert.AreNotSame to Axiom",
            "MSTest Assert.AreNotSame(expected, actual) can be migrated to 'actual.Should().NotBeSameAs(expected)'",
            "Convert to 'actual.Should().NotBeSameAs(expected)'")
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static IEnumerable<MstestAssertMigrationSpec> GetByMethodName(string methodName)
        => All.Where(spec => spec.MethodName == methodName);
}
