using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

// Minimal xUnit/NUnit/MSTest surface for migration examples in the docs. Keep this small and intentional.
namespace Xunit
{
    public static class Assert
    {
        public static void Equal<T>(T expected, T actual) { }
        public static void Equal<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
        public static void NotEqual<T>(T expected, T actual) { }
        public static void NotEqual<T>(T expected, T actual, IEqualityComparer<T> comparer) { }
        public static void Null(object? value) { }
        public static T NotNull<T>(T value) => value;
        public static void True(bool condition) { }
        public static void False(bool condition) { }
        public static void Empty(IEnumerable collection) { }
        public static void NotEmpty(IEnumerable collection) { }
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
        public static T Single<T>(IEnumerable<T> collection, Predicate<T> predicate) => default!;
        public static object Single(IEnumerable collection) => default!;
        public static void Same(object? expected, object? actual) { }
        public static void NotSame(object? expected, object? actual) { }
        public static T Throws<T>(Action testCode) where T : Exception => default!;
        public static T Throws<T>(string? paramName, Action testCode) where T : Exception => default!;
        public static T IsType<T>(object? value) => default!;
        public static T IsAssignableFrom<T>(object? value) => default!;
    }
}

namespace NUnit.Framework
{
    using Constraints;

    public static class Assert
    {
        public static void That<TActual>(TActual actual, IResolveConstraint expression) { }
    }

    public static class Is
    {
        public static ConstraintExpression Not => default!;
        public static NullConstraint Null => default!;
        public static TrueConstraint True => default!;
        public static FalseConstraint False => default!;
        public static EmptyConstraint Empty => default!;
        public static EqualConstraint EqualTo(object? expected) => default!;
    }
}

namespace NUnit.Framework.Constraints
{
    public interface IResolveConstraint { }

    public class ConstraintExpression : IResolveConstraint
    {
        public ConstraintExpression Not => default!;
        public NullConstraint Null => default!;
        public TrueConstraint True => default!;
        public FalseConstraint False => default!;
        public EmptyConstraint Empty => default!;
        public EqualConstraint EqualTo(object? expected) => default!;
    }

    public sealed class EqualConstraint : ConstraintExpression { }
    public sealed class NullConstraint : ConstraintExpression { }
    public sealed class TrueConstraint : ConstraintExpression { }
    public sealed class FalseConstraint : ConstraintExpression { }
    public sealed class EmptyConstraint : ConstraintExpression { }
}

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual) { }
        public static void AreNotEqual<T>(T expected, T actual) { }
        public static void IsNull(object? value) { }
        public static void IsNotNull(object? value) { }
        public static void IsTrue(bool condition) { }
        public static void IsFalse(bool condition) { }
        public static void AreSame(object? expected, object? actual) { }
        public static void AreNotSame(object? expected, object? actual) { }
    }
}
