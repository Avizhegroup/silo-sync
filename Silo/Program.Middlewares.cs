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
    }
}
