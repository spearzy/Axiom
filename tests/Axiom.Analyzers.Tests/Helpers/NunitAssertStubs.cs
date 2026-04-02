namespace Axiom.Analyzers.Tests.Helpers;

internal static class NunitAssertStubs
{
    public const string Source =
        """
        using System;
        using System.Collections.Generic;

        namespace NUnit.Framework
        {
            using Constraints;

            public static class Assert
            {
                public static void That<TActual>(TActual actual, IResolveConstraint expression) { }
                public static void That<TActual>(TActual actual, IResolveConstraint expression, string message) { }
            }

            public static class Is
            {
                public static ConstraintExpression Not => default!;
                public static NullConstraint Null => default!;
                public static TrueConstraint True => default!;
                public static FalseConstraint False => default!;
                public static EmptyConstraint Empty => default!;
                public static EqualConstraint EqualTo(object? expected) => default!;
                public static EqualConstraint EqualTo(string expected) => default!;
                public static EqualConstraint EqualTo<T>(IEnumerable<T> expected) => default!;
            }

            public static class Does
            {
                public static ContainsConstraint Contain(string expected) => default!;
            }

            public static class Has
            {
                public static CountConstraintExpression Count => default!;
            }
        }

        namespace NUnit.Framework.Constraints
        {
            public interface IResolveConstraint { }

            public class ConstraintExpression : IResolveConstraint
            {
                public ConstraintExpression Not => default!;
                public NullConstraint Null => default!;
                public TrueConstraint True => default!;
                public FalseConstraint False => default!;
                public EmptyConstraint Empty => default!;
                public EqualConstraint EqualTo(object? expected) => default!;
                public EqualConstraint EqualTo(string expected) => default!;
            }

            public class EqualConstraint : ConstraintExpression
            {
                public EqualConstraint Using(object comparer) => this;
                public EqualConstraint Within(double tolerance) => this;
            }

            public sealed class NullConstraint : ConstraintExpression { }
            public sealed class TrueConstraint : ConstraintExpression { }
            public sealed class FalseConstraint : ConstraintExpression { }
            public sealed class EmptyConstraint : ConstraintExpression { }
            public sealed class ContainsConstraint : ConstraintExpression
            {
                public ContainsConstraint IgnoreCase => this;
            }
            public sealed class CountConstraintExpression : ConstraintExpression
            {
                public EqualConstraint EqualTo(int expected) => default!;
            }
        }
        """;
}
