using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Silo.Api.Filters.Swagger;

public class AuthorizationMethodCheckFilter : IOperationFilter
{

    private readonly bool includeUnauthorizedAndForbiddenResponses;
    private readonly string schemeName;

    public AuthorizationMethodCheckFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
    {
        this.includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
        this.schemeName = schemeName;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.GetCustomAttributes(true);

        var hasAnonymous = attributes.OfType<AllowAnonymousAttribute>().Any();
        if (hasAnonymous) return;

        var hasAuthorize = attributes.OfType<AuthorizeAttribute>().Any();
        if (!hasAuthorize) return;

        if (includeUnauthorizedAndForbiddenResponses)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse() { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }

        var securityRequirements = new List<OpenApiSecurityRequirement>();
        var openApiScheme = new OpenApiSecurityScheme()
        {
            Scheme = schemeName,
            Reference = new OpenApiReference() { Type = ReferenceType.SecurityScheme, Id = schemeName }
        };
        var requirement = new OpenApiSecurityRequirement();
        requirement.Add(openApiScheme, new List<string>() { "readAccess", "writeAccess" });
        securityRequirements.Add(requirement);
        operation.Security = securityRequirements;
    }
}
