using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add environment variables
        config.AddEnvironmentVariables();

        // Optional: include local.settings.json for local testing (Functions uses this)
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();