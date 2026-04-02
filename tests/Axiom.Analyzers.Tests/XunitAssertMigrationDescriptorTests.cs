using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationDescriptorTests
{
    [Fact]
    public void DiagnosticDescriptors_AreStable()
    {
        var analyzer = new XunitAssertMigrationAnalyzer();
        var diagnostics = analyzer.SupportedDiagnostics.OrderBy(static rule => rule.Id).ToArray();

        Assert.Collection(
            diagnostics,
            rule =>
            {
                Assert.Equal("AXM1001", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Equal to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Equal(...) can be migrated to 'actual.Should().Be(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1002", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.NotEqual to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.NotEqual(...) can be migrated to 'actual.Should().NotBe(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1003", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Null to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Null(...) can be migrated to 'value.Should().BeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1004", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.NotNull to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.NotNull(...) can be migrated to 'value.Should().NotBeNull()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1005", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.True to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.True(...) can be migrated to 'condition.Should().BeTrue()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1006", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.False to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.False(...) can be migrated to 'condition.Should().BeFalse()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1007", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Empty to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Empty(...) can be migrated to 'subject.Should().BeEmpty()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1008", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.NotEmpty to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.NotEmpty(...) can be migrated to 'subject.Should().NotBeEmpty()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1009", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Contains to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Contains(...) can be migrated to 'collection.Should().Contain(item)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1010", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.DoesNotContain to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.DoesNotContain(...) can be migrated to 'collection.Should().NotContain(item)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1011", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Single to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Single(...) can be migrated to 'subject.Should().ContainSingle()' and append '.SingleItem' when the single item is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1012", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Same to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Same(...) can be migrated to 'actual.Should().BeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1013", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.NotSame to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.NotSame(...) can be migrated to 'actual.Should().NotBeSameAs(expected)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1014", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Throws to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Throws<TException>(...) can be migrated to '.Should().Throw<TException>()', chaining '.WithParamName(...)' for non-null constant param-name overloads and appending '.Thrown' when the exception is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1015", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.IsType to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.IsType<T>(...) can be migrated to 'value.Should().BeOfType<T>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1016", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.IsAssignableFrom to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.IsAssignableFrom<T>(...) can be migrated to 'value.Should().BeAssignableTo<T>()'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1017", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Contains string overload to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Contains(expectedSubstring, actualString) can be migrated to 'actualString.Should().Contain(expectedSubstring)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1018", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.DoesNotContain string overload to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.DoesNotContain(expectedSubstring, actualString) can be migrated to 'actualString.Should().NotContain(expectedSubstring)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1019", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Single predicate overload to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Single(collection, predicate) can be migrated to Axiom 'collection.Should().ContainSingle(...)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1020", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.Contains dictionary overload to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.Contains(key, dictionary) can be migrated to 'dictionary.Should().ContainKey(key)' and append '.WhoseValue' when the associated value is used", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1021", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.DoesNotContain dictionary overload to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.DoesNotContain(key, dictionary) can be migrated to 'dictionary.Should().NotContainKey(key)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1022", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.StartsWith to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.StartsWith(expectedPrefix, actualString) can be migrated to 'actualString.Should().StartWith(expectedPrefix)'", rule.MessageFormat.ToString());
            },
            rule =>
            {
                Assert.Equal("AXM1023", rule.Id);
                Assert.Equal("Migration", rule.Category);
                Assert.Equal(DiagnosticSeverity.Info, rule.DefaultSeverity);
                Assert.Equal("Migrate xUnit Assert.EndsWith to Axiom", rule.Title.ToString());
                Assert.Equal("xUnit Assert.EndsWith(expectedSuffix, actualString) can be migrated to 'actualString.Should().EndWith(expectedSuffix)'", rule.MessageFormat.ToString());
            });
    }

}
