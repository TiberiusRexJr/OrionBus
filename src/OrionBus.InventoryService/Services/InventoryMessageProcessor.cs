using System.Text.Json;
using Azure.Messaging.ServiceBus;
using OrionBus.Contracts.Messages;

namespace OrionBus.InventoryService.Services;

public sealed class InventoryMessageProcessor(ServiceBusClient client, TopicSub names, ILogger<InventoryMessageProcessor> log)
    : BackgroundService
{
    private ServiceBusProcessor? _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor = client.CreateProcessor(names.Topic, names.Subscription, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1
        });

        _processor.ProcessMessageAsync += OnMessageAsync;
        _processor.ProcessErrorAsync += OnErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);
        log.LogInformation("Inventory listening on {Topic}/{Sub}", names.Topic, names.Subscription);

        try { await Task.Delay(Timeout.Infinite, stoppingToken); }
        catch (TaskCanceledException) { }

        await _processor.StopProcessingAsync(stoppingToken);
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            if (args.Message.Subject == nameof(OrderCreated))
            {
                var evt = JsonSerializer.Deserialize<OrderCreated>(args.Message.Body.ToString());
                log.LogInformation("Inventory received OrderCreated: OrderId={OrderId} SKU={Sku} Qty={Qty}",
                    evt?.OrderId, evt?.Sku, evt?.Quantity);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Inventory processing failed");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs e)
    {
        log.LogError(e.Exception, "Service Bus error on {Entity}", e.EntityPath);
        return Task.CompletedTask;
    }
}
