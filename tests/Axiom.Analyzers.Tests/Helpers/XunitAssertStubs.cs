namespace Axiom.Analyzers.Tests.Helpers;

internal static class XunitAssertStubs
{
    public const string Source =
        """
        using System;
        using System.Collections;
        using System.Collections.Concurrent;
        using System.Collections.Generic;
        using System.Collections.Immutable;
        using System.Collections.ObjectModel;

        namespace Xunit
        {
            public static class Assert
            {
                public static void Equal<T>(T expected, T actual) { }
                public static void Equal(string expected, string actual) { }
                public static void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual) { }
                public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
                public static void Equal(string expected, string actual, bool ignoreCase, bool ignoreLineEndingDifferences, bool ignoreWhiteSpaceDifferences) { }
                public static void Equal(double expected, double actual, int precision) { }

                public static void NotEqual<T>(T expected, T actual) { }
                public static void NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) { }
                public static void NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
                public static void NotEqual(double expected, double actual, int precision) { }

                public static void Null(object? value) { }
                public static T NotNull<T>(T value) => value;

                public static void True(bool condition) { }
                public static void True(bool condition, string userMessage) { }
                public static void True(bool? condition) { }

                public static void False(bool condition) { }
                public static void False(bool condition, string userMessage) { }
                public static void False(bool? condition) { }

                public static void Empty(IEnumerable collection) { }
                public static void Empty(string value) { }
                public static void Empty<T>(IAsyncEnumerable<T> value) { }

                public static void NotEmpty(IEnumerable collection) { }
                public static void NotEmpty(string value) { }
                public static void NotEmpty<T>(IAsyncEnumerable<T> value) { }

                public static void Contains<T>(T expected, IEnumerable<T> collection) { }
                public static void Contains(string expectedSubstring, string actualString) { }
                public static void Contains(string expectedSubstring, string actualString, StringComparison comparisonType) { }
                public static TValue Contains<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection) => default!;
                public static TValue Contains<TKey, TValue>(TKey expected, IReadOnlyDictionary<TKey, TValue> collection) => default!;
                public static TValue Contains<TKey, TValue>(TKey expected, ConcurrentDictionary<TKey, TValue> collection) => default!;
                public static TValue Contains<TKey, TValue>(TKey expected, Dictionary<TKey, TValue> collection) => default!;
                public static TValue Contains<TKey, TValue>(TKey expected, ImmutableDictionary<TKey, TValue> collection) => default!;
                public static TValue Contains<TKey, TValue>(TKey expected, ReadOnlyDictionary<TKey, TValue> collection) => default!;

                public static void DoesNotContain<T>(T expected, IEnumerable<T> collection) { }
                public static void DoesNotContain(string expectedSubstring, string actualString) { }
                public static void DoesNotContain(string expectedSubstring, string actualString, StringComparison comparisonType) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, IDictionary<TKey, TValue> collection) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, IReadOnlyDictionary<TKey, TValue> collection) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, ConcurrentDictionary<TKey, TValue> collection) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, Dictionary<TKey, TValue> collection) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, ImmutableDictionary<TKey, TValue> collection) { }
                public static void DoesNotContain<TKey, TValue>(TKey expected, ReadOnlyDictionary<TKey, TValue> collection) { }

                public static void StartsWith(string expectedPrefix, string actualString) { }
                public static void StartsWith(string expectedPrefix, string actualString, StringComparison comparisonType) { }
                public static void EndsWith(string expectedSuffix, string actualString) { }
                public static void EndsWith(string expectedSuffix, string actualString, StringComparison comparisonType) { }

                public static T Single<T>(IEnumerable<T> collection) => default!;
                public static object Single(IEnumerable collection) => default!;
                public static T Single<T>(IEnumerable<T> collection, Predicate<T> predicate) => default!;

                public static void Same(object? expected, object? actual) { }
                public static void NotSame(object? expected, object? actual) { }
                public static T IsType<T>(object? value) => default!;
                public static object IsType(Type expectedType, object? value) => default!;
                public static T IsAssignableFrom<T>(object? value) => default!;
                public static object IsAssignableFrom(Type expectedType, object? value) => default!;

                public static T Throws<T>(Action testCode) where T : Exception => default!;
                public static T Throws<T>(Func<object?> testCode) where T : Exception => default!;
                public static T Throws<T>(string? paramName, Action testCode) where T : Exception => default!;
            }
        }
        """;
}
