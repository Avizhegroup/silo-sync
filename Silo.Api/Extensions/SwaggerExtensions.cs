using Microsoft.OpenApi.Models;
using Silo.Api.Filters.Swagger;
using Silo.Base.Controllers.Base;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Silo.Api.Extensions;

public static class SwaggerExtensions
{
    public static void AddSiloApiSwagger(this IServiceCollection services
        , IConfiguration config)
    {
        services.AddSwaggerGen(options =>
        {
            // Add schema filter FIRST to prevent schema generation errors for types with IFormFile
            options.SchemaFilter<IgnoreFormFileSchemaFilter>();
                
            // Add file upload filter to properly handle file uploads
            options.OperationFilter<FileUploadOperationFilter>();

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Silo.xml"), true);

            options.OperationFilter<AuthorizationMethodCheckFilter>(true, "Bearer");

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            options.OperationFilter<RemoveVersionFilter>();

            options.DocumentFilter<SetSiloApiPatternFilter>();

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                var versions = methodInfo.DeclaringType
                    .Namespace.Split('.').Last().Replace("v", "");
                return versions.Any(v => $"v{v}" == docName);
            });

            options.EnableAnnotations();

            var assembly = Assembly.GetExecutingAssembly();

            var namespaces = assembly.GetTypes()
                .Where(w => typeof(SiloBaseController).IsAssignableFrom(w)
                   && !w.Name.Contains("Base"))
                .Select(p => new string(p.Namespace)).Distinct();

            foreach (string ns in namespaces)
            {
                var version = ns.Split('.').Last().Replace("v", "");
                options.SwaggerDoc("v" + version, new OpenApiInfo() { Version = "v" + version, Title = "API version " + version });
            }
        });
    }

    public static void UseSiloApiSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(option =>
        {
            var assembly = Assembly.GetExecutingAssembly();

            var namespaces = assembly.GetTypes()
                .Where(w => typeof(SiloBaseController).IsAssignableFrom(w) && !w.Namespace.Contains("Base"))
                .Select(p => new string(p.Namespace)).Distinct();

            foreach (string ns in namespaces)
            {
                var version = ns.Split('.').Last().Replace("v", "");

                option.SwaggerEndpoint($"/swagger/v{version}/swagger.json", "Doc" + version);
            }

            option.InjectStylesheet("/Files/Css/swagger.css");

            option.InjectJavascript("/Files/Js/swagger.js");
        });
    }
}
