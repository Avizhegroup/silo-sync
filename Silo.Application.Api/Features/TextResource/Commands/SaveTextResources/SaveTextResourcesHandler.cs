namespace Silo.Application.Api.Features;

public class SaveTextResourcesHandler(WmsApiContext context)
    : IRequestHandler<SaveTextResourcesCommand, SaveTextResourcesVm>
{
    public async Task<SaveTextResourcesVm> Handle(SaveTextResourcesCommand request
        , CancellationToken cancellationToken)
    {
        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (request.DeletedIds.Any())
            {
                await context.TextResources
                    .Where(x => request.DeletedIds.Contains(x.Id))
                    .ExecuteDeleteAsync(cancellationToken);
            }

            var existingKeys = await context.TextResources
                .Select(x => x.Key)
                .ToListAsync(cancellationToken);

            foreach (var item in request.Items)
            {
                if (item.Id is 0)
                {
                    if (existingKeys.Contains(item.Key))
                    {
                        context.TextResources.Update(new()
                        {
                            Id = context.TextResources
                                .Where(x => x.Key == item.Key)
                                .Select(x => x.Id)
                                .First(),
                            Key = item.Key,
                            Value = item.Value
                        });
                    }
                    else
                    {
                        await context.TextResources.AddAsync(new()
                        {
                            Key = item.Key,
                            Value = item.Value
                        }, cancellationToken);
                    }
                }
                else
                {
                    context.TextResources.Update(new()
                    {
                        Id = item.Id,
                        Key = item.Key,
                        Value = item.Value
                    });
                }
            }

            var result = await context.SaveChangesAsync(cancellationToken) > 0
                || request.DeletedIds.Count is not 0;

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = result };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            return new() { Result = false };
        }
    }
}
