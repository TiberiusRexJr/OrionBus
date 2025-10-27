using OrionBus.CommandService.Services;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Load configuration and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var cfg = builder.Configuration;

// Read settings from the new "Values" section
var cs = cfg["Values:CommandServiceBusConnection"]
    ?? throw new InvalidOperationException("Missing Values:CommandServiceBusConnection");

var topic = cfg["Values:ORDERS_TOPIC"] ?? "products-topic";

// Register dependencies
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject ServiceBusPublisher using the new config
builder.Services.AddSingleton(_ => new ServiceBusPublisher(cs, topic));

var app = builder.Build();

// Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
