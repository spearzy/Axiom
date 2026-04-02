using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationTypeTests
{
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

}
