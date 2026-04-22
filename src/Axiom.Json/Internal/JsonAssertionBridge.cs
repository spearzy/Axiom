using System.Globalization;
using System.Text.Json;

namespace Axiom.Json;

internal static class JsonAssertionBridge
{
    public static void ValidatePath(string path) => _ = JsonPath.Parse(path);

    public static string FormatValue(object? value) => JsonAssertionSupport.FormatValue(value);

    public static string DescribeExpectedJson(string expectedJson, string argumentName)
    {
        using var expected = JsonParsedValue.ParseExpected(JsonInput.FromString(expectedJson), argumentName);
        return expected.DisplayText;
    }

    public static string DescribeExpectedJson(JsonDocument expectedJson)
        => JsonSerializer.Serialize(expectedJson.RootElement);

    public static string DescribeExpectedJson(JsonElement expectedJson)
        => JsonSerializer.Serialize(expectedJson);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        string expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromString(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        JsonDocument expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromDocument(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertBeEquivalentTo(
        string subjectJson,
        string subjectLabel,
        JsonElement expectedJson,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertBeJsonEquivalentTo(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            JsonInput.FromElement(expectedJson),
            nameof(expectedJson),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHavePath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertNotHavePath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertNotHaveJsonPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveStringAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonStringAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        decimal expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            JsonNumberExpectation.FromDecimal(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        double expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            JsonNumberExpectation.FromDouble(expectedValue),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNumberAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        int expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNumberAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            new JsonNumberExpectation(
                expectedValue.ToString(CultureInfo.InvariantCulture),
                expectedValue.ToString(CultureInfo.InvariantCulture)),
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveBooleanAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        bool expectedValue,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonBooleanAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            expectedValue,
            because,
            callerFilePath,
            callerLineNumber);

    public static void AssertHaveNullAtPath(
        string subjectJson,
        string subjectLabel,
        string path,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
        => JsonAssertionEngine.AssertHaveJsonNullAtPath(
            JsonInput.FromString(subjectJson),
            subjectLabel,
            path,
            because,
            callerFilePath,
            callerLineNumber);

    public static string? GetValueKindFailureDetailAtPath(
        string subjectJson,
        string path,
        JsonValueKind expectedKind)
    {
        using var parsed = JsonParsedValue.ParseSubject(JsonInput.FromString(subjectJson));

        if (!parsed.IsValid)
        {
            return parsed.InvalidDetail!;
        }

        var resolution = JsonPathResolver.ResolvePath(parsed.Root, JsonPath.Parse(path));
        if (!resolution.Success)
        {
            return resolution.FailureDetail!;
        }

        if (resolution.Value.ValueKind == expectedKind)
        {
            return null;
        }

        return JsonAssertionSupport.DescribeWrongValueKind(
            resolution.Value,
            path,
            JsonAssertionSupport.FormatValueKind(expectedKind));
    }
}
