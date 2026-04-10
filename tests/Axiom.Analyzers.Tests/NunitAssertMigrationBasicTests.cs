using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class NunitAssertMigrationBasicTests
{
    [Fact]
    public async Task AssertThat_EqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        {|AXM1024:Assert.That(actual, Is.EqualTo(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_NotEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        {|AXM1025:Assert.That(actual, Is.Not.EqualTo(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().NotBe(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_Null_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1026:Assert.That(value, Is.Null)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().BeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_NotNull_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1027:Assert.That(value, Is.Not.Null)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().NotBeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_True_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1028:Assert.That(condition, Is.True)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_False_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1029:Assert.That(condition, Is.False)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_Empty_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        {|AXM1030:Assert.That(values, Is.Empty)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_NotEmpty_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values)
                    {
                        {|AXM1031:Assert.That(values, Is.Not.Empty)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_DoesContain_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedSubstring)
                    {
                        {|AXM1040:Assert.That(actual, Does.Contain(expectedSubstring))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedSubstring)
                    {
                        actual.Should().Contain(expectedSubstring);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_DoesNotContain_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string unexpectedSubstring)
                    {
                        {|AXM1041:Assert.That(actual, Does.Not.Contain(unexpectedSubstring))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual, string unexpectedSubstring)
                    {
                        actual.Should().NotContain(unexpectedSubstring);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_DoesStartWith_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1042:Assert.That(actual, Does.StartWith("pre"))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_DoesEndWith_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1043:Assert.That(actual, Does.EndWith("suf"))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().EndWith("suf");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_HasCountEqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expectedCount)
                    {
                        {|AXM1044:Assert.That(values, Has.Count.EqualTo(expectedCount))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using NUnit.Framework;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(List<int> values, int expectedCount)
                    {
                        values.Should().HaveCount(expectedCount);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_IsSameAs_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object actual, object expected)
                    {
                        {|AXM1045:Assert.That(actual, Is.SameAs(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object actual, object expected)
                    {
                        actual.Should().BeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThat_IsNotSameAs_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(object actual, object expected)
                    {
                        {|AXM1046:Assert.That(actual, Is.Not.SameAs(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object actual, object expected)
                    {
                        actual.Should().NotBeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task FullyQualified_AssertThat_EqualTo_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        {|AXM1024:NUnit.Framework.Assert.That(actual, Is.EqualTo(expected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task UsingStaticAssert_ThatCall_IsFlagged_AndFixed()
    {
        const string source =
            """
                using NUnit.Framework;
                using static NUnit.Framework.Assert;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1028:That(condition, Is.True)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using static NUnit.Framework.Assert;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MultipleSupportedAssertions_AreAllFixed()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected, int otherActual, int otherExpected)
                    {
                        {|AXM1024:Assert.That(actual, Is.EqualTo(expected))|};
                        {|AXM1024:Assert.That(otherActual, Is.EqualTo(otherExpected))|};
                    }
                }
                """;

        const string fixedSource =
            """
                using NUnit.Framework;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int actual, int expected, int otherActual, int otherExpected)
                    {
                        actual.Should().Be(expected);
                        otherActual.Should().Be(otherExpected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<NunitAssertMigrationAnalyzer, NunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task UnsupportedConstraintChain_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(double actual, double expected)
                    {
                        Assert.That(actual, Is.EqualTo(expected).Within(0.1));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task UnsupportedComparerVariation_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string expected, object comparer)
                    {
                        Assert.That(actual, Is.EqualTo(expected).Using(comparer));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task MessageVariation_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Assert.That(actual, Is.EqualTo(expected), "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task DoesContain_WithCollectionSubject_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<string> values)
                    {
                        Assert.That(values, Does.Contain("sub"));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task HasCount_WithStringSubject_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.That(actual, Has.Count.EqualTo(2));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task DoesStartWith_WithVariableExpectedPrefix_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string expectedPrefix)
                    {
                        Assert.That(actual, Does.StartWith(expectedPrefix));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task IsSameAs_WithStringSubject_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(string actual, string expected)
                    {
                        Assert.That(actual, Is.SameAs(expected));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task EqualityWithCollectionExpected_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(List<int> actual, List<int> expected)
                    {
                        Assert.That(actual, Is.EqualTo(expected));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task EqualityWithObjectExpected_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual, object expected)
                    {
                        Assert.That(actual, Is.EqualTo(expected));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task EqualityWithNullExpectedOnValueType_IsNotFlagged()
    {
        const string source =
            """
                using NUnit.Framework;

                public sealed class Sample
                {
                    public void Check(int actual)
                    {
                        Assert.That(actual, Is.EqualTo(null));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NonNunitAssert_IsNotFlagged()
    {
        const string source =
            """
                namespace Custom
                {
                    public static class Assert
                    {
                        public static void That<T>(T actual, object constraint) { }
                    }
                }

                public sealed class IsLike
                {
                    public static object EqualTo(object? expected) => expected!;
                }

                public sealed class Sample
                {
                    public void Check(int actual, int expected)
                    {
                        Custom.Assert.That(actual, IsLike.EqualTo(expected));
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomCode_IsNotFlagged()
    {
        const string source =
            """
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;
                using System.Collections.Generic;

                public sealed class Sample
                {
                    public void Check(int actual, int expected, List<int> values, bool condition)
                    {
                        actual.Should().Be(expected);
                        values.Should().BeEmpty();
                        condition.Should().BeTrue();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<NunitAssertMigrationAnalyzer>(source);
    }
}
