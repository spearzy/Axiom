using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Axiom.Assertions.Chaining;
using Axiom.Core.Configuration;
using Axiom.Core.Failures;

namespace Axiom.Assertions.AssertionTypes;

public sealed class StringAssertions(string? subject, string? subjectExpression)
{
    public string? Subject { get; } = subject;
    public string? SubjectExpression { get; } = subjectExpression;

    public AndContinuation<StringAssertions> NotBeNull(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be null", IncludeExpectedValue: false),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeNull(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (Subject is not null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be null", IncludeExpectedValue: false),
                Subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> StartWith(
        string expectedPrefix,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return StartWith(expectedPrefix, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> StartWith(
        string expectedPrefix,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.StartsWith(expectedPrefix, comparison))
        {
            var detail = StringDifferenceDiagnostics.BuildStartWithFailureDetail(expectedPrefix, subject);
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to start with", expectedPrefix),
                RenderStringActualWithDetail(subject, detail),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> EndWith(
        string expectedSuffix,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return EndWith(expectedSuffix, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> EndWith(
        string expectedSuffix,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.EndsWith(expectedSuffix, comparison))
        {
            var detail = StringDifferenceDiagnostics.BuildEndWithFailureDetail(expectedSuffix, subject);
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to end with", expectedSuffix),
                RenderStringActualWithDetail(subject, detail),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> Contain(
        string expectedSubstring,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return Contain(expectedSubstring, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> Contain(
        string expectedSubstring,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to contain", expectedSubstring),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!subject.Contains(expectedSubstring, comparison))
        {
            var detail = StringDifferenceDiagnostics.BuildContainFailureDetail(expectedSubstring, subject);
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to contain", expectedSubstring),
                RenderStringActualWithDetail(subject, detail),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotContain(
        string unexpectedSubstring,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotContain(unexpectedSubstring, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> NotContain(
        string unexpectedSubstring,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not contain", unexpectedSubstring),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (subject.Contains(unexpectedSubstring, comparison))
        {
            var matchIndex = subject.IndexOf(unexpectedSubstring, comparison);
            var detail = StringDifferenceDiagnostics.BuildNotContainFailureDetail(unexpectedSubstring, subject, matchIndex);
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not contain", unexpectedSubstring),
                RenderStringActualWithDetail(subject, detail),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> HaveLength(
        int expectedLength,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        if (expectedLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedLength), "expectedLength must be non-negative.");
        }

        var subject = Subject;
        if (subject is null || subject.Length != expectedLength)
        {
            object? actual = subject is null ? null : subject.Length;
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to have length", expectedLength),
                actual,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (subject is null || subject.Length != 0)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be empty", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotBeEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (string.IsNullOrEmpty(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be empty", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeNullOrEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (!string.IsNullOrEmpty(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be null or empty", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotBeNullOrEmpty(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (string.IsNullOrEmpty(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be null or empty", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeEquivalentTo(
        string expected,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (!string.Equals(subject, expected, comparison))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be equivalent to", expected),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> BeNullOrWhiteSpace(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        // Null, empty, and white-space-only values all satisfy this assertion.
        if (!string.IsNullOrWhiteSpace(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be null or white-space", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotBeNullOrWhiteSpace(
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (string.IsNullOrWhiteSpace(subject))
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be null or white-space", IncludeExpectedValue: false),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> Match(
        string pattern,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return MatchCore(pattern, timeoutOverride: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> Match(
        string pattern,
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return MatchCore(pattern, timeout, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> Be(
        string expected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return Be(expected, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> Be(
        string expected,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (!string.Equals(subject, expected, comparison))
        {
            object? actual = subject;
            if (subject is not null)
            {
                var detail = StringDifferenceDiagnostics.BuildEqualityFailureDetail(expected, subject);
                actual = RenderStringActualWithDetail(subject, IncludeComparisonInDetail(detail, comparison));
            }

            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to be", expected),
                actual,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }

        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotBe(
        string unexpected,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotBe(unexpected, StringComparison.Ordinal, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> NotBe(
        string unexpected,
        StringComparison comparison,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        var subject = Subject;
        if (string.Equals(subject, unexpected, comparison))
        {
            object? actual = subject;
            if (subject is not null)
            {
                var detail = StringDifferenceDiagnostics.BuildEqualityFailureDetail(unexpected, subject);
                actual = RenderStringActualWithDetail(subject, IncludeComparisonInDetail(detail, comparison));
            }

            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not be", unexpected),
                actual,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    private AndContinuation<StringAssertions> MatchCore(
        string pattern,
        TimeSpan? timeoutOverride,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        var regexTimeout = ResolveRegexTimeout(timeoutOverride);

        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to match regex", pattern),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!TryEvaluateRegex(subject, pattern, regexTimeout, out var isMatch))
        {
            var timeoutFailure = new Failure(
                SubjectLabel(),
                new Expectation("to match regex", pattern),
                new RenderedText($"regex evaluation timed out after {regexTimeout.TotalMilliseconds:0} ms"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(timeoutFailure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!isMatch)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to match regex", pattern),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    public AndContinuation<StringAssertions> NotMatch(
        string pattern,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotMatchCore(pattern, timeoutOverride: null, because, callerFilePath, callerLineNumber);
    }

    public AndContinuation<StringAssertions> NotMatch(
        string pattern,
        TimeSpan timeout,
        string? because = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int callerLineNumber = 0)
    {
        return NotMatchCore(pattern, timeout, because, callerFilePath, callerLineNumber);
    }

    private AndContinuation<StringAssertions> NotMatchCore(
        string pattern,
        TimeSpan? timeoutOverride,
        string? because,
        string? callerFilePath,
        int callerLineNumber)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        var regexTimeout = ResolveRegexTimeout(timeoutOverride);

        var subject = Subject;
        if (subject is null)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not match regex", pattern),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (!TryEvaluateRegex(subject, pattern, regexTimeout, out var isMatch))
        {
            var timeoutFailure = new Failure(
                SubjectLabel(),
                new Expectation("to not match regex", pattern),
                new RenderedText($"regex evaluation timed out after {regexTimeout.TotalMilliseconds:0} ms"),
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(timeoutFailure), callerFilePath, callerLineNumber);
            return new AndContinuation<StringAssertions>(this);
        }

        if (isMatch)
        {
            var failure = new Failure(
                SubjectLabel(),
                new Expectation("to not match regex", pattern),
                subject,
                because);
            AssertionFailureDispatcher.Fail(FailureMessageRenderer.Render(failure), callerFilePath, callerLineNumber);
        }
        return new AndContinuation<StringAssertions>(this);
    }

    private string SubjectLabel()
    {
        return string.IsNullOrWhiteSpace(SubjectExpression) ? "<subject>" : SubjectExpression;
    }

    private static RenderedText RenderStringActualWithDetail(string actual, string detail)
    {
        return new RenderedText($"\"{EscapeForMessage(actual)}\" ({detail})");
    }

    private static string IncludeComparisonInDetail(string detail, StringComparison comparison)
    {
        if (comparison == StringComparison.Ordinal)
        {
            return detail;
        }

        return $"comparison {comparison}; {detail}";
    }

    private static string EscapeForMessage(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private static TimeSpan ResolveRegexTimeout(TimeSpan? timeoutOverride)
    {
        if (timeoutOverride.HasValue)
        {
            return ValidateRegexTimeout(timeoutOverride.Value, "timeout");
        }

        return ValidateRegexTimeout(AxiomServices.Configuration.RegexMatchTimeout, nameof(AxiomConfiguration.RegexMatchTimeout));
    }

    private static TimeSpan ValidateRegexTimeout(TimeSpan timeout, string paramName)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(paramName, "Regex timeout must be greater than zero.");
        }

        return timeout;
    }

    private static bool TryEvaluateRegex(string subject, string pattern, TimeSpan timeout, out bool isMatch)
    {
        try
        {
            isMatch = Regex.IsMatch(
                subject,
                pattern,
                RegexOptions.CultureInvariant,
                timeout);
            return true;
        }
        catch (RegexMatchTimeoutException)
        {
            isMatch = false;
            return false;
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Invalid regex pattern '{pattern}'.", nameof(pattern), ex);
        }
    }

    private readonly record struct RenderedText(string Text)
    {
        public override string ToString()
        {
            return Text;
        }
    }
}
