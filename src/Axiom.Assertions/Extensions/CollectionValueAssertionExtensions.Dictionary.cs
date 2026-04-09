using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Axiom.Assertions.AssertionTypes;
using Axiom.Assertions.Chaining;

namespace Axiom.Assertions.Extensions;

public static partial class CollectionValueAssertionExtensions
{
    public static ContainKeyContinuation<ValueAssertions<TDictionary>, TValue> ContainKey<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var result = CollectionAssertionEngine.AssertContainKeyAndCaptureResult(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainKeyContinuation<ValueAssertions<TDictionary>, TValue>(
            assertions,
            result.HasValue,
            result.Value,
            result.FailureMessage);
    }

    public static ContainKeyContinuation<ValueAssertions<Dictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainKey<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static ContainKeyContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainKey<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static ContainKeyContinuation<ValueAssertions<IDictionary<TKey, TValue>>, TValue> ContainKey<TKey, TValue>(
        this ValueAssertions<IDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var subject = AsReadOnlyDictionary(assertions.Subject);

        var result = CollectionAssertionEngine.AssertContainKeyAndCaptureResult(
            subject,
            assertions.SubjectExpression,
            expectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new ContainKeyContinuation<ValueAssertions<IDictionary<TKey, TValue>>, TValue>(
            assertions,
            result.HasValue,
            result.Value,
            result.FailureMessage);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainKey<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainKey(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainKey<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainKey<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IDictionary<TKey, TValue>>> NotContainKey<TKey, TValue>(
        this ValueAssertions<IDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(assertions);

        var subject = AsReadOnlyDictionary(assertions.Subject);

        CollectionAssertionEngine.AssertNotContainKey(
            subject,
            assertions.SubjectExpression,
            unexpectedKey,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<IDictionary<TKey, TValue>>>(assertions);
    }

    private static IReadOnlyDictionary<TKey, TValue>? AsReadOnlyDictionary<TKey, TValue>(
        IDictionary<TKey, TValue>? subject)
        where TKey : notnull
    {
        return subject switch
        {
            null => null,
            IReadOnlyDictionary<TKey, TValue> readOnly => readOnly,
            _ => new ReadOnlyDictionary<TKey, TValue>(subject),
        };
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedValue,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedValue,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainValue<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertNotContainValue(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainValue<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainValue<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedKey,
            expectedValue,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> ContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey expectedKey,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            expectedKey,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> ContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey expectedKey,
        TValue expectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return ContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            expectedKey,
            expectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);

        CollectionAssertionEngine.AssertNotContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedKey,
            unexpectedValue,
            comparer: null,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<TDictionary>> NotContainEntry<TDictionary, TKey, TValue>(
        this ValueAssertions<TDictionary> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TDictionary : IReadOnlyDictionary<TKey, TValue>
    {
        ArgumentNullException.ThrowIfNull(assertions);
        ArgumentNullException.ThrowIfNull(comparer);

        CollectionAssertionEngine.AssertNotContainEntry(
            assertions.Subject,
            assertions.SubjectExpression,
            unexpectedKey,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);

        return new AndContinuation<ValueAssertions<TDictionary>>(assertions);
    }

    public static AndContinuation<ValueAssertions<Dictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<Dictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<Dictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static AndContinuation<ValueAssertions<IReadOnlyDictionary<TKey, TValue>>> NotContainEntry<TKey, TValue>(
        this ValueAssertions<IReadOnlyDictionary<TKey, TValue>> assertions,
        TKey unexpectedKey,
        TValue unexpectedValue,
        IEqualityComparer<TValue> comparer,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
        where TKey : notnull
    {
        return NotContainEntry<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(
            assertions,
            unexpectedKey,
            unexpectedValue,
            comparer,
            because,
            callerFilePath,
            callerLineNumber);
    }
}
