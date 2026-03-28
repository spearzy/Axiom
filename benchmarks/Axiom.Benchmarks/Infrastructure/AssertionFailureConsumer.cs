using System.Runtime.CompilerServices;

namespace Axiom.Benchmarks.Infrastructure;

internal static class AssertionFailureConsumer
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ConsumeExpectedFailure(Action assertion)
    {
        try
        {
            assertion();
        }
        catch (InvalidOperationException)
        {
        }
    }
}
