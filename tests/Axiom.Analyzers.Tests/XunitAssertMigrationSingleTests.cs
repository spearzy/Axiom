using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationSingleTests
{
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

}
