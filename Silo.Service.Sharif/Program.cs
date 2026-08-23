using Silo.Service.Sharif;
using Silo.Infrastructure.Shared;
using Serilog;

var baseDir = AppContext.BaseDirectory;
var logsPath = Path.Combine(baseDir, "Logs");

if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(Path.Combine(logsPath, "InfoLogs"));
    Directory.CreateDirectory(Path.Combine(logsPath, "Exceptions"));
}

var builder = Host.CreateDefaultBuilder(args);

builder
    .UseWindowsService(config =>
    {
        config.ServiceName = "SiloSharifService";
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSiloSerilogForWindowsServices();

        services.AddSingleton<RfidConnectApiForSharif>();
        services.AddSingleton<RfidReaderService>();

        services.AddHostedService<Worker>();
    })
    .UseSerilog();

var host = builder.Build();
host.Run();
