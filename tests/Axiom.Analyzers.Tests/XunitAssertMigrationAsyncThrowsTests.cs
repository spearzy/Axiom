using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationAsyncThrowsTests
{
    [Fact]
    public async Task AwaitedAssertThrowsAsync_ActionVariable_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check(Func<Task> work)
                    {
                        await {|AXM1054:Assert.ThrowsAsync<InvalidOperationException>(work)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check(Func<Task> work)
                    {
                        await work.Should().ThrowExactlyAsync<InvalidOperationException>();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAsync_Lambda_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1054:Assert.ThrowsAsync<System.InvalidOperationException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new System.InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;
                using System;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<System.InvalidOperationException>();
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new System.InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAsync_ParamNameOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1054:Assert.ThrowsAsync<ArgumentNullException>("name", () => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        (await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<ArgumentNullException>()).WithParamName("name");
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAsync_ResultAssignment_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task<System.InvalidOperationException> Check()
                    {
                        var ex = await Assert.ThrowsAsync<System.InvalidOperationException>(() => ThrowNowAsync());
                        return ex;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new System.InvalidOperationException());
                }
                """;

        const string fixedSource =
            """
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;
                using System;

                public sealed class Sample
                {
                    public async Task<System.InvalidOperationException> Check()
                    {
                        var ex = (await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<System.InvalidOperationException>()).Thrown;
                        return ex;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new System.InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAsync_ParamName_ResultArgument_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Use(await Assert.ThrowsAsync<ArgumentNullException>("name", () => ThrowNowAsync()));
                    }

                    private static void Use(ArgumentNullException exception) { }
                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        Use((await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowExactlyAsync<ArgumentNullException>()).WithParamName("name").Thrown);
                    }

                    private static void Use(ArgumentNullException exception) { }
                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAnyAsync_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await {|AXM1055:Assert.ThrowsAnyAsync<ArgumentException>(() => ThrowNowAsync())|};
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task Check()
                    {
                        await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowAsync<ArgumentException>();
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AwaitedAssertThrowsAnyAsync_ResultReturn_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task<ArgumentException> Check()
                    {
                        return await Assert.ThrowsAnyAsync<ArgumentException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        const string fixedSource =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public async Task<ArgumentException> Check()
                    {
                        return (await new Func<System.Threading.Tasks.Task>(() => ThrowNowAsync()).Should().ThrowAsync<ArgumentException>()).Thrown;
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertThrowsAsync_WithoutAwait_IsNotFlagged()
    {
        const string source =
            """
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public Task Check()
                    {
                        return Assert.ThrowsAsync<System.InvalidOperationException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new System.InvalidOperationException());
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAsync_ParamNameOverload_WithVariableString_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public async Task Check(string paramName)
                    {
                        await Assert.ThrowsAsync<ArgumentNullException>(paramName, () => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertThrowsAnyAsync_WithoutAwait_IsNotFlagged()
    {
        const string source =
            """
                using System;
                using System.Threading.Tasks;
                using Xunit;

                public sealed class Sample
                {
                    public Task<ArgumentException> Check()
                    {
                        return Assert.ThrowsAnyAsync<ArgumentException>(() => ThrowNowAsync());
                    }

                    private static Task ThrowNowAsync() => Task.FromException(new ArgumentNullException("name"));
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }
}
