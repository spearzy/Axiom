namespace Axiom.Analyzers.Tests.Helpers;

internal static class AxiomAssertionStubs
{
    public const string Source =
        """
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Axiom.Assertions.Chaining;

        namespace Axiom.Assertions
        {
            public static class ShouldExtensions
            {
                public static StringAssertions Should(this string? subject) => throw null!;
                public static ValueAssertions<T> Should<T>(this T subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.AsyncActionAssertions Should(this Action subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.AsyncFunctionAssertions<T> Should<T>(this Func<Task<T>> subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.TaskAssertions Should(this Task subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.TaskAssertions<T> Should<T>(this Task<T> subject) => throw null!;
            }

            public sealed class ValueAssertions<T>
            {
                public AndContinuation<ValueAssertions<T>> Be(T expected) => default;
                public AndContinuation<ValueAssertions<T>> NotBe(T unexpected) => default;
                public AndContinuation<ValueAssertions<T>> BeNull() => default;
                public AndContinuation<ValueAssertions<T>> NotBeNull() => default;
                public AndContinuation<ValueAssertions<T>> BeOfType<TExpected>() => default;
                public AndContinuation<ValueAssertions<T>> BeAssignableTo<TExpected>() => default;
                public AndContinuation<ValueAssertions<T>> BeSameAs(T? expectedReference) => default;
                public AndContinuation<ValueAssertions<T>> NotBeSameAs(T? unexpectedReference) => default;
            }

            public sealed class StringAssertions
            {
                public AndContinuation<StringAssertions> Be(string? expected) => default;
                public AndContinuation<StringAssertions> NotBe(string? unexpected) => default;
                public AndContinuation<StringAssertions> BeNull() => default;
                public AndContinuation<StringAssertions> NotBeNull() => default;
                public AndContinuation<StringAssertions> Contain(string expectedSubstring) => default;
                public AndContinuation<StringAssertions> NotContain(string unexpectedSubstring) => default;
                public AndContinuation<StringAssertions> BeEmpty() => default;
                public AndContinuation<StringAssertions> NotBeEmpty() => default;
            }
        }

        namespace Axiom.Assertions.Extensions
        {
            public static class BooleanValueAssertionExtensions
            {
                public static AndContinuation<Axiom.Assertions.ValueAssertions<bool>> BeTrue(this Axiom.Assertions.ValueAssertions<bool> assertions) => default;
                public static AndContinuation<Axiom.Assertions.ValueAssertions<bool>> BeFalse(this Axiom.Assertions.ValueAssertions<bool> assertions) => default;
            }

            public static class CollectionValueAssertionExtensions
            {
                public static AndContinuation<Axiom.Assertions.ValueAssertions<TCollection>> Contain<TCollection, TItem>(this Axiom.Assertions.ValueAssertions<TCollection> assertions, TItem expected)
                    where TCollection : IEnumerable<TItem>
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<TCollection>> NotContain<TCollection, TItem>(this Axiom.Assertions.ValueAssertions<TCollection> assertions, TItem expected)
                    where TCollection : IEnumerable<TItem>
                    => default;

                public static ContainKeyContinuation<Axiom.Assertions.ValueAssertions<TDictionary>, TValue> ContainKey<TDictionary, TKey, TValue>(this Axiom.Assertions.ValueAssertions<TDictionary> assertions, TKey expectedKey)
                    where TDictionary : IReadOnlyDictionary<TKey, TValue>
                    => default;

                public static ContainKeyContinuation<Axiom.Assertions.ValueAssertions<Dictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<Dictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static ContainKeyContinuation<Axiom.Assertions.ValueAssertions<IReadOnlyDictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static ContainKeyContinuation<Axiom.Assertions.ValueAssertions<IDictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<IDictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<TDictionary>> NotContainKey<TDictionary, TKey, TValue>(this Axiom.Assertions.ValueAssertions<TDictionary> assertions, TKey expectedKey)
                    where TDictionary : IReadOnlyDictionary<TKey, TValue>
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<Dictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<Dictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<IDictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(this Axiom.Assertions.ValueAssertions<IDictionary<TKey, TValue>> assertions, TKey expectedKey)
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<TCollection>> BeEmpty<TCollection>(this Axiom.Assertions.ValueAssertions<TCollection> assertions)
                    where TCollection : IEnumerable
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<TCollection>> NotBeEmpty<TCollection>(this Axiom.Assertions.ValueAssertions<TCollection> assertions)
                    where TCollection : IEnumerable
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<TCollection>, TItem> ContainSingle<TCollection, TItem>(this Axiom.Assertions.ValueAssertions<TCollection> assertions)
                    where TCollection : IEnumerable<TItem>
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<TItem[]>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<TItem[]> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<List<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<List<TItem>> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<IEnumerable<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<IEnumerable<TItem>> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<ICollection<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<ICollection<TItem>> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<IList<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<IList<TItem>> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<IReadOnlyCollection<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<IReadOnlyCollection<TItem>> assertions)
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<IReadOnlyList<TItem>>, TItem> ContainSingle<TItem>(this Axiom.Assertions.ValueAssertions<IReadOnlyList<TItem>> assertions)
                    => default;

                public static AndContinuation<Axiom.Assertions.ValueAssertions<TCollection>> ContainSingle<TCollection>(this Axiom.Assertions.ValueAssertions<TCollection> assertions)
                    where TCollection : IEnumerable
                    => default;

                public static ContainSingleContinuation<Axiom.Assertions.ValueAssertions<TCollection>, TItem> ContainSingle<TCollection, TItem>(this Axiom.Assertions.ValueAssertions<TCollection> assertions, Func<TItem, bool> predicate)
                    where TCollection : IEnumerable<TItem>
                    => default;
            }
        }

        namespace Axiom.Assertions.AssertionTypes
        {
            public sealed class AsyncActionAssertions
            {
                public ThrownExceptionAssertions<AsyncActionAssertions, TException> Throw<TException>() where TException : Exception => default;
                public ValueTask<ThrownExceptionAssertions<AsyncActionAssertions, TException>> ThrowAsync<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<AsyncActionAssertions>> NotThrowAsync() => default;
            }

            public sealed class AsyncFunctionAssertions<T>
            {
                public ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<T>, TException>> ThrowAsync<TException>() where TException : Exception => default;
                public ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<T>, TException>> BeFaultedWith<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<AsyncFunctionAssertions<T>>> NotThrowAsync() => default;
                public ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<T>, T>> Succeed() => default;
                public ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<T>, T>> SucceedWithin(TimeSpan timeout) => default;
            }

            public sealed class TaskAssertions
            {
                public ValueTask<AndContinuation<TaskAssertions>> NotThrowAsync() => default;
            }

            public sealed class TaskAssertions<T>
            {
                public ValueTask<ThrownExceptionAssertions<TaskAssertions<T>, TException>> BeFaultedWith<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<TaskAssertions<T>>> NotThrowAsync() => default;
                public ValueTask<SuccessfulTaskContinuation<TaskAssertions<T>, T>> Succeed() => default;
                public ValueTask<SuccessfulTaskContinuation<TaskAssertions<T>, T>> SucceedWithin(TimeSpan timeout) => default;
            }

            public readonly struct ThrownExceptionAssertions<TParent, TException> where TException : Exception
            {
                public TException Thrown => throw null!;
                public ThrownExceptionAssertions<TParent, TException> WithParamName(string expectedParamName) => default;
            }
            public readonly struct SuccessfulTaskContinuation<TParent, TResult> { }
        }

        namespace Axiom.Assertions.Chaining
        {
            public readonly struct AndContinuation<TAssertions>
            {
                public TAssertions And => throw null!;
            }

            public readonly struct ContainSingleContinuation<TAssertions, TItem>
            {
                public TAssertions And => throw null!;
                public TItem SingleItem => throw null!;
            }

            public readonly struct ContainKeyContinuation<TAssertions, TValue>
            {
                public TAssertions And => throw null!;
                public TValue WhoseValue => throw null!;
            }

            public readonly struct SuccessfulTaskContinuation<TParent, TResult> { }
        }

        namespace Axiom.Core
        {
            public sealed class Batch : IDisposable
            {
                public Batch() { }
                public void Dispose() { }
            }

            public static class Assert
            {
                public static Batch Batch(string? name = null) => throw null!;
            }
        }
        """;
}
