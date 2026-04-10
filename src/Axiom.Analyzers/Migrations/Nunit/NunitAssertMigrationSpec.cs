using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.NunitMigration;

internal enum NunitAssertMigrationKind
{
    Be,
    NotBe,
    BeNull,
    NotBeNull,
    BeTrue,
    BeFalse,
    BeEmpty,
    NotBeEmpty,
    ContainSubstring,
    NotContainSubstring,
    StartWith,
    EndWith,
    HaveCount,
    BeSameAs,
    NotBeSameAs,
}

internal sealed class NunitAssertMigrationSpec
{
    public NunitAssertMigrationSpec(
        string diagnosticId,
        NunitAssertMigrationKind kind,
        string title,
        string message,
        string codeFixTitle)
    {
        DiagnosticId = diagnosticId;
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
            description: "Suggests a high-confidence migration from a supported NUnit Assert.That(...) constraint to the equivalent Axiom Should() assertion.");
    }

    public string DiagnosticId { get; }
    public NunitAssertMigrationKind Kind { get; }
    public string Title { get; }
    public string Message { get; }
    public string CodeFixTitle { get; }
    public DiagnosticDescriptor Rule { get; }
}

internal static class NunitAssertMigrationSpecs
{
    public static ImmutableArray<NunitAssertMigrationSpec> All { get; } =
    [
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEqualTo,
            NunitAssertMigrationKind.Be,
            "Migrate NUnit Assert.That equal constraint to Axiom",
            "NUnit Assert.That(actual, Is.EqualTo(expected)) can be migrated to 'actual.Should().Be(expected)'",
            "Convert to 'actual.Should().Be(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotEqualTo,
            NunitAssertMigrationKind.NotBe,
            "Migrate NUnit Assert.That not-equal constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.EqualTo(expected)) can be migrated to 'actual.Should().NotBe(expected)'",
            "Convert to 'actual.Should().NotBe(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNull,
            NunitAssertMigrationKind.BeNull,
            "Migrate NUnit Assert.That null constraint to Axiom",
            "NUnit Assert.That(value, Is.Null) can be migrated to 'value.Should().BeNull()'",
            "Convert to 'value.Should().BeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotNull,
            NunitAssertMigrationKind.NotBeNull,
            "Migrate NUnit Assert.That not-null constraint to Axiom",
            "NUnit Assert.That(value, Is.Not.Null) can be migrated to 'value.Should().NotBeNull()'",
            "Convert to 'value.Should().NotBeNull()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatTrue,
            NunitAssertMigrationKind.BeTrue,
            "Migrate NUnit Assert.That true constraint to Axiom",
            "NUnit Assert.That(condition, Is.True) can be migrated to 'condition.Should().BeTrue()'",
            "Convert to 'condition.Should().BeTrue()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatFalse,
            NunitAssertMigrationKind.BeFalse,
            "Migrate NUnit Assert.That false constraint to Axiom",
            "NUnit Assert.That(condition, Is.False) can be migrated to 'condition.Should().BeFalse()'",
            "Convert to 'condition.Should().BeFalse()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEmpty,
            NunitAssertMigrationKind.BeEmpty,
            "Migrate NUnit Assert.That empty constraint to Axiom",
            "NUnit Assert.That(collection, Is.Empty) can be migrated to 'collection.Should().BeEmpty()'",
            "Convert to 'collection.Should().BeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotEmpty,
            NunitAssertMigrationKind.NotBeEmpty,
            "Migrate NUnit Assert.That not-empty constraint to Axiom",
            "NUnit Assert.That(collection, Is.Not.Empty) can be migrated to 'collection.Should().NotBeEmpty()'",
            "Convert to 'collection.Should().NotBeEmpty()'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatContainsSubstring,
            NunitAssertMigrationKind.ContainSubstring,
            "Migrate NUnit Assert.That Does.Contain string constraint to Axiom",
            "NUnit Assert.That(actual, Does.Contain(expectedSubstring)) can be migrated to 'actual.Should().Contain(expectedSubstring)'",
            "Convert to 'actual.Should().Contain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatDoesNotContainSubstring,
            NunitAssertMigrationKind.NotContainSubstring,
            "Migrate NUnit Assert.That Does.Not.Contain string constraint to Axiom",
            "NUnit Assert.That(actual, Does.Not.Contain(expectedSubstring)) can be migrated to 'actual.Should().NotContain(expectedSubstring)'",
            "Convert to 'actual.Should().NotContain(expectedSubstring)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatStartsWith,
            NunitAssertMigrationKind.StartWith,
            "Migrate NUnit Assert.That Does.StartWith constraint to Axiom",
            "NUnit Assert.That(actual, Does.StartWith(expectedPrefix)) can be migrated to 'actual.Should().StartWith(expectedPrefix)'",
            "Convert to 'actual.Should().StartWith(expectedPrefix)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatEndsWith,
            NunitAssertMigrationKind.EndWith,
            "Migrate NUnit Assert.That Does.EndWith constraint to Axiom",
            "NUnit Assert.That(actual, Does.EndWith(expectedSuffix)) can be migrated to 'actual.Should().EndWith(expectedSuffix)'",
            "Convert to 'actual.Should().EndWith(expectedSuffix)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatHasCount,
            NunitAssertMigrationKind.HaveCount,
            "Migrate NUnit Assert.That Has.Count.EqualTo constraint to Axiom",
            "NUnit Assert.That(collection, Has.Count.EqualTo(expectedCount)) can be migrated to 'collection.Should().HaveCount(expectedCount)'",
            "Convert to 'collection.Should().HaveCount(expectedCount)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatSameAs,
            NunitAssertMigrationKind.BeSameAs,
            "Migrate NUnit Assert.That Is.SameAs constraint to Axiom",
            "NUnit Assert.That(actual, Is.SameAs(expected)) can be migrated to 'actual.Should().BeSameAs(expected)'",
            "Convert to 'actual.Should().BeSameAs(expected)'"),
        new(
            AxiomAnalyzerIds.MigrateNunitAssertThatNotSameAs,
            NunitAssertMigrationKind.NotBeSameAs,
            "Migrate NUnit Assert.That Is.Not.SameAs constraint to Axiom",
            "NUnit Assert.That(actual, Is.Not.SameAs(expected)) can be migrated to 'actual.Should().NotBeSameAs(expected)'",
            "Convert to 'actual.Should().NotBeSameAs(expected)'")
    ];

    public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        All.Select(static spec => spec.Rule).ToImmutableArray();

    public static bool TryGetByDiagnosticId(string diagnosticId, out NunitAssertMigrationSpec spec)
    {
        foreach (var candidate in All)
        {
            if (candidate.DiagnosticId == diagnosticId)
            {
                spec = candidate;
                return true;
            }
        }

        spec = null!;
        return false;
    }
}
