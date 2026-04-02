namespace Axiom.Analyzers.Tests.Helpers;

internal static class MstestAssertStubs
{
    public const string Source =
        """
        namespace Microsoft.VisualStudio.TestTools.UnitTesting
        {
            public static class Assert
            {
                public static void AreEqual<T>(T expected, T actual) { }
                public static void AreEqual<T>(T expected, T actual, string message) { }
                public static void AreEqual(double expected, double actual, double delta) { }
                public static void AreEqual(string? expected, string? actual, bool ignoreCase) { }
                public static void AreNotEqual<T>(T expected, T actual) { }
                public static void AreNotEqual<T>(T expected, T actual, string message) { }
                public static void AreNotEqual(double expected, double actual, double delta) { }
                public static void AreNotEqual(string? expected, string? actual, bool ignoreCase) { }
                public static void IsNull(object? value) { }
                public static void IsNull(object? value, string message) { }
                public static void IsNotNull(object? value) { }
                public static void IsNotNull(object? value, string message) { }
                public static void IsTrue(bool condition) { }
                public static void IsTrue(bool condition, string message) { }
                public static void IsFalse(bool condition) { }
                public static void IsFalse(bool condition, string message) { }
                public static void AreSame(object? expected, object? actual) { }
                public static void AreSame(object? expected, object? actual, string message) { }
                public static void AreNotSame(object? expected, object? actual) { }
                public static void AreNotSame(object? expected, object? actual, string message) { }
            }
        }
        """;
}
