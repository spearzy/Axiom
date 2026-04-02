using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class MstestAssertMigrationBasicTests
{
    [Fact]
    public async Task AssertAreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreNotEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1033:Assert.AreNotEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().NotBe(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsNull_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1034:Assert.IsNull(value)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().BeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsNotNull_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        {|AXM1035:Assert.IsNotNull(value)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object? value)
                    {
                        value.Should().NotBeNull();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsTrue_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1036:Assert.IsTrue(condition)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertIsFalse_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(bool condition)
                    {
                        {|AXM1037:Assert.IsFalse(condition)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreSame_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        {|AXM1038:Assert.AreSame(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        actual.Should().BeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertAreNotSame_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        {|AXM1039:Assert.AreNotSame(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(object expected, object actual)
                    {
                        actual.Should().NotBeSameAs(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task FullyQualified_AssertAreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task UsingStaticAssert_AreEqual_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        {|AXM1032:AreEqual(expected, actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        actual.Should().Be(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MultipleAreEqualAssertions_AreAllFixed()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, int otherExpected, int otherActual)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                        {|AXM1032:Assert.AreEqual(otherExpected, otherActual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        await AnalyzerVerifier.VerifyCodeFixAsync<MstestAssertMigrationAnalyzer, MstestAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task MixedSupportedAssertions_AreFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, object? value, bool condition)
                    {
                        {|AXM1032:Assert.AreEqual(expected, actual)|};
                        {|AXM1034:Assert.IsNull(value)|};
                        {|AXM1037:Assert.IsFalse(condition)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task MessageOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        Assert.AreEqual(expected, actual, "custom message");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task PrecisionOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(double expected, double actual)
                    {
                        Assert.AreEqual(expected, actual, 0.1);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task ComparerStyleOverload_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string expected, string actual)
                    {
                        Assert.AreEqual(expected, actual, true);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task CollectionEquality_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(List<int> expected, List<int> actual)
                    {
                        Assert.AreEqual(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NullLiteralReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check()
                    {
                        Assert.IsNull(null);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task StringReferenceEquality_IsNotFlagged()
    {
        const string source =
            """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                public sealed class Sample
                {
                    public void Check(string expected, string actual)
                    {
                        Assert.AreSame(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task NonMstestAssert_IsNotFlagged()
    {
        const string source =
            """
                public static class Assert
                {
                    public static void AreEqual<T>(T expected, T actual)
                    {
                    }
                }

                public sealed class Sample
                {
                    public void Check(int expected, int actual)
                    {
                        Assert.AreEqual(expected, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomCode_IsNotFlagged()
    {
        const string source =
            """
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(int expected, int actual, object? value, bool condition, object expectedReference, object actualReference)
                    {
                        actual.Should().Be(expected);
                        actual.Should().NotBe(expected);
                        value.Should().BeNull();
                        value.Should().NotBeNull();
                        condition.Should().BeTrue();
                        condition.Should().BeFalse();
                        actualReference.Should().BeSameAs(expectedReference);
                        actualReference.Should().NotBeSameAs(expectedReference);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<MstestAssertMigrationAnalyzer>(source);
    }
}
