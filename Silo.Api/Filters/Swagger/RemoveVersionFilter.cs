using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Silo.Api.Filters.Swagger;

public class RemoveVersionFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name.Equals("version"));
        if (!(versionParameter is null))
            operation.Parameters.Remove(versionParameter);
    }
}
