using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OrionBus.Contracts.Messages;

namespace OrionBus.BillingService.Functions;

public class BillingOrderHandler
{
    private readonly ILogger<BillingOrderHandler> _log;

    public BillingOrderHandler(ILogger<BillingOrderHandler> log)
    {
        _log = log;
    }

    [Function("BillingOrderHandler")]
    public void Run(
        [ServiceBusTrigger("%ORDERS_TOPIC%", "%BILLING_SUB%", Connection = "BillingServiceBusConnection")]
        string message)
    {
        var evt = JsonSerializer.Deserialize<OrderCreated>(message);
        var amount = (evt?.UnitPrice ?? 0m) * (evt?.Quantity ?? 0);

        _log.LogInformation(
            "Billing received OrderCreated: OrderId={OrderId}, Amount={Amount}",
            evt?.OrderId, amount);
    }
}
