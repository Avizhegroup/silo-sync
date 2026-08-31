using Microsoft.Extensions.Logging.Abstractions;

namespace Silo;

public static partial class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder.Build();

        try
        {
            var cache = app.Services.GetRequiredService<IFormalDataCache>();
            var textResources = cache.GetTextResources().GetAwaiter().GetResult();

            ResourceManager.Load(textResources.ToDictionary(x => x.Key, x => x.Value));
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger(nameof(Program));
            (logger ?? NullLogger.Instance).LogError(ex, "Failed to load text resources into ResourceManager.");
        }

        app.Configure();

        app.Run();
    }
}
