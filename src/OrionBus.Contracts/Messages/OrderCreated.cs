namespace OrionBus.Contracts.Messages;

public sealed record OrderCreated(
    string OrderId,
    string Sku,
    int Quantity,
    decimal UnitPrice,
    DateTimeOffset CreatedAtUtc);
