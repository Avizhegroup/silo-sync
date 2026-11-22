using Silo.Application.Shared.Contracts;

namespace Silo.Application;

public sealed class TeamYarSecBehavior<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> requestHandler,
    WmsApiContext dbContext) : IPipelineBehavior<TRequest, TResponse>
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
            Method = requestHandler.GetType().Name
        };

        dbContext.StructuredExternalConnections.Add(connection);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await next(cancellationToken);
    }
}
