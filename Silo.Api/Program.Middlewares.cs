using Microsoft.Extensions.FileProviders;
using Silo.Api.Extensions;
using Silo.Api.Hubs;
using Silo.Infrastructure.Shared;

namespace Silo.Api;
public static partial class Program
{
    public static WebApplication Configure(this WebApplication app
       , IConfiguration configuration)
    {
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Files")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Files"));
        }

        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(
                 Path.Combine(Directory.GetCurrentDirectory(), "Files")),
            RequestPath = "/Files",
            EnableDefaultFiles = true
        });

        app.UseSiloApiSwagger();

        app.UseAuthorization();

        app.UseCors("OpenCors");

        app.MapControllers();

        app.MapHub<WmsHub>("/wmshub");

        app.UseInfrastructureSharedMiddlewares();

        app.UseResponseCompression();

        return app;
    }
}
