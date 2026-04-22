using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using Axiom.Core.Failures;

namespace Axiom.Http;

internal static class HttpAssertionSupport
{
    public static void Fail(
        string subjectLabel,
        Expectation expectation,
        object? actual,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var failure = new Failure(subjectLabel, expectation, actual, because);
        AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
    }

    public static string SubjectLabel(string? subjectExpression) => string.IsNullOrWhiteSpace(subjectExpression) ? "response" : subjectExpression;

    public static string JsonBodySubjectLabel(string? subjectExpression) => $"{SubjectLabel(subjectExpression)} JSON body";

    public static string ProblemDetailsSubjectLabel(string? subjectExpression) => $"{SubjectLabel(subjectExpression)} problem details body";

    public static HttpDisplay FormatStatusCode(int statusCode)
    {
        var knownName = Enum.GetName(typeof(HttpStatusCode), statusCode);
        return knownName is null
            ? new HttpDisplay(statusCode.ToString(CultureInfo.InvariantCulture))
            : new HttpDisplay($"{statusCode.ToString(CultureInfo.InvariantCulture)} ({knownName})");
    }

    public static HttpDisplay FormatHeaderValues(IReadOnlyList<string> values)
        => new($"[{string.Join(", ", values.Select(static value => Quote(value)))}]");

    public static HttpDisplay DescribeHeader(string name, IReadOnlyList<string> values)
        => new($"header {name} with values {FormatHeaderValues(values)}");

    public static HttpDisplay DescribeContentType(MediaTypeHeaderValue contentType)
    {
        var text = string.IsNullOrWhiteSpace(contentType.CharSet)
            ? contentType.MediaType ?? string.Empty
            : $"{contentType.MediaType}; charset={contentType.CharSet}";
        return new HttpDisplay(text);
    }

    public static bool TryGetHeaderValues(HttpResponseMessage response, string name, out string[] values)
    {
        List<string>? collected = null;

        if (response.Headers.TryGetValues(name, out var responseValues))
        {
            collected = [.. responseValues];
        }

        if (response.Content?.Headers.TryGetValues(name, out var contentValues) is true)
        {
            collected ??= [];
            collected.AddRange(contentValues);
        }

        if (collected is null)
        {
            values = [];
            return false;
        }

        values = [.. collected];
        return true;
    }

    public static string? ReadContent(HttpResponseMessage response)
    {
        if (response.Content is null)
        {
            return null;
        }

        return response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public static void ValidateHeaderName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("name must not be null or whitespace.", nameof(name));
        }
    }

    public static void ValidateExpectedValues(string[] expectedValues, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(expectedValues, parameterName);
        if (expectedValues.Length == 0)
        {
            throw new ArgumentException($"{parameterName} must contain at least one value.", parameterName);
        }

        for (var index = 0; index < expectedValues.Length; index++)
        {
            if (expectedValues[index] is null)
            {
                throw new ArgumentException($"{parameterName} must not contain null values.", parameterName);
            }
        }
    }

    public static void ValidateMediaType(string expectedMediaType, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(expectedMediaType))
        {
            throw new ArgumentException($"{parameterName} must not be null or whitespace.", parameterName);
        }
    }

    private static string Quote(string value)
        => "\"" + value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
}

internal readonly record struct HttpDisplay(string Text)
{
    public override string ToString() => Text;
}
