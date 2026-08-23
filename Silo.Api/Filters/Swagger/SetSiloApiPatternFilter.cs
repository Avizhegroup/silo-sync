using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Silo.Api.Filters.Swagger;

public class SetSiloApiPatternFilter : IDocumentFilter
{
    private readonly IConfiguration configuration;

    public SetSiloApiPatternFilter(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();
        foreach (var entry in swaggerDoc.Paths)
        {
                paths.Add(
                    entry.Key.Replace("v{version}", swaggerDoc.Info.Version),
                    entry.Value);
        }

        swaggerDoc.Paths = paths;
    }
}
