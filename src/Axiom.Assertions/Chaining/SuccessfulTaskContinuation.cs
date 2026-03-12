using System.Diagnostics;
using Axiom.Core.Failures;

namespace Axiom.Assertions.Chaining;

public readonly struct SuccessfulTaskContinuation<TAssertions, TResult>
{
    private readonly bool _hasResult;
    //Cant replace with field keyword for backwards compatability with older C# versions
    private readonly TResult? _result;
    private readonly string? _successFailureMessage;

    public SuccessfulTaskContinuation(
        TAssertions assertions,
        bool hasResult,
        TResult? result,
        string? successFailureMessage)
    {
        And = assertions;
        _hasResult = hasResult;
        _result = result;
        _successFailureMessage = successFailureMessage;
    }

    public TAssertions And { get; }

    public TResult WhoseResult
    {
        get
        {
            if (_hasResult)
            {
                return _result!;
            }

            var message = _successFailureMessage is null
                ? "WhoseResult is unavailable because Succeed failed."
                : $"WhoseResult is unavailable because Succeed failed with error: {_successFailureMessage}";
            AssertionFailureDispatcher.Throw(message);
            throw new UnreachableException();
        }
    }

    public TResult Which => WhoseResult;
}
