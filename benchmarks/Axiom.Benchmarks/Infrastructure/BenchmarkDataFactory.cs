using Axiom.Benchmarks.Models;

namespace Axiom.Benchmarks.Infrastructure;

internal static class BenchmarkDataFactory
{
    public static OrderSnapshot CreateEquivalentOrder()
    {
        return new OrderSnapshot(
            OrderId: "ORD-1024",
            Customer: new CustomerSnapshot(
                Id: "CUS-001",
                Name: "Ada Lovelace",
                Email: "ada@example.test",
                Tier: "Gold",
                Address: new AddressSnapshot(
                    Line1: "1 Analytical Engine Way",
                    City: "London",
                    Region: "Greater London",
                    Postcode: "SW1A 1AA",
                    CountryCode: "GB")),
            ShippingAddress: new AddressSnapshot(
                Line1: "1 Analytical Engine Way",
                City: "London",
                Region: "Greater London",
                Postcode: "SW1A 1AA",
                CountryCode: "GB"),
            Payment: new PaymentSnapshot(
                Method: "Card",
                Currency: "GBP",
                AuthorizedAmount: 129.48m,
                AuthorizedAtUtc: new DateTime(2026, 03, 15, 12, 30, 00, DateTimeKind.Utc)),
            Items:
            [
                new LineItemSnapshot("BK-001", "Notebook", 2, 12.50m, "Warehouse"),
                new LineItemSnapshot("KB-101", "Mechanical Keyboard", 1, 89.99m, "Warehouse"),
                new LineItemSnapshot("MS-220", "Wireless Mouse", 1, 19.99m, "Dropship")
            ],
            Subtotal: 134.98m,
            Tax: -5.50m,
            Total: 129.48m,
            SubmittedAtUtc: new DateTime(2026, 03, 15, 12, 29, 55, DateTimeKind.Utc),
            Status: "Submitted");
    }

    public static OrderSnapshot CreateOrderWithNestedMismatch()
    {
        var baseline = CreateEquivalentOrder();

        return baseline with
        {
            Customer = baseline.Customer with
            {
                Address = baseline.Customer.Address with
                {
                    Postcode = "EC1A 1BB"
                }
            }
        };
    }

    public static BatchUserSnapshot CreateBatchUser()
    {
        return new BatchUserSnapshot(
            Name: "Ada",
            Age: 36,
            Email: "ada@example.test",
            IsActive: true,
            CountryCode: "GB");
    }
}
