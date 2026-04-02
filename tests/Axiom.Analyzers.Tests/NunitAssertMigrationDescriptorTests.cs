using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationDescriptorTests
{
    [Fact]
    public void DiagnosticDescriptors_AreStable()
    {
        var analyzer = new NunitAssertMigrationAnalyzer();
        var diagnostics = analyzer.SupportedDiagnostics.OrderBy(static rule => rule.Id).ToArray();

        Assert.Collection(
            diagnostics,
            rule =>
            {
                Assert.Equal("AXM1024", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That equal constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.EqualTo(expected)) can be migrated to 'actual.Should().Be(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1025", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-equal constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(actual, Is.Not.EqualTo(expected)) can be migrated to 'actual.Should().NotBe(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1026", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That null constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(value, Is.Null) can be migrated to 'value.Should().BeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1027", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-null constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(value, Is.Not.Null) can be migrated to 'value.Should().NotBeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1028", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That true constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(condition, Is.True) can be migrated to 'condition.Should().BeTrue()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1029", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That false constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(condition, Is.False) can be migrated to 'condition.Should().BeFalse()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1030", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That empty constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Is.Empty) can be migrated to 'collection.Should().BeEmpty()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1031", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate NUnit Assert.That not-empty constraint to Axiom", rule.Title.ToString());
                Assert.Equal("NUnit Assert.That(collection, Is.Not.Empty) can be migrated to 'collection.Should().NotBeEmpty()'", rule.MessageFormat.ToString());
            });
    }
}
