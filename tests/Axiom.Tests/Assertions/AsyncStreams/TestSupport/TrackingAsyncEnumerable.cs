namespace Axiom.Tests.Assertions.AsyncStreams.TestSupport;

internal sealed class TrackingAsyncEnumerable<T>(IReadOnlyList<T> items) : IAsyncEnumerable<T>
{
    public int MoveNextCallCount { get; private set; }
    public int YieldCount { get; private set; }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new Enumerator(this, items);
    }

    private sealed class Enumerator(TrackingAsyncEnumerable<T> owner, IReadOnlyList<T> items) : IAsyncEnumerator<T>
    {
        private int _index = -1;

        public T Current { get; private set; } = default!;

        public ValueTask<bool> MoveNextAsync()
        {
            owner.MoveNextCallCount++;
            _index++;

            if (_index >= items.Count)
            {
                return ValueTask.FromResult(false);
            }

            owner.YieldCount++;
            Current = items[_index];
            return ValueTask.FromResult(true);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
