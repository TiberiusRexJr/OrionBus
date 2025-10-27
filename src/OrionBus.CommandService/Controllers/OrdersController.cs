using Microsoft.AspNetCore.Mvc;
using OrionBus.CommandService.Services;
using OrionBus.Contracts.Messages;

namespace OrionBus.CommandService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ServiceBusPublisher _publisher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ServiceBusPublisher publisher, ILogger<OrdersController> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req, CancellationToken ct)
    {
        var evt = new OrderCreated(
            OrderId: string.IsNullOrWhiteSpace(req.OrderId) ? Guid.NewGuid().ToString("N") : req.OrderId!,
            Sku: req.Sku,
            Quantity: req.Quantity,
            UnitPrice: req.UnitPrice,
            CreatedAtUtc: DateTimeOffset.UtcNow);

        await _publisher.PublishAsync(evt, ct);
        _logger.LogInformation("Published OrderCreated for {OrderId}", evt.OrderId);
        return Accepted(new { evt.OrderId });
    }
}

public sealed record CreateOrderRequest(string? OrderId, string Sku, int Quantity, decimal UnitPrice);
