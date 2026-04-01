using System.Linq;
using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationAnalyzerTests
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
            });
    }

    [Fact]
    public async Task AssertEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    {|AXM1001:Assert.Equal(expected, actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    actual.Should().Be(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertNotEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    {|AXM1002:Assert.NotEqual(expected, actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    actual.Should().NotBe(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StringAssertEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string expected, string actual)
                {
                    {|AXM1001:Assert.Equal(expected, actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string expected, string actual)
                {
                    actual.Should().Be(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertNull_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object? value)
                {
                    {|AXM1003:Assert.Null(value)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object? value)
                {
                    value.Should().BeNull();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StringAssertNull_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string? value)
                {
                    {|AXM1003:Assert.Null(value)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string? value)
                {
                    value.Should().BeNull();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertNotNull_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object? value)
                {
                    {|AXM1004:Assert.NotNull(value)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object? value)
                {
                    value.Should().NotBeNull();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StringAssertNotNull_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string? value)
                {
                    {|AXM1004:Assert.NotNull(value)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string? value)
                {
                    value.Should().NotBeNull();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertTrue_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    {|AXM1005:Assert.True(condition)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    condition.Should().BeTrue();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertFalse_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    {|AXM1006:Assert.False(condition)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    condition.Should().BeFalse();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertEmpty_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    {|AXM1007:Assert.Empty(values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    values.Should().BeEmpty();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertNotEmpty_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    {|AXM1008:Assert.NotEmpty(values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    values.Should().NotBeEmpty();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected)
                {
                    {|AXM1009:Assert.Contains(expected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected)
                {
                    values.Should().Contain(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values, int unexpected)
                {
                    {|AXM1010:Assert.DoesNotContain(unexpected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values, int unexpected)
                {
                    values.Should().NotContain(unexpected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1017:Assert.Contains("sub", actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    actual.Should().Contain("sub");
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringCollectionOverload_UsesCollectionDiagnostic()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string[] values, string expected)
                {
                    {|AXM1009:Assert.Contains(expected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(string[] values, string expected)
                {
                    values.Should().Contain(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class StringWrapper
            {
                public static implicit operator string(StringWrapper value) => "";
            }

            public sealed class Sample
            {
                public void Check(StringWrapper wrapper)
                {
                    Assert.Contains("sub", wrapper);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    Assert.Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    Assert.DoesNotContain(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    Assert.Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_IDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    Assert.Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ConcreteReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using System.Collections.ObjectModel;
            using Xunit;

            public sealed class Sample
            {
                public void Check(ReadOnlyDictionary<int, string> values, int key)
                {
                    Assert.Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using System.Collections.ObjectModel;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(ReadOnlyDictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ImmutableDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Immutable;
            using Xunit;

            public sealed class Sample
            {
                public void Check(ImmutableDictionary<int, string> values, int key)
                {
                    Assert.Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Immutable;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(ImmutableDictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public string Check(Dictionary<int, string> values, int key)
                {
                    var value = Assert.Contains(key, values);
                    return value;
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public string Check(Dictionary<int, string> values, int key)
                {
                    var value = values.Should().ContainKey(key).WhoseValue;
                    return value;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ReadOnlyDictionaryOverload_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections;
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public string Check(Lookup values, int key)
                {
                    return Assert.Contains(key, values);
                }

                private sealed class Lookup : IReadOnlyDictionary<int, string>
                {
                    public IEnumerable<int> Keys => throw null!;
                    public IEnumerable<string> Values => throw null!;
                    public int Count => throw null!;
                    public string this[int key] => throw null!;

                    public bool ContainsKey(int key) => throw null!;
                    public bool TryGetValue(int key, out string value) => throw null!;
                    public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => throw null!;
                    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections;
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public string Check(Lookup values, int key)
                {
                    return values.Should().ContainKey(key).WhoseValue;
                }

                private sealed class Lookup : IReadOnlyDictionary<int, string>
                {
                    public IEnumerable<int> Keys => throw null!;
                    public IEnumerable<string> Values => throw null!;
                    public int Count => throw null!;
                    public string this[int key] => throw null!;

                    public bool ContainsKey(int key) => throw null!;
                    public bool TryGetValue(int key, out string value) => throw null!;
                    public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => throw null!;
                    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_InArgumentPosition_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    Use(Assert.Contains(key, values));
                }

                private static void Use(string value) { }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    Use(values.Should().ContainKey(key).WhoseValue);
                }

                private static void Use(string value) { }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_ReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    Assert.DoesNotContain(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_IDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    Assert.DoesNotContain(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IDictionary<int, string> values, int key)
                {
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_ConcurrentDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Concurrent;
            using Xunit;

            public sealed class Sample
            {
                public void Check(ConcurrentDictionary<int, string> values, int key)
                {
                    Assert.DoesNotContain(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Concurrent;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(ConcurrentDictionary<int, string> values, int key)
                {
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1018:Assert.DoesNotContain("sub", actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    actual.Should().NotContain("sub");
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class StringWrapper
            {
                public static implicit operator string(StringWrapper value) => "";
            }

            public sealed class Sample
            {
                public void Check(StringWrapper wrapper)
                {
                    Assert.DoesNotContain("sub", wrapper);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringCollectionOverload_UsesCollectionDiagnostic()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string[] values, string unexpected)
                {
                    {|AXM1010:Assert.DoesNotContain(unexpected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(string[] values, string unexpected)
                {
                    values.Should().NotContain(unexpected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    {|AXM1011:Assert.Single(values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    values.Should().ContainSingle();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_NonGenericEnumerable_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections;
            using Xunit;

            public sealed class Sample
            {
                public void Check(ArrayList values)
                {
                    {|AXM1011:Assert.Single(values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(ArrayList values)
                {
                    values.Should().ContainSingle();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    var value = Assert.Single(values);
                    return value;
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    var value = values.Should().ContainSingle().SingleItem;
                    return value;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_UsedResult_ReadOnlyList_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(IReadOnlyList<int> values)
                {
                    return Assert.Single(values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(IReadOnlyList<int> values)
                {
                    return values.Should().ContainSingle().SingleItem;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_ReturnExpression_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return values.Should().ContainSingle().SingleItem;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_ArgumentPosition_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Use(Assert.Single(values));
                }

                private static int Use(int value) => value;
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Use(values.Should().ContainSingle().SingleItem);
                }

                private static int Use(int value) => value;
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_PredicateOverload_UnusedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    Assert.Single(values, value => value > 0);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    values.Should().ContainSingle(value => value > 0);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_PredicateOverload_UnusedResult_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    {|AXM1019:Assert.Single(values, value => value > 0)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateOverload_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    var value = Assert.Single(values, candidate => candidate > 0);
                    return value;
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    var value = values.Should().ContainSingle(candidate => candidate > 0).SingleItem;
                    return value;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_PredicateMethodGroup_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values, IsPositive);
                }

                private static bool IsPositive(int value) => value > 0;
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return values.Should().ContainSingle(IsPositive).SingleItem;
                }

                private static bool IsPositive(int value) => value > 0;
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSingle_PredicateVariable_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    Predicate<int> match = value => value > 0;
                    return Assert.Single(values, match);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateNullLiteral_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values, null);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateDefaultLiteral_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values, default);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateTypedDefault_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values, default(Predicate<int>));
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSame_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, object actual)
                {
                    {|AXM1012:Assert.Same(expected, actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object expected, object actual)
                {
                    actual.Should().BeSameAs(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertSame_StringActual_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string expected, string actual)
                {
                    Assert.Same(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertNotSame_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, object actual)
                {
                    {|AXM1013:Assert.NotSame(expected, actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object expected, object actual)
                {
                    actual.Should().NotBeSameAs(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertNotSame_TaskActual_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, Task actual)
                {
                    Assert.NotSame(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsActionVariable_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(Action work)
                {
                    {|AXM1014:Assert.Throws<InvalidOperationException>(work)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(Action work)
                {
                    work.Should().Throw<InvalidOperationException>();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsLambda_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Assert.Throws<InvalidOperationException>(() => ThrowNow())|};
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check()
                {
                    new Action(() => ThrowNow()).Should().Throw<InvalidOperationException>();
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsMethodGroup_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Assert.Throws<InvalidOperationException>(ThrowNow)|};
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check()
                {
                    new Action(ThrowNow).Should().Throw<InvalidOperationException>();
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsMethodGroup_AddsSystemUsing_InsteadOfFullyQualifiedAction()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Assert.Throws<global::System.InvalidOperationException>(ThrowNow)|};
                }

                private static void ThrowNow() => throw new global::System.InvalidOperationException();
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;
            using System;

            public sealed class Sample
            {
                public void Check()
                {
                    new Action(ThrowNow).Should().Throw<global::System.InvalidOperationException>();
                }

                private static void ThrowNow() => throw new global::System.InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsType_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    {|AXM1015:Assert.IsType<string>(actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    actual.Should().BeOfType<string>();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsType_StringActual_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    Assert.IsType<string>(actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsType_NonGenericOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    Assert.IsType(typeof(string), actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsType_ResultIsConsumed_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public string Check(object actual)
                {
                    var typed = Assert.IsType<string>(actual);
                    return typed;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsAssignableFrom_IsFlagged_AndFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    {|AXM1016:Assert.IsAssignableFrom<System.IDisposable>(actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    actual.Should().BeAssignableTo<System.IDisposable>();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsAssignableFrom_StringActual_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    Assert.IsAssignableFrom<object>(actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsAssignableFrom_NonGenericOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object actual)
                {
                    Assert.IsAssignableFrom(typeof(IDisposable), actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertIsAssignableFrom_ResultIsConsumed_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public IDisposable Check(object actual)
                {
                    var typed = Assert.IsAssignableFrom<System.IDisposable>(actual);
                    return typed;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssert_IsFlagged()
    {
        const string source =
            """
            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    {|AXM1001:Xunit.Assert.Equal(expected, actual)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertIsType_IsFlagged()
    {
        const string source =
            """
            public sealed class Sample
            {
                public void Check(object actual)
                {
                    {|AXM1015:Xunit.Assert.IsType<string>(actual)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertIsAssignableFrom_IsFlagged()
    {
        const string source =
            """
            public sealed class Sample
            {
                public void Check(object actual)
                {
                    {|AXM1016:Xunit.Assert.IsAssignableFrom<System.IDisposable>(actual)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected)
                {
                    {|AXM1009:Xunit.Assert.Contains(expected, values)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertSingle_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Xunit.Assert.Single(values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return values.Should().ContainSingle().SingleItem;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingAssert_IsFlagged()
    {
        const string source =
            """
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    {|AXM1005:True(condition)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StaticUsingSingle_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Single(values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return values.Should().ContainSingle().SingleItem;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingThrows_IsFlagged()
    {
        const string source =
            """
            using System;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Throws<InvalidOperationException>(() => ThrowNow())|};
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StaticUsingThrows_ParamNameOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check()
                {
                    Throws<ArgumentNullException>("name", () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using static Xunit.Assert;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check()
                {
                    new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name");
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingContains_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1017:Contains("sub", actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using static Xunit.Assert;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    actual.Should().Contain("sub");
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingDoesNotContain_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1018:DoesNotContain("sub", actual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using static Xunit.Assert;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    actual.Should().NotContain("sub");
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingContains_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    Contains(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingDoesNotContain_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    DoesNotContain(key, values);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task NonXunitAssert_IsNotFlagged()
    {
        const string source =
            """
            using Custom;

            namespace Custom
            {
                public static class Assert
                {
                    public static void Equal<T>(T expected, T actual) { }
                }
            }

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_StringOverload_IsFlagged()
    {
        const string source =
            """
            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1017:Xunit.Assert.Contains("sub", actual)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertThrows_ParamNameOverload_IsFlagged()
    {
        const string source =
            """
            using System;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Xunit.Assert.Throws<ArgumentNullException>("name", () => ThrowNow())|};
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_StringOverload_IsFlagged()
    {
        const string source =
            """
            public sealed class Sample
            {
                public void Check(string actual)
                {
                    {|AXM1018:Xunit.Assert.DoesNotContain("sub", actual)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_DictionaryOverload_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    {|AXM1020:Xunit.Assert.Contains(key, values)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_DictionaryOverload_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public void Check(IReadOnlyDictionary<int, string> values, int key)
                {
                    {|AXM1021:Xunit.Assert.DoesNotContain(key, values)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_DictionaryConcreteOverload_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    {|AXM1020:Xunit.Assert.Contains(key, values)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_DictionaryConcreteOverload_IsFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    {|AXM1021:Xunit.Assert.DoesNotContain(key, values)|};
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_ReturnValueUsage_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections;
            using Xunit;

            public sealed class Sample
            {
                public object Check(ArrayList values)
                {
                    var value = Assert.Single(values);
                    return value;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedContainSingleSingleItem_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return values.Should().ContainSingle().SingleItem;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateOverload_WithStaticUsing_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    Single(values, static value => value > 0);
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using static Xunit.Assert;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values)
                {
                    values.Should().ContainSingle(static value => value > 0);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_ReturnValueUsage_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public InvalidOperationException Check()
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => ThrowNow());
                    return exception;
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    {|AXM1014:Assert.Throws<ArgumentNullException>("name", () => ThrowNow())|};
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check()
                {
                    new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name");
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_WithConstString_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                private const string ParamName = "name";

                public void Check()
                {
                    {|AXM1014:Assert.Throws<ArgumentNullException>(ParamName, () => ThrowNow())|};
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                private const string ParamName = "name";

                public void Check()
                {
                    new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName(ParamName);
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_Assignment_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public ArgumentNullException Check()
                {
                    var ex = Assert.Throws<ArgumentNullException>("name", () => ThrowNow());
                    return ex;
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public ArgumentNullException Check()
                {
                    var ex = new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
                    return ex;
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_ReturnValue_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public ArgumentNullException Check()
                {
                    return Assert.Throws<ArgumentNullException>("name", () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public ArgumentNullException Check()
                {
                    return new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_ArgumentPosition_IsFlagged_AndFixed()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Use(Assert.Throws<ArgumentNullException>("name", () => ThrowNow()));
                }

                private static void Use(ArgumentNullException exception) { }
                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        const string fixedSource =
            """
            using System;
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check()
                {
                    Use(new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown);
                }

                private static void Use(ArgumentNullException exception) { }
                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrows_FuncOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Assert.Throws<InvalidOperationException>(() => GetValue());
                }

                private static object? GetValue() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_WithNullLiteral_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Assert.Throws<ArgumentNullException>(null, () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_WithDefaultString_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Assert.Throws<ArgumentNullException>(default(string), () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_WithConstNull_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                private const string? ParamName = null;

                public void Check()
                {
                    Assert.Throws<ArgumentNullException>(ParamName, () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException();
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrows_ParamNameOverload_WithVariableString_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(string paramName)
                {
                    Assert.Throws<ArgumentNullException>(paramName, () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomAssertion_IsNotFlagged()
    {
        const string source =
            """
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(int expected, int actual)
                {
                    actual.Should().Be(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomThrowAssertion_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public ArgumentNullException Check()
                {
                    return new Action(() => ThrowNow()).Should().Throw<ArgumentNullException>().WithParamName("name").Thrown;
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomDictionaryAssertion_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(Dictionary<int, string> values, int key)
                {
                    values.Should().ContainKey(key);
                    var value = values.Should().ContainKey(key).WhoseValue;
                    values.Should().NotContainKey(key);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task MessageBearingTrueOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    Assert.True(condition, "message");
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ComparerOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(string expected, string actual, IEqualityComparer<string> comparer)
                {
                    Assert.Equal(expected, actual, comparer);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task PrecisionOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(double expected, double actual)
                {
                    Assert.Equal(expected, actual, 2);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringComparisonOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string expected, string actual)
                {
                    Assert.Equal(expected, actual, ignoreCase: true, ignoreLineEndingDifferences: false, ignoreWhiteSpaceDifferences: false);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task EnumerableEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IEnumerable<int> expected, IEnumerable<int> actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ActionEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, Action actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AsyncActionEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, Func<Task> actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AsyncFunctionEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, Func<Task<int>> actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ValueTaskEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, ValueTask actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task TaskEqualityOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(object expected, Task actual)
                {
                    Assert.Equal(expected, actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ActionNullOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check(Action value)
                {
                    Assert.Null(value);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task TaskNotNullOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Xunit;

            public sealed class Sample
            {
                public void Check(Task value)
                {
                    Assert.NotNull(value);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AsyncEnumerableNotNullOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(IAsyncEnumerable<int> value)
                {
                    Assert.NotNull(value);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NullableBooleanOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(bool? condition)
                {
                    Assert.True(condition);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AsyncEnumerableEmptyOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Assert.Empty(GetValues());
                }

                private static async IAsyncEnumerable<int> GetValues()
                {
                    yield break;
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CodeFix_AddsExtensionsUsing_ForContainmentMigration()
    {
        const string source =
            """
            using Axiom.Assertions;
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected)
                {
                    {|AXM1009:Assert.Contains(expected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Axiom.Assertions;
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected)
                {
                    values.Should().Contain(expected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task CodeFix_AddsOnlyMissingExtensionsUsing_WhenAxiomUsingAlreadyExists()
    {
        const string source =
            """
            using Axiom.Assertions;
            using Xunit;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    {|AXM1005:Assert.True(condition)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Axiom.Assertions;
            using Xunit;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(bool condition)
                {
                    condition.Should().BeTrue();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MultipleContainAssertions_AreAllFixed()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected, int otherExpected)
                {
                    {|AXM1009:Assert.Contains(expected, values)|};
                    {|AXM1009:Assert.Contains(otherExpected, values)|};
                }
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using Xunit;
            using Axiom.Assertions;
            using Axiom.Assertions.Extensions;

            public sealed class Sample
            {
                public void Check(List<int> values, int expected, int otherExpected)
                {
                    values.Should().Contain(expected);
                    values.Should().Contain(otherExpected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MultipleEqualAssertions_AreAllFixed()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(int expected, int actual, int otherExpected, int otherActual)
                {
                    {|AXM1001:Assert.Equal(expected, actual)|};
                    {|AXM1001:Assert.Equal(otherExpected, otherActual)|};
                }
            }
            """;

        const string fixedSource =
            """
            using Xunit;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public void Check(int expected, int actual, int otherExpected, int otherActual)
                {
                    actual.Should().Be(expected);
                    otherActual.Should().Be(otherExpected);
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }
}
