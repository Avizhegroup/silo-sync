using Silo.Ui.Bypass;
using Silo.Ui.Bypass.Services.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});

builder.Services.AddScoped(sp =>
{
    HttpClient httpClient = new(sp.GetRequiredService<ApiHandler>());

    return httpClient;
});

builder.Services.AddScoped<ApiHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// TODO: WTF!!!! We should remove this line soon as possible
PdfExporter.WebRootPath = app.Environment.WebRootPath;

app.Run();
