using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace OrionBus.BillingService;

public class BillingFunction
{
    private readonly ILogger _logger;
    public BillingFunction(ILoggerFactory loggerFactory)
        => _logger = loggerFactory.CreateLogger<BillingFunction>();

    [Function("BillingFunction")]
    public void Run(
      [ServiceBusTrigger("%ServiceBusTopic%", "%ServiceBusSubscription%",
                       Connection = "AzureWebJobsServiceBus")]
    string message)
    {
        _logger.LogInformation("Billing function processed: {message}", message);
    }

}
