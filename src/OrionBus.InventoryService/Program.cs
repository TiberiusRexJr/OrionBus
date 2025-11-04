using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrionBus.InventoryService.Services;

var builder = WebApplication.CreateBuilder(args);

// config: appsettings + env
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var cfg = builder.Configuration;

// read ONLY the Values:* keys you set in .env
var cs = cfg["Values:InventoryServiceBusConnection"]
          ?? throw new InvalidOperationException("Missing Values:InventoryServiceBusConnection");
var topic = cfg["Values:ORDERS_TOPIC"] ?? "products-topic";
var sub = cfg["Values:INVENTORY_SUB"] ?? "inventory-sub";

// DI
builder.Services.AddSingleton(new TopicSub(topic, sub));
builder.Services.AddSingleton(_ => new ServiceBusClient(cs));
builder.Services.AddHostedService<InventoryMessageProcessor>();

// Health checks (optional formal one)
builder.Services.AddHealthChecks();

var app = builder.Build();

// Liveness
app.MapGet("/healthz", () => Results.Ok("InventoryService is alive"));

// Readiness (checks key config)
app.MapGet("/readyz", () =>
{
    var conn = app.Configuration["Values:InventoryServiceBusConnection"];
    return string.IsNullOrWhiteSpace(conn)
        ? Results.Problem("Service Bus connection not configured.")
        : Results.Ok("InventoryService ready");
});

// (Optional) formal health endpoint if you prefer /healthz via HealthChecks
// app.MapHealthChecks("/healthz");

app.Run();

public sealed record TopicSub(string Topic, string Subscription);
