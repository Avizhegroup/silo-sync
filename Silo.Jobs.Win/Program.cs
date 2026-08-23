using Silo.Jobs.Win;
using Serilog;

var webApplicationOptions = new WebApplicationOptions() 
{ 
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
};

var builder = WebApplication.CreateBuilder(webApplicationOptions);

builder.Host
       .UseWindowsService(config =>
       {
           config.ServiceName = "Silo Service";
       })
       .ConfigureServices((context, services) =>
       {
           services.ConfigureServices(context.Configuration);
       })
       .UseSerilog();

var app = builder.Build();

app.Configure();

app.Run();
