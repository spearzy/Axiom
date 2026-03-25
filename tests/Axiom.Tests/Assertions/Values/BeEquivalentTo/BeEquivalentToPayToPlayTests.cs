using System.Reflection;
using Axiom.Assertions.Equivalency;

namespace Axiom.Tests.Assertions.Values.BeEquivalentTo;

public sealed class BeEquivalentToPayToPlayTests : IDisposable
{
    public void Dispose()
    {
        ExpectedPathHelperProbe.Disable();
    }

    [Fact]
    public void GivenEquivalentUnmappedObjectGraph_WhenComparing_ThenExpectedPathHelpersAreNotUsed()
    {
        using var probe = ExpectedPathHelperProbe.Enable();

        var actual = new Person
        {
            Name = "Alice",
            Address = new Address { Postcode = "AB1 2CD" },
        };
        var expected = new Person
        {
            Name = "Alice",
            Address = new Address { Postcode = "AB1 2CD" },
        };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);

        var counts = probe.Snapshot();
        Assert.All(counts, count => Assert.Equal(0, count));
    }

    [Fact]
    public void GivenEquivalentUnmappedCollectionGraph_WhenComparing_ThenExpectedPathHelpersAreNotUsed()
    {
        using var probe = ExpectedPathHelperProbe.Enable();

        var actual = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };
        var expected = new Order
        {
            Items =
            [
                new() { Sku = "A-1", Quantity = 1 },
                new() { Sku = "B-2", Quantity = 2 },
            ]
        };

        var ex = Record.Exception(() => actual.Should().BeEquivalentTo(expected));

        Assert.Null(ex);

        var counts = probe.Snapshot();
        Assert.All(counts, count => Assert.Equal(0, count));
    }

    [Fact]
    public void GivenTypedMappings_WhenComparing_ThenExpectedPathHelpersAreUsed()
    {
        using var probe = ExpectedPathHelperProbe.Enable();

        var actual = new ActualUser
        {
            GivenName = "Alice",
            Address = new ActualAddress { Postcode = "AB1 2CD" },
        };
        var expected = new ExpectedUser
        {
            FirstName = "Alice",
            Location = new ExpectedLocation { ZipCode = "AB1 2CD" },
        };

        var ex = Record.Exception(() =>
            actual.Should().BeEquivalentTo(
                expected,
                options =>
                {
                    options.RequireStrictRuntimeTypes = false;
                    options.FailOnMissingMembers = false;
                    options.FailOnExtraMembers = false;
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.GivenName, x => x.FirstName);
                    options.MatchMember<ActualUser, ExpectedUser>(x => x.Address!.Postcode, x => x.Location!.ZipCode);
                }));

        Assert.Null(ex);

        var counts = probe.Snapshot();
        Assert.True(counts.Any(static count => count > 0), ExpectedPathHelperProbe.Format(counts));
    }

    private sealed class ExpectedPathHelperProbe : IDisposable
    {
        private static readonly Type EngineType =
            typeof(EquivalencyOptions).Assembly.GetType("Axiom.Assertions.Equivalency.EquivalencyEngine", throwOnError: true)!;

        private static readonly MethodInfo SetEnabledMethod =
            EngineType.GetMethod("SetExpectedPathHelperProbeEnabled", BindingFlags.Static | BindingFlags.NonPublic)!;

        private static readonly MethodInfo ResetMethod =
            EngineType.GetMethod("ResetExpectedPathHelperProbe", BindingFlags.Static | BindingFlags.NonPublic)!;

        private static readonly MethodInfo SnapshotMethod =
            EngineType.GetMethod("SnapshotExpectedPathHelperProbe", BindingFlags.Static | BindingFlags.NonPublic)!;

        public static ExpectedPathHelperProbe Enable()
        {
            Reset();
            SetEnabled(true);
            return new ExpectedPathHelperProbe();
        }

        public static void Disable()
        {
            SetEnabled(false);
            Reset();
        }

        public int[] Snapshot()
        {
            return (int[])SnapshotMethod.Invoke(obj: null, parameters: null)!;
        }

        public void Dispose()
        {
            Disable();
        }

        public static string Format(IReadOnlyList<int> counts)
        {
            return $"AppendPath={counts[0]}, AppendIndex={counts[1]}, ResolveExpectedMemberPath={counts[2]}, ResolveActualMemberPath={counts[3]}, GetDirectChildMemberName={counts[4]}";
        }

        private static void SetEnabled(bool enabled)
        {
            SetEnabledMethod.Invoke(obj: null, parameters: [enabled]);
        }

        private static void Reset()
        {
            ResetMethod.Invoke(obj: null, parameters: null);
        }
    }

    private sealed class Person
    {
        public string? Name { get; init; }
        public Address? Address { get; init; }
    }

    private sealed class Address
    {
        public string? Postcode { get; init; }
    }

    private sealed class Order
    {
        public List<LineItem> Items { get; init; } = [];
    }

    private sealed class LineItem
    {
        public string? Sku { get; init; }
        public int Quantity { get; init; }
    }

    private sealed class ActualUser
    {
        public string? GivenName { get; init; }
        public ActualAddress? Address { get; init; }
    }

    private sealed class ActualAddress
    {
        public string? Postcode { get; init; }
    }

    private sealed class ExpectedUser
    {
        public string? FirstName { get; init; }
        public ExpectedLocation? Location { get; init; }
    }

    private sealed class ExpectedLocation
    {
        public string? ZipCode { get; init; }
    }
}
