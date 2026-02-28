namespace Axiom.Assertions.Chaining;

public readonly struct AndContinuation<TAssertions>
{
    // Holds the active assertion object so ".And" can continue the chain.
    private readonly TAssertions _assertions;

    public AndContinuation(TAssertions assertions)
    {
        _assertions = assertions;
    }

    public TAssertions And => _assertions;
}
