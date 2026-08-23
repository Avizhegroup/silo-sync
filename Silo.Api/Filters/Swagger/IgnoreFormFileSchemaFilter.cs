using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Silo.Api.Filters.Swagger;

/// <summary>
/// Schema filter to prevent Swashbuckle from generating schemas for types containing IFormFile.
/// This allows the FileUploadOperationFilter to handle these types properly.
/// </summary>
public class IgnoreFormFileSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Check if the type contains IFormFile properties
    if (context.Type != null && HasFormFileProperty(context.Type))
        {
        // Clear all properties to prevent schema generation errors
            schema.Properties?.Clear();
  schema.Type = "object";
            schema.Format = "binary";
        }
    }

    private static bool HasFormFileProperty(Type type)
    {
 if (type == null || type == typeof(string) || type.IsPrimitive)
  return false;

     return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
         .Any(p => p.PropertyType == typeof(IFormFile));
    }
}
