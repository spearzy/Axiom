namespace Axiom.Assertions.Chaining;

public readonly struct ContainSingleContinuation<TAssertions>
{
    private readonly bool _hasSingleItem;
    private readonly object? _singleItem;
    private readonly string? _containSingleFailureMessage;

    public ContainSingleContinuation(
        TAssertions assertions,
        bool hasSingleItem,
        object? singleItem,
        string? containSingleFailureMessage)
    {
        And = assertions;
        _hasSingleItem = hasSingleItem;
        _singleItem = singleItem;
        _containSingleFailureMessage = containSingleFailureMessage;
    }

    public TAssertions And { get; }

    public object? SingleItem
    {
        get
        {
            if (_hasSingleItem)
            {
                return _singleItem;
            }

            var message = _containSingleFailureMessage is null
                ? "SingleItem is unavailable because ContainSingle failed."
                : $"SingleItem is unavailable because ContainSingle failed with error: {_containSingleFailureMessage}";
            throw new InvalidOperationException(message);
        }
    }
}
