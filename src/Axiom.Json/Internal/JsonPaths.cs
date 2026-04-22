using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Axiom.Json;

internal sealed class JsonPath
{
    private JsonPath(JsonPathSegment[] segments, string displayPath)
    {
        Segments = segments;
        DisplayPath = displayPath;
    }

    public static string RootDisplayPath => "$";

    public JsonPathSegment[] Segments { get; }
    public string DisplayPath { get; }

    public static JsonPath Parse(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var trimmedPath = path.Trim();
        if (trimmedPath.Length == 0)
        {
            throw new ArgumentException("path must not be empty.", nameof(path));
        }

        var segments = new List<JsonPathSegment>();
        var displayBuilder = new StringBuilder(RootDisplayPath);
        var index = 0;

        if (trimmedPath[index] == '$')
        {
            index++;
            if (index == trimmedPath.Length)
            {
                return new JsonPath([], RootDisplayPath);
            }
        }

        while (index < trimmedPath.Length)
        {
            if (trimmedPath[index] == '.')
            {
                index++;
                if (index >= trimmedPath.Length)
                {
                    throw new ArgumentException("path must not end with '.'.", nameof(path));
                }
            }

            if (trimmedPath[index] == '[')
            {
                index++;
                var digitsStart = index;
                while (index < trimmedPath.Length && char.IsDigit(trimmedPath[index]))
                {
                    index++;
                }

                if (digitsStart == index || index >= trimmedPath.Length || trimmedPath[index] != ']')
                {
                    throw new ArgumentException("path contains an invalid array index segment.", nameof(path));
                }

                var arrayIndex = int.Parse(trimmedPath[digitsStart..index], CultureInfo.InvariantCulture);
                index++;
                var segment = JsonPathSegment.Index(arrayIndex);
                segments.Add(segment);
                displayBuilder.Append('[').Append(arrayIndex).Append(']');
                continue;
            }

            if (trimmedPath[index] == ']')
            {
                throw new ArgumentException("path contains an unexpected ']'.", nameof(path));
            }

            var nameStart = index;
            while (index < trimmedPath.Length && trimmedPath[index] is not '.' and not '[' and not ']')
            {
                index++;
            }

            if (nameStart == index)
            {
                throw new ArgumentException("path contains an empty property segment.", nameof(path));
            }

            var propertyName = trimmedPath[nameStart..index];
            var propertySegment = JsonPathSegment.Property(propertyName);
            segments.Add(propertySegment);
            displayBuilder.Append('.').Append(propertyName);
        }

        return new JsonPath([.. segments], displayBuilder.ToString());
    }

    public static string Append(string currentPath, JsonPathSegment segment)
        => segment.PropertyName is not null
            ? currentPath + "." + segment.PropertyName
            : currentPath + "[" + segment.ArrayIndex!.Value.ToString(CultureInfo.InvariantCulture) + "]";
}

internal readonly record struct JsonPathSegment(string? PropertyName, int? ArrayIndex)
{
    public static JsonPathSegment Property(string propertyName) => new(propertyName, null);

    public static JsonPathSegment Index(int arrayIndex) => new(null, arrayIndex);
}

internal readonly record struct JsonPathResolution(bool Success, JsonElement Value, string? FailureDetail)
{
    public static JsonPathResolution Succeeded(JsonElement value) => new(true, value, null);

    public static JsonPathResolution Failed(string detail) => new(false, default, detail);
}

internal static class JsonPathResolver
{
    public static JsonPathResolution ResolvePath(JsonElement root, JsonPath path)
    {
        var current = root;
        var currentPath = JsonPath.RootDisplayPath;

        foreach (var segment in path.Segments)
        {
            if (segment.PropertyName is not null)
            {
                var nextPath = JsonPath.Append(currentPath, segment);
                if (current.ValueKind != JsonValueKind.Object)
                {
                    return JsonPathResolution.Failed(
                        $"path traversal stopped at {currentPath}: expected Object but found {JsonAssertionSupport.FormatValueKind(current.ValueKind)}.");
                }

                if (!current.TryGetProperty(segment.PropertyName, out current))
                {
                    return JsonPathResolution.Failed($"missing path {nextPath}");
                }

                currentPath = nextPath;
                continue;
            }

            var arrayIndex = segment.ArrayIndex!.Value;
            var indexedPath = JsonPath.Append(currentPath, segment);
            if (current.ValueKind != JsonValueKind.Array)
            {
                return JsonPathResolution.Failed(
                    $"path traversal stopped at {currentPath}: expected Array but found {JsonAssertionSupport.FormatValueKind(current.ValueKind)}.");
            }

            if (arrayIndex < 0 || arrayIndex >= current.GetArrayLength())
            {
                return JsonPathResolution.Failed($"missing path {indexedPath}");
            }

            var elementIndex = 0;
            JsonElement? match = null;
            foreach (var item in current.EnumerateArray())
            {
                if (elementIndex == arrayIndex)
                {
                    match = item;
                    break;
                }

                elementIndex++;
            }

            current = match!.Value;
            currentPath = indexedPath;
        }

        return JsonPathResolution.Succeeded(current);
    }
}
