namespace Silo;
public static partial class Program
{
    public static void Configure(this WebApplication app)
    {
#if DEBUG
        app.UseDeveloperExceptionPage();
#else
        app.UseExceptionHandler("/Home/Error");
#endif
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("OpenCors");

        app.UseResponseCompression();

        app.MapDefaultControllerRoute();

        app.MapBlazorHub();

        app.MapFallbackToPage("/_Host");

        try
        {
            using var scope = app.Services.CreateScope();

            var cache = scope.ServiceProvider.GetRequiredService<IFormalDataCache>();
            
            cache.GetTextResources().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetService<ILoggerFactory>()?.CreateLogger(nameof(Program));

            (logger).LogError(ex, "Failed to load text resources into ResourceManager.");
        }
    }
}
