using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationThrowsTests
{
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

}
