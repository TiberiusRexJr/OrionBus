using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace OrionBus.CommandService.Services;

public sealed class ServiceBusPublisher : IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBusPublisher(string connectionString, string topicName)
    {
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    public async Task PublishAsync<T>(T payload, CancellationToken ct = default)
    {
        var body = JsonSerializer.Serialize(payload);
        var message = new ServiceBusMessage(body)
        {
            ContentType = "application/json",
            Subject = typeof(T).Name // "OrderCreated"
        };
        await _sender.SendMessageAsync(message, ct);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
