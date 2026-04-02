using System.Collections;
using System.Text;
using Axiom.Core.Configuration;

namespace Axiom.Assertions.AssertionTypes;

internal static partial class CollectionAssertionEngine
{
    private static string SubjectLabel(string? subjectExpression)
    {
        return string.IsNullOrWhiteSpace(subjectExpression) ? "<subject>" : subjectExpression;
    }

    private static IComparer<T> GetOrderComparer<T>()
    {
        if (DefaultOrderComparerCache<T>.HasDefaultOrderComparer)
        {
            return DefaultOrderComparerCache<T>.DefaultComparer;
        }

        throw new InvalidOperationException(
            $"Type '{typeof(T).FullName}' does not define a default ordering. Use an overload that accepts an IComparer<{typeof(T).Name}>.");
    }

    private static IEqualityComparer<T> GetComparer<T>()
    {
        if (AxiomServices.Configuration.ComparerProvider.TryGetEqualityComparer<T>(out var comparer) &&
            comparer is not null)
        {
            return comparer;
        }

        return EqualityComparer<T>.Default;
    }

    private static bool TryGetCount(IEnumerable subject, out int count)
    {
        // Prefer non-enumerating count paths when collection interfaces are available.
        if (subject is ICollection nonGenericCollection)
        {
            count = nonGenericCollection.Count;
            return true;
        }

        count = 0;
        return false;
    }

    private static int GetCount(IEnumerable subject)
    {
        return TryGetCount(subject, out var knownCount)
            ? knownCount
            : CountItems(subject);
    }

    private static T[] MaterialiseExpectedSequence<T>(IEnumerable<T> expectedSequence)
    {
        if (expectedSequence is T[] array)
        {
            return array;
        }

        var buffer = new List<T>();
        foreach (var item in expectedSequence)
        {
            buffer.Add(item);
        }

        return buffer.ToArray();
    }

    private static bool ContainsItem<T>(IReadOnlyList<T> values, T candidate, IEqualityComparer<T> comparer)
    {
        for (var i = 0; i < values.Count; i++)
        {
            if (comparer.Equals(values[i], candidate))
            {
                return true;
            }
        }

        return false;
    }

    private static string FormatSingleValue<T>(T value)
    {
        return AxiomServices.Configuration.ValueFormatter.Format(value);
    }

    private static string FormatSequence<T>(IReadOnlyList<T> values)
    {
        var formatter = AxiomServices.Configuration.ValueFormatter;
        if (values.Count == 0)
        {
            return "[]";
        }

        var builder = new StringBuilder();
        builder.Append('[');
        for (var i = 0; i < values.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            builder.Append(formatter.Format(values[i]));
        }

        builder.Append(']');
        return builder.ToString();
    }

    private static string FormatSortedSequence<T>(IEnumerable<T> values)
    {
        var formatter = AxiomServices.Configuration.ValueFormatter;
        var renderedValues = new List<string>();
        foreach (var value in values)
        {
            renderedValues.Add(formatter.Format(value));
        }

        renderedValues.Sort(StringComparer.Ordinal);

        if (renderedValues.Count == 0)
        {
            return "[]";
        }

        var builder = new StringBuilder();
        builder.Append('[');
        for (var i = 0; i < renderedValues.Count; i++)
        {
            if (i > 0)
            {
                builder.Append(", ");
            }

            builder.Append(renderedValues[i]);
        }

        builder.Append(']');
        return builder.ToString();
    }

    private static string FormatEntry<TKey, TValue>(TKey key, TValue value)
    {
        return $"{FormatSingleValue(key)} => {FormatSingleValue(value)}";
    }

    private static int CountItems(IEnumerable subject)
    {
        var count = 0;
        var enumerator = subject.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                count++;
            }
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }

        return count;
    }

    private readonly record struct RenderedText(string Text)
    {
        public override string ToString()
        {
            return Text;
        }
    }

    private static class DefaultOrderComparerCache<T>
    {
        public static readonly IComparer<T> DefaultComparer = Comparer<T>.Default;
        public static readonly bool HasDefaultOrderComparer =
            typeof(IComparable<T>).IsAssignableFrom(typeof(T)) ||
            typeof(IComparable).IsAssignableFrom(typeof(T));
    }
}
