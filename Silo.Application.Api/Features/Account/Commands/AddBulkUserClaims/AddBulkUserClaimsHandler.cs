namespace Silo.Application.Api.Features;

public class AddBulkUserClaimsHandler(WmsApiContext context)
    : IRequestHandler<AddBulkUserClaimsCommand, AddBulkUserClaimsVm>
{
    public async Task<AddBulkUserClaimsVm> Handle(
        AddBulkUserClaimsCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.UserIds.Any() || !request.Claims.Any())
        {
            return new AddBulkUserClaimsVm { Succeeded = false };
        }

        var claimTypes = request.Claims
            .Select(c => c.Type)
            .Distinct()
            .ToList();

        await using var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var userId in request.UserIds)
            {
                var existing = await context.UserClaims
                    .Where(uc => uc.UserId == userId && claimTypes.Contains(uc.ClaimType))
                    .ToListAsync(cancellationToken);

                context.UserClaims.RemoveRange(existing);

                await context.UserClaims.AddRangeAsync(
                    request.Claims.Select(c => new UserClaim
                    {
                        UserId = userId,
                        ClaimType = c.Type,
                        ClaimValue = c.Value
                    }),
                    cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new AddBulkUserClaimsVm { Succeeded = true };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

            return new AddBulkUserClaimsVm { Succeeded = false };
        }
    }
}
