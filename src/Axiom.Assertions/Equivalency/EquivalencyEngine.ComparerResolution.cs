using System.Collections;
using System.Reflection;
using Axiom.Core.Comparison;
using Axiom.Core.Configuration;

namespace Axiom.Assertions.Equivalency;

internal static partial class EquivalencyEngine
{
    private static bool LeafValuesEquivalent(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options)
    {
        // Per-assertion tolerance settings take priority for supported leaf types.
        if (TryCompareWithTolerance(actual, expected, options, out var toleranceResult))
        {
            return toleranceResult;
        }

        if (TryCompareWithConfiguredComparer(actual, expected, actualType, expectedType, options, out var comparerResult))
        {
            return comparerResult;
        }

        return Equals(actual, expected);
    }

    private static bool LeafValuesEquivalentWithoutTolerance(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options)
    {
        if (TryCompareWithConfiguredComparer(actual, expected, actualType, expectedType, options, out var comparerResult))
        {
            return comparerResult;
        }

        return Equals(actual, expected);
    }

    private static bool TryCompareWithTolerance(
        object actual,
        object expected,
        EquivalencyOptions options,
        out bool areEquivalent)
    {
        switch (actual)
        {
            // Absolute delta check for float with NaN/Infinity-safe handling.
            case float actualFloat when expected is float expectedFloat && options.FloatTolerance.HasValue:
                areEquivalent = AreFloatsEquivalent(actualFloat, expectedFloat, Math.Abs(options.FloatTolerance.Value));
                return true;

            // Absolute delta check for double with NaN/Infinity-safe handling.
            case double actualDouble when expected is double expectedDouble && options.DoubleTolerance.HasValue:
                areEquivalent = AreDoublesEquivalent(actualDouble, expectedDouble, Math.Abs(options.DoubleTolerance.Value));
                return true;

            // Compare Half values via float arithmetic to keep tolerance behaviour consistent.
            case Half actualHalf when expected is Half expectedHalf && options.HalfTolerance.HasValue:
                areEquivalent = AreFloatsEquivalent((float)actualHalf, (float)expectedHalf, Math.Abs(options.HalfTolerance.Value));
                return true;

            // Absolute delta check for decimal values.
            case decimal actualDecimal when expected is decimal expectedDecimal && options.DecimalTolerance.HasValue:
                areEquivalent = decimal.Abs(actualDecimal - expectedDecimal) <= decimal.Abs(options.DecimalTolerance.Value);
                return true;

            // DateOnly has day precision, so compare day gap against the configured window.
            case DateOnly actualDateOnly when expected is DateOnly expectedDateOnly && options.DateOnlyTolerance.HasValue:
                areEquivalent = TimeSpan.FromDays(Math.Abs(actualDateOnly.DayNumber - expectedDateOnly.DayNumber)) <=
                                NormaliseTemporalTolerance(options.DateOnlyTolerance.Value, nameof(EquivalencyOptions.DateOnlyTolerance));
                return true;

            // Compare clock instants by permitted time window.
            case DateTime actualDateTime when expected is DateTime expectedDateTime && options.DateTimeTolerance.HasValue:
                areEquivalent = (actualDateTime - expectedDateTime).Duration() <=
                                NormaliseTemporalTolerance(options.DateTimeTolerance.Value, nameof(EquivalencyOptions.DateTimeTolerance));
                return true;

            // Compare offset-aware instants by permitted time window.
            case DateTimeOffset actualDateTimeOffset when expected is DateTimeOffset expectedDateTimeOffset && options.DateTimeOffsetTolerance.HasValue:
                areEquivalent = (actualDateTimeOffset - expectedDateTimeOffset).Duration() <=
                                NormaliseTemporalTolerance(options.DateTimeOffsetTolerance.Value, nameof(EquivalencyOptions.DateTimeOffsetTolerance));
                return true;

            // Compare time-of-day values by permitted time window.
            case TimeOnly actualTimeOnly when expected is TimeOnly expectedTimeOnly && options.TimeOnlyTolerance.HasValue:
                areEquivalent = AbsoluteTimeOnlyDifference(actualTimeOnly, expectedTimeOnly) <=
                                NormaliseTemporalTolerance(options.TimeOnlyTolerance.Value, nameof(EquivalencyOptions.TimeOnlyTolerance));
                return true;

            // Compare durations by permitted time window.
            case TimeSpan actualTimeSpan when expected is TimeSpan expectedTimeSpan && options.TimeSpanTolerance.HasValue:
                areEquivalent = (actualTimeSpan - expectedTimeSpan).Duration() <=
                                NormaliseTemporalTolerance(options.TimeSpanTolerance.Value, nameof(EquivalencyOptions.TimeSpanTolerance));
                return true;
        }

        areEquivalent = false;
        return false;
    }

