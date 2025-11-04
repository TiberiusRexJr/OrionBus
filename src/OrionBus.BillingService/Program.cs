using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// (optional) keep this if you want App Insights locally
builder.ConfigureFunctionsWebApplication();
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.ConfigureFunctionsApplicationInsights();

// ---- PROBE: read from config and env to verify values exist ----
string Read(string key)
    => builder.Configuration[key] ?? Environment.GetEnvironmentVariable(key);

var topic = Read("ServiceBusTopic");
var sub = Read("ServiceBusSubscription");
var conn = Read("AzureWebJobsServiceBus");

Console.WriteLine($"[Startup] ServiceBusTopic: {topic}");
Console.WriteLine($"[Startup] ServiceBusSubscription: {sub}");
Console.WriteLine($"[Startup] AzureWebJobsServiceBus set: {(!string.IsNullOrWhiteSpace(conn)).ToString()}");

// If any are missing, throw so you see it immediately
if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException("Missing Service Bus settings. Ensure local.settings.json 'Values' contains " +
        "AzureWebJobsServiceBus, ServiceBusTopic, and ServiceBusSubscription.");
}
// ----------------------------------------------------------------

builder.Build().Run();
