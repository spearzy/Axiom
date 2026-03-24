namespace Axiom.Analyzers.Tests.Helpers;

internal static class AxiomAssertionStubs
{
    public const string Source =
        """
        using System;
        using System.Threading.Tasks;

        namespace Axiom.Assertions
        {
            public static class ShouldExtensions
            {
                public static Axiom.Assertions.AssertionTypes.AsyncActionAssertions Should(this Action subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.AsyncFunctionAssertions<T> Should<T>(this Func<Task<T>> subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.TaskAssertions Should(this Task subject) => throw null!;
                public static Axiom.Assertions.AssertionTypes.TaskAssertions<T> Should<T>(this Task<T> subject) => throw null!;
            }
        }

        namespace Axiom.Assertions.AssertionTypes
        {
            public sealed class AsyncActionAssertions
            {
                public ValueTask<ThrownExceptionAssertions<AsyncActionAssertions, TException>> ThrowAsync<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<AsyncActionAssertions>> NotThrowAsync() => default;
            }

            public sealed class AsyncFunctionAssertions<T>
            {
                public ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<T>, TException>> ThrowAsync<TException>() where TException : Exception => default;
                public ValueTask<ThrownExceptionAssertions<AsyncFunctionAssertions<T>, TException>> BeFaultedWith<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<AsyncFunctionAssertions<T>>> NotThrowAsync() => default;
                public ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<T>, T>> Succeed() => default;
                public ValueTask<SuccessfulTaskContinuation<AsyncFunctionAssertions<T>, T>> SucceedWithin(TimeSpan timeout) => default;
            }

            public sealed class TaskAssertions
            {
                public ValueTask<AndContinuation<TaskAssertions>> NotThrowAsync() => default;
            }

            public sealed class TaskAssertions<T>
            {
                public ValueTask<ThrownExceptionAssertions<TaskAssertions<T>, TException>> BeFaultedWith<TException>() where TException : Exception => default;
                public ValueTask<AndContinuation<TaskAssertions<T>>> NotThrowAsync() => default;
                public ValueTask<SuccessfulTaskContinuation<TaskAssertions<T>, T>> Succeed() => default;
                public ValueTask<SuccessfulTaskContinuation<TaskAssertions<T>, T>> SucceedWithin(TimeSpan timeout) => default;
            }

            public readonly struct AndContinuation<T> { }
            public readonly struct SuccessfulTaskContinuation<TParent, TResult> { }
            public readonly struct ThrownExceptionAssertions<TParent, TException> where TException : Exception { }
        }

        namespace Axiom.Core
        {
            public sealed class Batch : IDisposable
            {
                public Batch() { }
                public void Dispose() { }
            }

            public static class Assert
            {
                public static Batch Batch(string? name = null) => throw null!;
            }
        }
        """;
}
