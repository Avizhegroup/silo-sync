using Serilog;
using Silo.Infrastructure.Shared;
using Silo.Sync.Core;
using Silo.Sync.Worker;

var builder = Host.CreateDefaultBuilder(args)
    .UseWindowsService(config =>
    {
        config.ServiceName = "Silo Sync Worker";
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSyncCoreServices(hostContext.Configuration);
        services.AddSyncWorker(hostContext.Configuration);
    })
    .UseSerilog();

var host = builder.Build();
await host.RunAsync();
