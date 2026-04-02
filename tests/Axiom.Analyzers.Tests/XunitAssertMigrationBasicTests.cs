using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationBasicTests
{
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
