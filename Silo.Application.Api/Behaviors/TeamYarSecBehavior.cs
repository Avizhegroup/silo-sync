using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Shared.Contracts;

namespace Silo.Application;

public sealed class TeamYarSecBehavior<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> requestHandler,
    WmsApiContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ILogger<TeamYarSecBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (requestHandler is not ITeamYarSecHandler || request is not TeamYarBaseCommand command)
        {
            return await next(cancellationToken);
        }

        var connection = new StructuredExternalConnection
        {
            Type = request.GetType().Name,
            EventId = command.CommandUniqueId,
            DateTime = DateTime.Now,
            Data = JsonSerializer.Serialize(request),
            Method = requestHandler.GetType().Name,
            Status = 0,
        };

        dbContext.StructuredExternalConnections.Add(connection);
        await dbContext.SaveChangesAsync(cancellationToken);

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var capture = httpContext.Features.Get<TeamYarSecResponseCapture>();
            if (capture is null)
            {
                capture = new TeamYarSecResponseCapture(httpContext.Response.Body);
                httpContext.Features.Set(capture);
                httpContext.Response.Body = capture.ResponseBody;
            }

            capture.ConnectionIds.Add(connection.Id);
        }

        try
        {
            return await next(cancellationToken);
        }
        catch
        {
            try
            {
                await dbContext.StructuredExternalConnections
                    .Where(x => x.Id == connection.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.Status, StructuredExternalConnection.FailedStatus),
                        CancellationToken.None);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Could not mark TeamYar connection {ConnectionId} as failed.", connection.Id);
            }

            throw;
        }
    }
}
