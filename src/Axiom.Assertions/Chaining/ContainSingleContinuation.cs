using System.Diagnostics;
using Axiom.Core.Failures;

namespace Axiom.Assertions.Chaining;

public readonly struct ContainSingleContinuation<TAssertions>
{
    private readonly bool _hasSingleItem;
    private readonly object? _singleItem;
    private readonly string? _containSingleFailureMessage;
    private readonly string _assertionName;

    public ContainSingleContinuation(
        TAssertions assertions,
        bool hasSingleItem,
        object? singleItem,
        string? containSingleFailureMessage,
        string assertionName = "ContainSingle")
    {
        And = assertions;
        _hasSingleItem = hasSingleItem;
        _singleItem = singleItem;
        _containSingleFailureMessage = containSingleFailureMessage;
        _assertionName = assertionName;
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
                ? $"SingleItem is unavailable because {_assertionName} failed."
                : $"SingleItem is unavailable because {_assertionName} failed with error: {_containSingleFailureMessage}";
            AssertionFailureDispatcher.Throw(message);
            throw new UnreachableException();
        }
    }
}

public readonly struct ContainSingleContinuation<TAssertions, TItem>
{
    private readonly bool _hasSingleItem;
    private readonly TItem _singleItem;
    private readonly string? _containSingleFailureMessage;
    private readonly string _assertionName;

    public ContainSingleContinuation(
        TAssertions assertions,
        bool hasSingleItem,
        TItem singleItem,
        string? containSingleFailureMessage,
        string assertionName = "ContainSingle")
    {
        And = assertions;
        _hasSingleItem = hasSingleItem;
        _singleItem = singleItem;
        _containSingleFailureMessage = containSingleFailureMessage;
        _assertionName = assertionName;
    }

    public TAssertions And { get; }

    public TItem SingleItem
    {
        get
        {
            if (_hasSingleItem)
            {
                return _singleItem;
            }

            var message = _containSingleFailureMessage is null
                ? $"SingleItem is unavailable because {_assertionName} failed."
                : $"SingleItem is unavailable because {_assertionName} failed with error: {_containSingleFailureMessage}";
            AssertionFailureDispatcher.Throw(message);
            throw new UnreachableException();
        }
    }
}
