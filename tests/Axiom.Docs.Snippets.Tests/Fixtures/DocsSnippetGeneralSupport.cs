using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Axiom.Vectors;

// Shared sample data and helper types used to compile literal docs examples against the real Axiom assemblies.
public static class DocsSnippetFixtures
{
    public static UserProfile ActualUserProfile { get; } = new()
    {
        Name = "Bob",
        Email = "bob@example.com",
        Address = new UserAddress
        {
            Postcode = "SW1A 1AA",
            CountryCode = "GB",
        },
    };

    public static UserProfile ExpectedUserProfile { get; } = new()
    {
        Name = "Bob",
        Email = "bob@example.com",
        Address = new UserAddress
        {
            Postcode = "SW1A 1AA",
            CountryCode = "GB",
        },
    };

    public static object Value { get; } = new();

    public static User User { get; } = new()
    {
        Id = 42,
        Name = "Bob",
        Email = "bob@example.com",
        Age = 36,
        Roles = ["admin", "finance"],
    };

    public static Invoice Invoice { get; } = new()
    {
        Currency = "GBP",
        DueDate = new DateOnly(2026, 3, 1),
    };

    public static DateOnly Today { get; } = new(2026, 4, 2);

    public static ApiResponse Response { get; } = new()
    {
        StatusCode = 200,
        ErrorCode = null,
    };

    public static ApiResponse FailedResponse { get; } = new()
    {
        StatusCode = 404,
        ErrorCode = "ORDER_NOT_FOUND",
    };

    public static Order Order { get; } = new()
    {
        Total = 129.48m,
        Code = "ORD-1",
        Items =
        [
            new LineItem { Sku = "SKU-1" },
            new LineItem { Sku = "SKU-2" },
        ],
    };

    public static List<string> TextValues { get; } = ["expected", "other"];

    public static List<int> IntegerValues { get; } = [1, 2, 3];

    public static int[] IntegerArray { get; } = [1, 2, 3];

    public static List<dynamic> DynamicValues { get; } = [1, "expected", 3];

    public static List<DocsMigrationValue> MigrationValues { get; } = [1, "expected", 3];

    public static Dictionary<int, dynamic> DynamicLookup { get; } = new()
    {
        [1] = "one",
        [2] = 2,
    };

    public static Dictionary<int, string> StringLookup { get; } = new()
    {
        [1] = "one",
        [2] = "two",
    };

    public static IAsyncEnumerable<int> StepIds { get; } = GetStepIds();

    public static float[] Embedding { get; } = [1f, 0f, 0f];

    public static float[] ExpectedEmbedding { get; } = [1f, 0f, 0f];

    public static float[] UnrelatedEmbedding { get; } = [0f, 1f, 0f];

    public static string[] RankedResults { get; } = ["doc-2", "doc-7", "doc-5", "doc-9"];

    public static string[] RelevantDocuments { get; } = ["doc-2", "doc-5"];

    public static RankingQuery<string>[] RankingQueries { get; } =
    [
        new(["doc-2", "doc-7", "doc-5"], ["doc-2"]),
        new(["doc-8", "doc-5", "doc-3"], ["doc-5"]),
    ];

    public static async IAsyncEnumerable<Order> GetOrders()
    {
        yield return new Order { Total = 10m };
        await Task.Yield();
        yield return new Order { Total = 20m };
    }

    public static async IAsyncEnumerable<User> GetUsers()
    {
        yield return new User { Id = 1, Email = "bob@example.com", Name = "Bob" };
        await Task.Yield();
        yield return new User { Id = 2, Email = "alice@example.com", Name = "Alice" };
    }

    public static async IAsyncEnumerable<WorkflowStep> GetStatuses()
    {
        yield return WorkflowStep.Started;
        await Task.Yield();
        yield return WorkflowStep.Completed;
    }

    public static async IAsyncEnumerable<WorkflowEvent> GetEvents()
    {
        yield return new WorkflowEvent { Step = WorkflowStep.Started };
        await Task.Yield();
        yield return new WorkflowEvent { Step = WorkflowStep.Completed };
    }

    private static async IAsyncEnumerable<int> GetStepIds()
    {
        yield return 1;
        await Task.Yield();
        yield return 2;
        yield return 3;
    }
}

