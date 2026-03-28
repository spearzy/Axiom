namespace Axiom.Benchmarks.Models;

public sealed record OrderSnapshot(
    string OrderId,
    CustomerSnapshot Customer,
    AddressSnapshot ShippingAddress,
    PaymentSnapshot Payment,
    LineItemSnapshot[] Items,
    decimal Subtotal,
    decimal Tax,
    decimal Total,
    DateTime SubmittedAtUtc,
    string Status);

public sealed record CustomerSnapshot(
    string Id,
    string Name,
    string Email,
    string Tier,
    AddressSnapshot Address);

public sealed record AddressSnapshot(
    string Line1,
    string City,
    string Region,
    string Postcode,
    string CountryCode);

public sealed record PaymentSnapshot(
    string Method,
    string Currency,
    decimal AuthorizedAmount,
    DateTime AuthorizedAtUtc);

public sealed record LineItemSnapshot(
    string Sku,
    string Name,
    int Quantity,
    decimal UnitPrice,
    string FulfillmentChannel);

public sealed record BatchUserSnapshot(
    string Name,
    int Age,
    string Email,
    bool IsActive,
    string CountryCode);