    private static bool AreFloatsEquivalent(float actual, float expected, float tolerance)
    {
        if (float.IsNaN(actual) || float.IsNaN(expected))
        {
            return float.IsNaN(actual) && float.IsNaN(expected);
        }

        if (float.IsInfinity(actual) || float.IsInfinity(expected))
        {
            return actual.Equals(expected);
        }

        return Math.Abs(actual - expected) <= tolerance;
    }

    private static bool AreDoublesEquivalent(double actual, double expected, double tolerance)
    {
        if (double.IsNaN(actual) || double.IsNaN(expected))
        {
            return double.IsNaN(actual) && double.IsNaN(expected);
        }

        if (double.IsInfinity(actual) || double.IsInfinity(expected))
        {
            return actual.Equals(expected);
        }

        return Math.Abs(actual - expected) <= tolerance;
    }

    private static TimeSpan NormaliseTemporalTolerance(TimeSpan tolerance, string optionName)
    {
        if (tolerance == TimeSpan.MinValue)
        {
            throw new ArgumentOutOfRangeException(optionName, "Tolerance must not be TimeSpan.MinValue.");
        }

        return tolerance.Duration();
    }

    private static TimeSpan AbsoluteTimeOnlyDifference(TimeOnly left, TimeOnly right)
    {
        var directTicks = Math.Abs(left.Ticks - right.Ticks);
        var wrappedTicks = TimeSpan.TicksPerDay - directTicks;
        return TimeSpan.FromTicks(Math.Min(directTicks, wrappedTicks));
    }

    private static bool TryCompareWithConfiguredComparer(
        object actual,
        object expected,
        Type actualType,
        Type expectedType,
        EquivalencyOptions options,
        out bool areEquivalent)
    {
        areEquivalent = false;
        // We can only ask the provider for one generic T, so first choose a shared T
        // that can represent both runtime values (same type or assignable base/interface).
        var comparisonType = GetSharedComparisonType(actualType, expectedType);
        if (comparisonType is null)
        {
            return false;
        }

        // Per-assertion type comparer should win over global provider behaviour.
        if (options.TryCompareWithTypeComparer(comparisonType, actual, expected, out var localComparerResult))
        {
            areEquivalent = localComparerResult;
            return true;
        }

        var provider = AxiomServices.Configuration.ComparerProvider;
        // Build (or reuse) a strongly-typed comparer call delegate for this runtime type.
        var invoker = ComparerInvokers.GetOrAdd(comparisonType, static type => BuildComparerInvoker(type));
        var comparerResult = invoker(provider, actual, expected);
        if (!comparerResult.HasValue)
        {
            return false;
        }

        areEquivalent = comparerResult.Value;
        return true;
    }

    private static bool TryCompareWithConfiguredPathComparer(
        object actual,
        object expected,
        string path,
        string rootPath,
        EquivalencyOptions options,
        out bool areEquivalent)
    {
        if (options.TryCompareWithPathComparer(path, actual, expected, out areEquivalent))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 &&
            options.TryCompareWithPathComparer(relativePath, actual, expected, out areEquivalent))
        {
            return true;
        }

        areEquivalent = false;
        return false;
    }

    private static bool TryGetConfiguredCollectionItemComparer(
        string path,
        string rootPath,
        EquivalencyOptions options,
        out IEqualityComparer comparer)
    {
        if (options.TryGetCollectionItemComparer(path, out comparer))
        {
            return true;
        }

        var relativePath = ToRelativePath(path, rootPath);
        if (relativePath.Length > 0 &&
            options.TryGetCollectionItemComparer(relativePath, out comparer))
        {
            return true;
        }

        comparer = null!;
        return false;
    }

    private static Type? GetSharedComparisonType(Type actualType, Type expectedType)
    {
        if (actualType == expectedType)
        {
            return actualType;
        }

        if (actualType.IsAssignableFrom(expectedType))
        {
            return actualType;
        }

        if (expectedType.IsAssignableFrom(actualType))
        {
            return expectedType;
        }

        return null;
    }

    private static Func<IComparerProvider, object, object, bool?> BuildComparerInvoker(Type comparisonType)
    {
        // Convert CompareWithProviderCore<T> into a callable delegate where T is only known at runtime.
        // This avoids repeated reflection inside hot comparison paths.
        var compareMethod = typeof(EquivalencyEngine)
            .GetMethod(nameof(CompareWithProviderCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(comparisonType);

        return (Func<IComparerProvider, object, object, bool?>)compareMethod
            .CreateDelegate(typeof(Func<IComparerProvider, object, object, bool?>));
    }

    private static bool? CompareWithProviderCore<T>(IComparerProvider provider, object actual, object expected)
    {
        // If no comparer exists for T, return null so caller can fall back to default equality.
        if (!provider.TryGetEqualityComparer<T>(out var comparer) || comparer is null)
        {
            return null;
        }

        // Delegate is only built for compatible types, so these casts are intentional and safe here.
        return comparer.Equals((T)actual, (T)expected);
    }
}
