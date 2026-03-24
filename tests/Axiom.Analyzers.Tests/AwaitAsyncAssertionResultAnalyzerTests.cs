using Axiom.Analyzers.Tests.Helpers;
using Axiom.Analyzers.CodeFixes;

namespace Axiom.Analyzers.Tests;

public sealed class AwaitAsyncAssertionResultAnalyzerTests
{
    [Fact]
    public async Task Ignored_ThrowAsync_IsFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Action act = static () => { };
                    [|act.Should().ThrowAsync<InvalidOperationException>()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Ignored_NotThrowAsync_IsFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task operation = Task.CompletedTask;
                    [|operation.Should().NotThrowAsync()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Ignored_Succeed_IsFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    [|loader.Should().Succeed()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Ignored_SucceedWithin_IsFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    [|loader.Should().SucceedWithin(TimeSpan.FromSeconds(1))|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Ignored_BeFaultedWith_IsFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromException<int>(new InvalidOperationException());
                    [|loader.Should().BeFaultedWith<InvalidOperationException>()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Awaited_AsyncAssertion_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    await loader.Should().Succeed();
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Returned_AsyncAssertion_IsNotFlagged()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;
            using Axiom.Assertions.AssertionTypes;

            public sealed class Sample
            {
                public ValueTask<SuccessfulTaskContinuation<TaskAssertions<int>, int>> CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    return loader.Should().Succeed();
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task Discarded_AsyncAssertion_IsFlagged()
    {
        const string source =
            """
            using System;
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Action act = static () => { };
                    _ = [|act.Should().ThrowAsync<InvalidOperationException>()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<AwaitAsyncAssertionResultAnalyzer>(source);
    }

    [Fact]
    public async Task CodeFix_PrependsAwait_InAsyncMethod()
    {
        const string source =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    [|loader.Should().Succeed()|];
                }
            }
            """;

        const string fixedSource =
            """
            using System.Threading.Tasks;
            using Axiom.Assertions;

            public sealed class Sample
            {
                public async Task CheckAsync()
                {
                    Task<int> loader = Task.FromResult(42);
                    await loader.Should().Succeed();
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<AwaitAsyncAssertionResultAnalyzer, AwaitAsyncAssertionResultCodeFixProvider>(source, fixedSource);
    }
}
