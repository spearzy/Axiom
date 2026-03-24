using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class UndisposedBatchAnalyzerTests
{
    [Fact]
    public async Task LocalDeclaration_WithNewBatch_IsFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    var batch = [|new Batch()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task LocalDeclaration_WithAssertBatch_IsFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    var batch = [|Assert.Batch("user")|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task BareNewBatchExpression_IsFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    [|new Batch()|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task BareAssertBatchExpression_IsFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    [|Assert.Batch("user")|];
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task UsingVar_IsNotFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    using var batch = Assert.Batch("user");
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task UsingStatement_IsNotFlagged()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    using (var batch = new Batch())
                    {
                    }
                }
            }
            """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<UndisposedBatchAnalyzer>(source);
    }

    [Fact]
    public async Task CodeFix_ConvertsLocalDeclaration_ToUsingVar()
    {
        const string source =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    var batch = [|Assert.Batch("user")|];
                }
            }
            """;

        const string fixedSource =
            """
            using Axiom.Core;

            public sealed class Sample
            {
                public void Check()
                {
                    using var batch = Assert.Batch("user");
                }
            }
            """;

        await AnalyzerVerifier.VerifyCodeFixAsync<UndisposedBatchAnalyzer, DisposeBatchCodeFixProvider>(source, fixedSource);
    }
}
