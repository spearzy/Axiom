namespace Axiom.Assertions.AssertionTypes;

internal static class StringDifferenceDiagnostics
{
    private const int SnippetRadius = 8;

    public static string BuildEqualityFailureDetail(string expected, string actual)
    {
        return BuildDifferenceDetail("first string difference", expected, actual, actualOffset: 0);
    }

    public static string BuildStartWithFailureDetail(string expectedPrefix, string actual)
    {
        var comparedPrefix = actual.Length <= expectedPrefix.Length
            ? actual
            : actual[..expectedPrefix.Length];

        var difference = BuildDifferenceDetail("start comparison", expectedPrefix, comparedPrefix, actualOffset: 0);
        if (actual.Length < expectedPrefix.Length)
        {
            return $"subject shorter than expected prefix ({actual.Length} < {expectedPrefix.Length}); {difference}";
        }

        return difference;
    }

    public static string BuildEndWithFailureDetail(string expectedSuffix, string actual)
    {
        var comparedSuffix = actual.Length <= expectedSuffix.Length
            ? actual
            : actual[^expectedSuffix.Length..];

        var comparedStart = actual.Length - comparedSuffix.Length;
        var difference = BuildDifferenceDetail("end comparison", expectedSuffix, comparedSuffix, actualOffset: comparedStart);
        if (actual.Length < expectedSuffix.Length)
        {
            return $"subject shorter than expected suffix ({actual.Length} < {expectedSuffix.Length}); {difference}";
        }

        return difference;
    }

    public static string BuildContainFailureDetail(string expectedSubstring, string actual)
    {
        if (actual.Length == 0)
        {
            return $"subject is empty; {BuildDifferenceDetail("contain comparison", expectedSubstring, string.Empty, actualOffset: 0)}";
        }

        var closestWindow = SelectClosestWindow(actual, expectedSubstring, out var startIndex);
        var detail = BuildDifferenceDetail(
            $"closest subject segment starts at index {startIndex}",
            expectedSubstring,
            closestWindow,
            actualOffset: startIndex);

        if (expectedSubstring.Length > actual.Length)
        {
            return $"expected substring length {expectedSubstring.Length} exceeds subject length {actual.Length}; {detail}";
        }

        return detail;
    }

    public static string BuildNotContainFailureDetail(string unexpectedSubstring, string actual, int matchIndex)
    {
        if (matchIndex < 0 || unexpectedSubstring.Length == 0 || matchIndex > actual.Length - unexpectedSubstring.Length)
        {
            return "unexpected substring detected.";
        }

        var matchedSegment = actual.Substring(matchIndex, unexpectedSubstring.Length);
        return BuildDifferenceDetail(
            $"unexpected match at index {matchIndex}",
            unexpectedSubstring,
            matchedSegment,
            actualOffset: matchIndex);
    }

    private static string SelectClosestWindow(string actual, string expected, out int startIndex)
    {
        if (actual.Length == 0)
        {
            startIndex = 0;
            return string.Empty;
        }

        var windowLength = Math.Min(actual.Length, Math.Max(1, expected.Length));
        var maxStart = actual.Length - windowLength;
        var bestStart = 0;
        var bestPrefixLength = -1;

        for (var candidateStart = 0; candidateStart <= maxStart; candidateStart++)
        {
            var prefixLength = ComputeCommonPrefixLength(expected, actual, candidateStart, windowLength);
            if (prefixLength <= bestPrefixLength)
            {
                continue;
            }

            bestPrefixLength = prefixLength;
            bestStart = candidateStart;
            if (bestPrefixLength == windowLength || bestPrefixLength == expected.Length)
            {
                break;
            }
        }

        startIndex = bestStart;
        return actual.Substring(bestStart, windowLength);
    }

    private static int ComputeCommonPrefixLength(string expected, string actual, int start, int windowLength)
    {
        var limit = Math.Min(windowLength, expected.Length);
        var prefixLength = 0;
        while (prefixLength < limit && actual[start + prefixLength] == expected[prefixLength])
        {
            prefixLength++;
        }

        return prefixLength;
    }

    private static string BuildDifferenceDetail(string context, string expected, string actualSegment, int actualOffset)
    {
        var differenceIndex = FindFirstDifferenceIndex(expected, actualSegment);
        if (differenceIndex < 0)
        {
            return $"{context}; strings are identical";
        }

        return $"{context}; first difference at expected index {differenceIndex}, actual index {actualOffset + differenceIndex}; expected snippet {BuildSnippet(expected, differenceIndex)}, actual snippet {BuildSnippet(actualSegment, differenceIndex)}";
    }

    private static int FindFirstDifferenceIndex(string expected, string actual)
    {
        var commonLength = Math.Min(expected.Length, actual.Length);
        for (var index = 0; index < commonLength; index++)
        {
            if (expected[index] != actual[index])
            {
                return index;
            }
        }

        return expected.Length == actual.Length ? -1 : commonLength;
    }

    private static string BuildSnippet(string value, int focusIndex)
    {
        if (value.Length == 0)
        {
            return "\"\"";
        }

        var clampedFocus = Math.Clamp(focusIndex, 0, value.Length - 1);
        var startIndex = Math.Max(0, clampedFocus - SnippetRadius);
        var endIndex = Math.Min(value.Length, clampedFocus + SnippetRadius + 1);
        var snippet = value.Substring(startIndex, endIndex - startIndex);

        var hasPrefix = startIndex > 0;
        var hasSuffix = endIndex < value.Length;
        return $"{(hasPrefix ? "..." : string.Empty)}\"{EscapeForSnippet(snippet)}\"{(hasSuffix ? "..." : string.Empty)}";
    }

    private static string EscapeForSnippet(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
    }
}
