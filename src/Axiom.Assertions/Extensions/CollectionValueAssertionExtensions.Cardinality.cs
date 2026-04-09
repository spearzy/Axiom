using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static partial class CollectionValueAssertionExtensions
{
    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItems<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var dispatcher = HaveUniqueItemsGenericDispatcherCache<TCollection>.Dispatcher;
        if (dispatcher is not null)
        {
            dispatcher(
                assertions.Subject,
                assertions.SubjectExpression,
                because,
                callerFilePath,
                callerLineNumber);
        }
        else
        {
            CollectionAssertionEngine.AssertHaveUniqueItems(
                (IEnumerable?)assertions.Subject,
                assertions.SubjectExpression,
                because,
                callerFilePath,
                callerLineNumber);
        }

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItems<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertHaveUniqueItems(
            assertions.Subject,
            assertions.SubjectExpression,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItemsBy<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);

        CollectionAssertionEngine.AssertHaveUniqueItemsBy(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveUniqueItemsBy<TCollection, TItem, TKey>(
        this ValueAssertions<TCollection> assertions,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertHaveUniqueItemsBy(
            assertions.Subject,
            assertions.SubjectExpression,
            keySelector,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainExactly<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);

        CollectionAssertionEngine.AssertContainExactly(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainExactly<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainExactly(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainExactlyInAnyOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);

        CollectionAssertionEngine.AssertContainExactlyInAnyOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> ContainExactlyInAnyOrder<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSequence,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSequence);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainExactlyInAnyOrder(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSequence,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSubsetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSuperset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSuperset);

        CollectionAssertionEngine.AssertBeSubsetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSuperset,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSubsetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSuperset,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSuperset);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeSubsetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSuperset,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSupersetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSubset,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSubset);

        CollectionAssertionEngine.AssertBeSupersetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSubset,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeSupersetOf<TCollection, TItem>(
        this ValueAssertions<TCollection> assertions,
        IEnumerable<TItem> expectedSubset,
        IEqualityComparer<TItem> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable<TItem>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(expectedSubset);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertBeSupersetOf(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedSubset,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> HaveCount<TCollection>(
        this ValueAssertions<TCollection> assertions,
        int expectedCount,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertHaveCount(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedCount,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> BeEmpty<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertBeEmpty(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }

    public static AndContinuation<ValueAssertions<TCollection>> NotBeEmpty<TCollection>(
        this ValueAssertions<TCollection> assertions,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TCollection : IEnumerable
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotBeEmpty(
            assertions.Subject,
            assertions.SubjectExpression,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TCollection>>(assertions);
    }
    private static void AssertHaveUniqueItemsGeneric<TCollection, TItem>(
        TCollection subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        where TCollection : IEnumerable<TItem>
    {
        CollectionAssertionEngine.AssertHaveUniqueItems<TItem>(
            subject,
            subjectExpression,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static Type? FindEnumerableItemType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return interfaceType.GetGenericArguments()[0];
            }
        }

        return null;
    }

    private static class HaveUniqueItemsGenericDispatcherCache<TCollection>
    {
        public static readonly Action<TCollection, string?, string?, string?, int>? Dispatcher = BuildDispatcher();

        private static Action<TCollection, string?, string?, string?, int>? BuildDispatcher()
        {
            var itemType = FindEnumerableItemType(typeof(TCollection));
            if (itemType is null)
            {
                return null;
            }

            var method = typeof(CollectionValueAssertionExtensions)
                .GetMethod(nameof(AssertHaveUniqueItemsGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(typeof(TCollection), itemType);

            return (Action<TCollection, string?, string?, string?, int>)method.CreateDelegate(
                typeof(Action<TCollection, string?, string?, string?, int>));
        }
    }
}
