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
            // IFormalDataCache is registered as scoped, so it must be resolved from a
            // dedicated DI scope rather than the root provider (which throws when
            // scope validation is enabled, e.g. in the Development environment).
            using var scope = app.Services.CreateScope();

            var cache = scope.ServiceProvider.GetRequiredService<IFormalDataCache>();

            // GetTextResources() fetches (and caches) the text resources from the API and
            // loads them into the static ResourceManager used by TextResources.
            cache.GetTextResources().GetAwaiter().GetResult();
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
