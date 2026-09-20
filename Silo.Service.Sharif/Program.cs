using Silo.Service.Sharif;
using Silo.Service.Sharif.Components;
using Silo.Service.Sharif.Configuration;
using Silo.Service.Sharif.Services;
using Silo.Service.Sharif.State;
using Silo.Infrastructure.Shared;
using Serilog;

var baseDir = AppContext.BaseDirectory;
var logsPath = Path.Combine(baseDir, "Logs");

if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(Path.Combine(logsPath, "InfoLogs"));
    Directory.CreateDirectory(Path.Combine(logsPath, "Exceptions"));
}

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService(config =>
{
    config.ServiceName = "SiloSharifService";
});

builder.WebHost.UseUrls(builder.Configuration["Ui:Url"] ?? "http://localhost:5005");

builder.Host.UseSerilog();

builder.Services.AddSiloSerilogForWindowsServices(builder.Configuration);

builder.Services.Configure<RfidWorkerOptions>(builder.Configuration.GetSection("RfidWorker"));

builder.Services.AddSingleton<ReaderStateStore>();
builder.Services.AddSingleton<RfidReaderService>();
builder.Services.AddSingleton<ReaderControlService>();
builder.Services.AddSingleton<WorkerSettingsWriter>();
builder.Services.AddSingleton<LogTailService>();

builder.Services.AddSingleton<RfidConnectApiForSharif>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTelerikBlazor();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
