using System.Text.Json;
using Axiom.Core.Failures;
using Axiom.Json;

namespace Axiom.Http;

internal static class HttpJsonAssertions
{
    private const string ProblemJsonMediaType = "application/problem+json";

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson, nameof(expectedJson))));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHavePath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertNotHaveJsonPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to not have JSON path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertNotHavePath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonStringAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON string at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNumberAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        decimal expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON number at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNumberAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        double expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON number at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBooleanAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation(
            $"to have JSON boolean at path {path} equal to {JsonAssertionBridge.FormatValue(expectedValue)}",
            IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveBooleanAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonNullAtPath(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        JsonAssertionBridge.ValidatePath(path);
        var expectation = new Expectation($"to have JSON null at path {path}", IncludeExpectedValue: false);

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNullAtPath(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            path,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetails(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details", IncludeExpectedValue: false);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var titleFailure = JsonAssertionBridge.GetValueKindFailureDetailAtPath(bodyText, "$.title", JsonValueKind.String);
        if (titleFailure is not null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay(titleFailure),
                because,
                callerFilePath,
                callerLineNumber);
            return;
        }

        var statusFailure = JsonAssertionBridge.GetValueKindFailureDetailAtPath(bodyText, "$.status", JsonValueKind.Number);
        if (statusFailure is not null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay(statusFailure),
                because,
                callerFilePath,
                callerLineNumber);
        }
    }

    public static void AssertHaveProblemDetailsTitle(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedTitle,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details title", expectedTitle);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.title",
            expectedTitle,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsStatus(
        HttpResponseMessage? subject,
        string? subjectExpression,
        int expectedStatus,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details status", new HttpDisplay(expectedStatus.ToString()));
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveNumberAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.status",
            expectedStatus,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsType(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedType,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details type", expectedType);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.type",
            expectedType,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveProblemDetailsDetail(
        HttpResponseMessage? subject,
        string? subjectExpression,
        string expectedDetail,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation("to have problem details detail", expectedDetail);
        if (!TryGetProblemDetailsBody(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertHaveStringAtPath(
            bodyText,
            HttpAssertionSupport.ProblemDetailsSubjectLabel(subjectExpression),
            "$.detail",
            expectedDetail,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        JsonDocument expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson)));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    public static void AssertHaveJsonBodyEquivalentTo(
        HttpResponseMessage? subject,
        string? subjectExpression,
        JsonElement expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        var expectation = new Expectation(
            "to have JSON body equivalent to",
            new HttpDisplay(JsonAssertionBridge.DescribeExpectedJson(expectedJson)));

        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out var bodyText))
        {
            return;
        }

        JsonAssertionBridge.AssertBeEquivalentTo(
            bodyText,
            HttpAssertionSupport.JsonBodySubjectLabel(subjectExpression),
            expectedJson,
            because,
            callerFilePath,
            callerLineNumber);
    }

    private static bool TryGetBodyText(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out string bodyText)
    {
        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        if (subject is null)
        {
            bodyText = string.Empty;
            HttpAssertionSupport.Fail(subjectLabel, expectation, null, because, callerFilePath, callerLineNumber);
            return false;
        }

        var content = HttpAssertionSupport.ReadContent(subject);
        if (string.IsNullOrEmpty(content))
        {
            bodyText = string.Empty;
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay("no response content"),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        bodyText = content;
        return true;
    }

    private static bool TryGetProblemDetailsBody(
        HttpResponseMessage? subject,
        string? subjectExpression,
        Expectation expectation,
        string? because,
        string? callerFilePath,
        int callerLineNumber,
        out string bodyText)
    {
        if (!TryGetBodyText(subject, subjectExpression, expectation, because, callerFilePath, callerLineNumber, out bodyText))
        {
            return false;
        }

        var subjectLabel = HttpAssertionSupport.SubjectLabel(subjectExpression);
        var contentType = subject!.Content!.Headers.ContentType;
        if (contentType is null)
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                new HttpDisplay("no content type"),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        if (!string.Equals(contentType.MediaType, ProblemJsonMediaType, StringComparison.OrdinalIgnoreCase))
        {
            HttpAssertionSupport.Fail(
                subjectLabel,
                expectation,
                HttpAssertionSupport.DescribeContentType(contentType),
                because,
                callerFilePath,
                callerLineNumber);
            return false;
        }

        return true;
    }
}
