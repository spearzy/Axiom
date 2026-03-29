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
                Assert.Equal("xUnit Assert.Single(...) can be migrated to 'subject.Should().ContainSingle()'", rule.MessageFormat.ToString());
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
                Assert.Equal("xUnit Assert.Throws<TException>(...) can be migrated to an Axiom '.Should().Throw<TException>()' assertion", rule.MessageFormat.ToString());
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
                    new global::System.Action(work).Should().Throw<InvalidOperationException>();
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
                    new global::System.Action(() => ThrowNow()).Should().Throw<InvalidOperationException>();
                }

                private static void ThrowNow() => throw new InvalidOperationException();
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
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
    public async Task StringContainsOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    Assert.Contains("sub", actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringDoesNotContainOverload_IsNotFlagged()
    {
        const string source =
            """
            using Xunit;

            public sealed class Sample
            {
                public void Check(string actual)
                {
                    Assert.DoesNotContain("sub", actual);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task DictionaryContainsOverload_IsNotFlagged()
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

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task DictionaryDoesNotContainOverload_IsNotFlagged()
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

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_ReturnValueUsage_IsNotFlagged()
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

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertSingle_PredicateOverload_IsNotFlagged()
    {
        const string source =
            """
            using System.Collections.Generic;
            using Xunit;

            public sealed class Sample
            {
                public int Check(List<int> values)
                {
                    return Assert.Single(values, static value => value > 0);
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
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
    public async Task AssertThrows_ParamNameOverload_IsNotFlagged()
    {
        const string source =
            """
            using System;
            using Xunit;

            public sealed class Sample
            {
                public void Check()
                {
                    Assert.Throws<ArgumentNullException>("name", () => ThrowNow());
                }

                private static void ThrowNow() => throw new ArgumentNullException("name");
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
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
