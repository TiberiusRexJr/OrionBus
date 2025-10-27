using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using OrionBus.InventoryService.Services;

var builder = Host.CreateApplicationBuilder(args);

// files + env; env wins
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var cfg = builder.Configuration;

// read ONLY the new Values:* keys
var cs  = cfg["Values:InventoryServiceBusConnection"]
          ?? throw new InvalidOperationException("Missing Values:InventoryServiceBusConnection");

var topic = cfg["Values:ORDERS_TOPIC"]    ?? "orders-topic";
var sub   = cfg["Values:INVENTORY_SUB"]   ?? "inventory-sub";

builder.Services.AddSingleton(new TopicSub(topic, sub));
builder.Services.AddSingleton(_ => new ServiceBusClient(cs));
builder.Services.AddHostedService<InventoryMessageProcessor>();

await builder.Build().RunAsync();

public sealed record TopicSub(string Topic, string Subscription);
