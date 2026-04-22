using Axiom.Core.Failures;

namespace Axiom.Http;

internal static class HttpResponseAssertionEngine
{
    public static void AssertHaveStatusCode(
        HttpResponseMessage? subject,
        string? subjectExpression,
        int expected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have status code", HttpAssertionSupport.FormatStatusCode(expected));
        AssertStatusCode(subject, subjectExpression, expected, expectation, shouldMatch: true, because, callerFilePath, callerLineNumber);
    }

    public static void AssertNotHaveStatusCode(
        HttpResponseMessage? subject,
        string? subjectExpression,
        int unexpected,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to not have status code", HttpAssertionSupport.FormatStatusCode(unexpected));
        AssertStatusCode(subject, subjectExpression, unexpected, expectation, shouldMatch: false, because, callerFilePath, callerLineNumber);
    }

    public static void AssertHaveHeader(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string name,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        HttpAssertionSupport.ValidateHeaderName(name);
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to have header {name}", IncludeExpectedValue: false);

        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (HttpAssertionSupport.TryGetHeaderValues(subject, name, out _))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            new HttpDisplay($"missing header {name}"),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotHaveHeader(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string name,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        HttpAssertionSupport.ValidateHeaderName(name);
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to not have header {name}", IncludeExpectedValue: false);

        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!HttpAssertionSupport.TryGetHeaderValues(subject, name, out var actualValues))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            HttpAssertionSupport.DescribeHeader(name, actualValues),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveHeaderValue(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string name,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        HttpAssertionSupport.ValidateHeaderName(name);
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation($"to have header {name} value", expectedValue);

        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!HttpAssertionSupport.TryGetHeaderValues(subject, name, out var actualValues))
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay($"missing header {name}"),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (actualValues.Length == 1 && string.Equals(actualValues[0], expectedValue, StringComparison.Ordinal))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            HttpAssertionSupport.DescribeHeader(name, actualValues),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveHeaderValues(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string name,
        string[] expectedValues,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        HttpAssertionSupport.ValidateHeaderName(name);
        HttpAssertionSupport.ValidateExpectedValues(expectedValues, nameof(expectedValues));
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var expectation = new Expectation(
            $"to have header {name} values",
            HttpAssertionSupport.FormatHeaderValues(expectedValues));

        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        if (!HttpAssertionSupport.TryGetHeaderValues(subject, name, out var actualValues))
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay($"missing header {name}"),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (actualValues.Length == expectedValues.Length)
        {
            var allMatch = true;
            for (var index = 0; index < actualValues.Length; index++)
            {
                if (!string.Equals(actualValues[index], expectedValues[index], StringComparison.Ordinal))
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                return;
            }
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            HttpAssertionSupport.DescribeHeader(name, actualValues),
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveContentType(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedMediaType,
        string? expectedCharset,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        HttpAssertionSupport.ValidateMediaType(expectedMediaType, nameof(expectedMediaType));
        if (expectedCharset is not null)
        {
            HttpAssertionSupport.ValidateMediaType(expectedCharset, nameof(expectedCharset));
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var expectedDisplay = expectedCharset is null
            ? new HttpDisplay(expectedMediaType)
            : new HttpDisplay($"{expectedMediaType}; charset={expectedCharset}");
        var expectation = new Expectation("to have content type", expectedDisplay);

        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        var contentType = subject.Content?.Headers.ContentType;
        if (contentType is null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay("no content type"),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (!string.Equals(contentType.MediaType, expectedMediaType, StringComparison.OrdinalIgnoreCase))
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                HttpAssertionSupport.DescribeContentType(contentType),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        if (expectedCharset is null)
        {
            return;
        }

        if (string.Equals(contentType.CharSet, expectedCharset, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            HttpAssertionSupport.DescribeContentType(contentType),
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static void AssertStatusCode(
        HttpResponseMessage? subject,
        string? subjectExpression,
        int expected,
        Expectation expectation,
        bool shouldMatch,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        if (subject is null)
        {
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return;
        }

        var actual = (int)subject.StatusCode;
        if ((shouldMatch && actual == expected) || (!shouldMatch && actual != expected))
        {
            return;
        }

        HttpAssertionSupport.Fail(
            subjectLabel,
            expectation,
            HttpAssertionSupport.FormatStatusCode(actual),
            because,
            callerFilePath,
            callerLineNumber);
    }
}
