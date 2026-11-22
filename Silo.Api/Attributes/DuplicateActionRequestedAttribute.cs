using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Silo.Application.Shared.Exceptions;
using Silo.Domains.Services;

namespace Silo.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class DuplicateActionRequestedAttribute : TypeFilterAttribute
{
    public DuplicateActionRequestedAttribute() : base(typeof(DuplicateActionRequestedFilter))
    {
    }
}

public sealed class DuplicateActionRequestedFilter(WmsApiContext dbContext) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var command in context.ActionArguments.Values.OfType<TeamYarBaseCommand>())
        {
            var isDuplicate = await dbContext.StructuredExternalConnections
                .AnyAsync(connection => connection.EventId == command.CommandUniqueId,
                    context.HttpContext.RequestAborted);

            if (isDuplicate)
            {
                var actionName = context.ActionDescriptor.RouteValues["action"];
                throw new DuplicateActionRequestedException(actionName, command.CommandUniqueId);
            }
        }

        await next();
    }
}
