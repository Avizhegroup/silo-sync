using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Silo.Api.Filters.Swagger;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check for parameters that are IFormFile directly WITH [FromForm] binding source
        var directFileParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile) && 
         (p.Source?.Id == "Form" || p.Source?.Id == "FormFile"))
.ToList();

        // Check for complex types that contain IFormFile properties with [FromForm] binding
      var complexTypeWithFileParameters = context.ApiDescription.ParameterDescriptions
.Where(p => p.Source?.Id == "Form" &&
       p.Type != null && 
    p.Type != typeof(IFormFile) &&
        p.Type.IsClass && 
           p.Type != typeof(string) &&
      HasFormFileProperty(p.Type))
     .ToList();

        // If no file parameters found, skip
        if (!directFileParameters.Any() && !complexTypeWithFileParameters.Any())
            return;

        // Clear existing parameters to avoid conflicts
        operation.Parameters?.Clear();

  // Create request body for multipart/form-data
        operation.RequestBody = new OpenApiRequestBody
      {
     Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
       {
          ["multipart/form-data"] = new OpenApiMediaType
         {
   Schema = new OpenApiSchema
          {
  Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>(),
      Required = new HashSet<string>()
     }
         }
            }
 };

        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

   // Add direct file upload fields
    foreach (var fileParam in directFileParameters)
        {
            schema.Properties[fileParam.Name] = new OpenApiSchema
       {
                Type = "string",
     Format = "binary",
    Description = $"Upload {fileParam.Name}"
    };

            if (fileParam.IsRequired)
            {
            schema.Required.Add(fileParam.Name);
   }
        }

      // Add properties from complex types containing IFormFile
        foreach (var complexParam in complexTypeWithFileParameters)
    {
            var properties = complexParam.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
        if (prop.PropertyType == typeof(IFormFile))
     {
           // Add file property
                schema.Properties[prop.Name] = new OpenApiSchema
        {
     Type = "string",
      Format = "binary",
          Description = $"Upload {prop.Name}"
            };
                }
             else
    {
    // Add other properties as form fields
        schema.Properties[prop.Name] = GetSchemaForType(prop.PropertyType);
       }
      }
        }

        // Re-add non-file, non-form parameters (like headers, query params)
    var otherParameters = context.ApiDescription.ParameterDescriptions
     .Where(p => p.Source?.Id != "Form" && 
          p.Source?.Id != "FormFile" &&
            p.Type != typeof(IFormFile) &&
          !HasFormFileProperty(p.Type))
            .ToList();

  foreach (var param in otherParameters)
        {
  if (param.Source?.Id == "Header")
            {
         // Add as header parameter
     operation.Parameters ??= new List<OpenApiParameter>();
   operation.Parameters.Add(new OpenApiParameter
      {
   Name = param.Name,
     In = ParameterLocation.Header,
    Required = param.IsRequired,
         Description = param.Name,
              Schema = new OpenApiSchema
         {
                Type = "string"
            }
        });
            }
 else if (param.Source?.Id == "Query")
         {
     // Add as query parameter
    operation.Parameters ??= new List<OpenApiParameter>();
                operation.Parameters.Add(new OpenApiParameter
      {
Name = param.Name,
          In = ParameterLocation.Query,
       Required = param.IsRequired,
      Schema = new OpenApiSchema
  {
          Type = "string"
        }
            });
    }
 }
 }

    private static bool HasFormFileProperty(Type type)
    {
        if (type == null || type == typeof(string) || type.IsPrimitive)
     return false;

        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .Any(p => p.PropertyType == typeof(IFormFile));
    }

    private static OpenApiSchema GetSchemaForType(Type type)
{
   if (type == typeof(string))
   return new OpenApiSchema { Type = "string" };
      
 if (type == typeof(int) || type == typeof(int?))
          return new OpenApiSchema { Type = "integer", Format = "int32" };
    
        if (type == typeof(long) || type == typeof(long?))
   return new OpenApiSchema { Type = "integer", Format = "int64" };
        
        if (type == typeof(bool) || type == typeof(bool?))
            return new OpenApiSchema { Type = "boolean" };
        
        if (type == typeof(DateTime) || type == typeof(DateTime?))
     return new OpenApiSchema { Type = "string", Format = "date-time" };
        
if (type == typeof(decimal) || type == typeof(decimal?) || 
   type == typeof(double) || type == typeof(double?) ||
     type == typeof(float) || type == typeof(float?))
   return new OpenApiSchema { Type = "number" };

        // Default to string for complex types
        return new OpenApiSchema { Type = "string" };
    }
}
