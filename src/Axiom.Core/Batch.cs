using Axiom.Core.Failures;
using Axiom.Core.Output;

namespace Axiom.Core;

public sealed class Batch : IDisposable
{
    // One ambient "current batch" per async flow.
    private static readonly AsyncLocal<Batch?> CurrentBatch = new();
    // Captures parent batch so nested scopes can restore/merge correctly.
    private readonly Batch? _previous;
    private List<string>? _failures;
    private bool _disposed;

    public Batch(string? name = null)
    {
        Name = name;
        _previous = CurrentBatch.Value;
        CurrentBatch.Value = this;
    }

    public string? Name { get; }

    public static Batch? Current => CurrentBatch.Value;

    public void AddFailure(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        (_failures ??= new List<string>()).Add(message);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        // Always restore parent context first, even if we throw below.
        // This prevents leaking the disposed batch into later assertions.
        CurrentBatch.Value = _previous;

        if (_failures is null || _failures.Count == 0)
        {
            AssertionOutputWriter.ReportPass("Batch", Name is { Length: > 0 } ? Name : "<batch>", null, 0);
            return;
        }

        if (_previous is not null)
        {
            // Nested batches contribute failures upward instead of throwing.
            // Only the root batch throws a combined exception.
            _previous.AddFailures(_failures);
            return;
        }

        var report = BatchReportRenderer.Render(Name, _failures);
        AssertionOutputWriter.ReportFailure(report, null, 0);
        throw new InvalidOperationException(report);
    }

    private void AddFailures(List<string> failures)
    {
        if (failures.Count == 0)
        {
            return;
        }

        _failures ??= new List<string>(failures.Count);
        _failures.AddRange(failures);
    }

}