public readonly struct DocsMigrationValue
{
    private readonly object _value;

    public DocsMigrationValue(object value)
    {
        _value = value;
    }

    public static implicit operator DocsMigrationValue(int value) => new(value);

    public static implicit operator DocsMigrationValue(string value) => new(value);

    public static implicit operator string(DocsMigrationValue value) => value._value?.ToString() ?? string.Empty;

    public static bool operator >(DocsMigrationValue value, int threshold) => value.AsInt32() > threshold;

    public static bool operator <(DocsMigrationValue value, int threshold) => value.AsInt32() < threshold;

    public override string ToString() => _value?.ToString() ?? string.Empty;

    private int AsInt32()
    {
        return _value switch
        {
            int number => number,
            string text when int.TryParse(text, out var parsed) => parsed,
            _ => 0,
        };
    }
}

public sealed class User
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int Age { get; init; }
    public List<string> Roles { get; init; } = [];
}

public sealed class UserProfile
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserAddress Address { get; init; } = new();
}

public sealed class UserAddress
{
    public string Postcode { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
}

public sealed class Invoice
{
    public string Currency { get; init; } = string.Empty;
    public DateOnly DueDate { get; init; }
}

public sealed class ApiResponse
{
    public int StatusCode { get; init; }
    public string? ErrorCode { get; init; }
}

public sealed class Order
{
    public decimal Total { get; init; }
    public string Code { get; init; } = string.Empty;
    public List<LineItem> Items { get; init; } = [];
}

public sealed class LineItem
{
    public string Sku { get; init; } = string.Empty;
}

public sealed class WorkflowEvent
{
    public WorkflowStep Step { get; init; }
}

public enum WorkflowStep
{
    Started,
    Completed,
}

public enum OrderStatus
{
    Submitted,
}

public sealed class Person
{
    public string Name { get; init; } = string.Empty;
}

public sealed class ActualUser
{
    public string GivenName { get; init; } = string.Empty;
    public ActualAddress Address { get; init; } = new();
}

public sealed class ActualAddress
{
    public string Postcode { get; init; } = string.Empty;
}

public sealed class ExpectedUser
{
    public string FirstName { get; init; } = string.Empty;
    public ExpectedLocation Location { get; init; } = new();
}

public sealed class ExpectedLocation
{
    public string ZipCode { get; init; } = string.Empty;
}

namespace Axiom.Assertions.Extensions
{
    public static class DocsSnippetCollectionValueAssertionExtensions
    {
        // This keeps the migration gallery binding the same compact predicate form shown in the docs.
        public static Axiom.Assertions.Chaining.ContainSingleContinuation<Axiom.Assertions.AssertionTypes.ValueAssertions<List<TItem>>, TItem> ContainSingle<TItem>(
            this Axiom.Assertions.AssertionTypes.ValueAssertions<List<TItem>> assertions,
            Func<TItem, bool> predicate,
            string? because = null,
            [CallerFilePath] string? callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            return Axiom.Assertions.Extensions.CollectionValueAssertionExtensions.ContainSingle<List<TItem>, TItem>(
                assertions,
                predicate,
                because,
                callerFilePath,
                callerLineNumber);
        }
    }
}

public sealed class OrderSnapshot
{
    public int Id { get; init; }
    public decimal Total { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public SnapshotMetadata Metadata { get; init; } = new();
}

public sealed class SnapshotMetadata
{
    public string RequestId { get; init; } = string.Empty;
}

public sealed class OddEvenMatchIntComparer : IEqualityComparer<int>
{
    public bool Equals(int x, int y) => x % 2 == y % 2;

    public int GetHashCode(int obj) => obj % 2;
}

public sealed class LineItemSkuComparer : IEqualityComparer, IEqualityComparer<LineItem>
{
    public new bool Equals(object? x, object? y)
    {
        return Equals(x as LineItem, y as LineItem);
    }

    public int GetHashCode(object obj)
    {
        return obj is LineItem lineItem ? GetHashCode(lineItem) : 0;
    }

    public bool Equals(LineItem? x, LineItem? y)
    {
        return string.Equals(x?.Sku, y?.Sku, StringComparison.Ordinal);
    }

    public int GetHashCode(LineItem obj)
    {
        return StringComparer.Ordinal.GetHashCode(obj.Sku);
    }
}
