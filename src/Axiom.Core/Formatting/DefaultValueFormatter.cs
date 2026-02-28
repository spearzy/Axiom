using System.Globalization;

namespace Axiom.Core.Formatting;

public sealed class DefaultValueFormatter : IValueFormatter
{
    public static DefaultValueFormatter Instance { get; } = new();

    public string Format(object? value)
    {
        return value switch
        {
            null => "<null>",
            string text => $"\"{text}\"",
            char character => $"'{character}'",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? value.ToString() ?? "<null>",
            _ => value.ToString() ?? "<null>",
        };
    }
}
