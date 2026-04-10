using Axiom.Analyzers.CodeFixes;
using Axiom.Analyzers.Tests.Helpers;

namespace Axiom.Analyzers.Tests;

public sealed class XunitAssertMigrationContainmentTests
{
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
    public async Task AssertContains_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1017:Assert.Contains("sub", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().Contain("sub");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringComparisonOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.Contains("sub", actual, StringComparison.OrdinalIgnoreCase);
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
                    public void Check(string actual)
                    {
                        actual.Should().Contain("sub", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringCollectionOverload_UsesCollectionDiagnostic()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string[] values, string expected)
                    {
                        {|AXM1009:Assert.Contains(expected, values)|};
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
                    public void Check(string[] values, string expected)
                    {
                        values.Should().Contain(expected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class StringWrapper
                {
                    public static implicit operator string(StringWrapper value) => "";
                }

                public sealed class Sample
                {
                    public void Check(StringWrapper wrapper)
                    {
                        Assert.Contains("sub", wrapper);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_IsFlagged_AndFixed()
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

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(Dictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_DictionaryOverload_IsFlagged_AndFixed()
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

        const string fixedSource =
            """
                using System.Collections.Generic;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(Dictionary<int, string> values, int key)
                    {
                        values.Should().NotContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        Assert.Contains(key, values);
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
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_IDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        Assert.Contains(key, values);
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
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ConcreteReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using System.Collections.ObjectModel;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(ReadOnlyDictionary<int, string> values, int key)
                    {
                        Assert.Contains(key, values);
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Generic;
                using System.Collections.ObjectModel;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(ReadOnlyDictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ImmutableDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Immutable;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(ImmutableDictionary<int, string> values, int key)
                    {
                        Assert.Contains(key, values);
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Immutable;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(ImmutableDictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public string Check(Dictionary<int, string> values, int key)
                    {
                        var value = Assert.Contains(key, values);
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
                    public string Check(Dictionary<int, string> values, int key)
                    {
                        var value = values.Should().ContainKey(key).WhoseValue;
                        return value;
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_ReadOnlyDictionaryOverload_UsedResult_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections;
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public string Check(Lookup values, int key)
                    {
                        return Assert.Contains(key, values);
                    }

                    private sealed class Lookup : IReadOnlyDictionary<int, string>
                    {
                        public IEnumerable<int> Keys => throw null!;
                        public IEnumerable<string> Values => throw null!;
                        public int Count => throw null!;
                        public string this[int key] => throw null!;

                        public bool ContainsKey(int key) => throw null!;
                        public bool TryGetValue(int key, out string value) => throw null!;
                        public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => throw null!;
                        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections;
                using System.Collections.Generic;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public string Check(Lookup values, int key)
                    {
                        return values.Should().ContainKey(key).WhoseValue;
                    }

                    private sealed class Lookup : IReadOnlyDictionary<int, string>
                    {
                        public IEnumerable<int> Keys => throw null!;
                        public IEnumerable<string> Values => throw null!;
                        public int Count => throw null!;
                        public string this[int key] => throw null!;

                        public bool ContainsKey(int key) => throw null!;
                        public bool TryGetValue(int key, out string value) => throw null!;
                        public IEnumerator<KeyValuePair<int, string>> GetEnumerator() => throw null!;
                        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertContains_DictionaryOverload_InArgumentPosition_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        Use(Assert.Contains(key, values));
                    }

                    private static void Use(string value) { }
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
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        Use(values.Should().ContainKey(key).WhoseValue);
                    }

                    private static void Use(string value) { }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_ReadOnlyDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        Assert.DoesNotContain(key, values);
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
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        values.Should().NotContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_IDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        Assert.DoesNotContain(key, values);
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
                    public void Check(IDictionary<int, string> values, int key)
                    {
                        values.Should().NotContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_ConcurrentDictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Concurrent;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(ConcurrentDictionary<int, string> values, int key)
                    {
                        Assert.DoesNotContain(key, values);
                    }
                }
                """;

        const string fixedSource =
            """
                using System.Collections.Concurrent;
                using Xunit;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(ConcurrentDictionary<int, string> values, int key)
                    {
                        values.Should().NotContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1018:Assert.DoesNotContain("sub", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().NotContain("sub");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringComparisonOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.DoesNotContain("sub", actual, StringComparison.OrdinalIgnoreCase);
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
                    public void Check(string actual)
                    {
                        actual.Should().NotContain("sub", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class StringWrapper
                {
                    public static implicit operator string(StringWrapper value) => "";
                }

                public sealed class Sample
                {
                    public void Check(StringWrapper wrapper)
                    {
                        Assert.DoesNotContain("sub", wrapper);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1022:Assert.StartsWith("pre", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertStartsWith_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class StringWrapper
                {
                    public static implicit operator string(StringWrapper value) => "";
                }

                public sealed class Sample
                {
                    public void Check(StringWrapper wrapper)
                    {
                        Assert.StartsWith("pre", wrapper);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_StringOverload_WithNullReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check()
                    {
                        Assert.StartsWith("pre", null);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_StringOverload_WithNullExpectedPrefix_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.StartsWith(null, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_StringOverload_WithVariableExpectedPrefix_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string expectedPrefix, string actual)
                    {
                        Assert.StartsWith(expectedPrefix, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertStartsWith_StringComparisonOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.StartsWith("pre", actual, StringComparison.OrdinalIgnoreCase);
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
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertEndsWith_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1023:Assert.EndsWith("suf", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using Xunit;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().EndWith("suf");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertEndsWith_StringOverload_WithImplicitStringReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class StringWrapper
                {
                    public static implicit operator string(StringWrapper value) => "";
                }

                public sealed class Sample
                {
                    public void Check(StringWrapper wrapper)
                    {
                        Assert.EndsWith("suf", wrapper);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertEndsWith_StringOverload_WithDefaultReceiver_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check()
                    {
                        Assert.EndsWith("suf", default);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertEndsWith_StringOverload_WithDefaultExpectedSuffix_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.EndsWith(default(string), actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertEndsWith_StringOverload_WithVariableExpectedSuffix_IsNotFlagged()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string expectedSuffix, string actual)
                    {
                        Assert.EndsWith(expectedSuffix, actual);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AssertEndsWith_StringComparisonOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System;
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        Assert.EndsWith("suf", actual, StringComparison.OrdinalIgnoreCase);
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
                    public void Check(string actual)
                    {
                        actual.Should().EndWith("suf", StringComparison.OrdinalIgnoreCase);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task AssertDoesNotContain_StringCollectionOverload_UsesCollectionDiagnostic()
    {
        const string source =
            """
                using Xunit;

                public sealed class Sample
                {
                    public void Check(string[] values, string unexpected)
                    {
                        {|AXM1010:Assert.DoesNotContain(unexpected, values)|};
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
                    public void Check(string[] values, string unexpected)
                    {
                        values.Should().NotContain(unexpected);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
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
    public async Task StaticUsingContains_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1017:Contains("sub", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using static Xunit.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().Contain("sub");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingDoesNotContain_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1018:DoesNotContain("sub", actual)|};
                    }
                }
                """;

        const string fixedSource =
            """
                using static Xunit.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().NotContain("sub");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingStartsWith_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        StartsWith("pre", actual);
                    }
                }
                """;

        const string fixedSource =
            """
                using static Xunit.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingEndsWith_StringOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        EndsWith("suf", actual);
                    }
                }
                """;

        const string fixedSource =
            """
                using static Xunit.Assert;
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().EndWith("suf");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingContains_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        Contains(key, values);
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
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task StaticUsingDoesNotContain_DictionaryOverload_IsFlagged_AndFixed()
    {
        const string source =
            """
                using System.Collections.Generic;
                using static Xunit.Assert;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        DoesNotContain(key, values);
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
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        values.Should().NotContainKey(key);
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAppliedCodeFixAsync<XunitAssertMigrationAnalyzer, XunitAssertMigrationCodeFixProvider>(source, fixedSource);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_StringOverload_IsFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1017:Xunit.Assert.Contains("sub", actual)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_StringOverload_IsFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1018:Xunit.Assert.DoesNotContain("sub", actual)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertStartsWith_StringOverload_IsFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1022:Xunit.Assert.StartsWith("pre", actual)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertEndsWith_StringOverload_IsFlagged()
    {
        const string source =
            """
                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        {|AXM1023:Xunit.Assert.EndsWith("suf", actual)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_DictionaryOverload_IsFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        {|AXM1020:Xunit.Assert.Contains(key, values)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_DictionaryOverload_IsFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;

                public sealed class Sample
                {
                    public void Check(IReadOnlyDictionary<int, string> values, int key)
                    {
                        {|AXM1021:Xunit.Assert.DoesNotContain(key, values)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertContains_DictionaryConcreteOverload_IsFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;

                public sealed class Sample
                {
                    public void Check(Dictionary<int, string> values, int key)
                    {
                        {|AXM1020:Xunit.Assert.Contains(key, values)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task FullyQualifiedXunitAssertDoesNotContain_DictionaryConcreteOverload_IsFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;

                public sealed class Sample
                {
                    public void Check(Dictionary<int, string> values, int key)
                    {
                        {|AXM1021:Xunit.Assert.DoesNotContain(key, values)|};
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomStringPrefixSuffixAssertions_AreNotFlagged()
    {
        const string source =
            """
                using Axiom.Assertions;

                public sealed class Sample
                {
                    public void Check(string actual)
                    {
                        actual.Should().StartWith("pre");
                        actual.Should().EndWith("suf");
                    }
                }
                """;

        await AnalyzerVerifier.VerifyAnalyzerAsync<XunitAssertMigrationAnalyzer>(source);
    }

    [Fact]
    public async Task AlreadyMigratedAxiomDictionaryAssertion_IsNotFlagged()
    {
        const string source =
            """
                using System.Collections.Generic;
                using Axiom.Assertions;
                using Axiom.Assertions.Extensions;

                public sealed class Sample
                {
                    public void Check(Dictionary<int, string> values, int key)
                    {
                        values.Should().ContainKey(key);
                        var value = values.Should().ContainKey(key).WhoseValue;
                        values.Should().NotContainKey(key);
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

}
