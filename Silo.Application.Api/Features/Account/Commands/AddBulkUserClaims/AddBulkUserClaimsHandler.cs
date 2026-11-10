namespace Silo.Application.Api.Features;

public class AddBulkUserClaimsHandler(WmsApiContext context)
    : IRequestHandler<AddBulkUserClaimsCommand, AddBulkUserClaimsVm>
{
    public async Task<AddBulkUserClaimsVm> Handle(
        AddBulkUserClaimsCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.UserIds.Any())
            return new AddBulkUserClaimsVm { Succeeded = false };

        var claimTypes = request.ReplaceClaimTypes?.Distinct().ToList()
            ?? request.Claims.Select(c => c.Type).Distinct().ToList();

        if (!claimTypes.Any())
            return new AddBulkUserClaimsVm { Succeeded = false };

        if (request.Claims.Any(c => !claimTypes.Contains(c.Type)))
            return new AddBulkUserClaimsVm { Succeeded = false };

        await using var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var userId in request.UserIds)
            {
                var existing = await context.UserClaims
                    .Where(uc =>
                        uc.UserId == userId &&
                        claimTypes.Contains(uc.ClaimType))
                    .ToListAsync(cancellationToken);

                context.UserClaims.RemoveRange(existing);

                if (request.Claims.Any())
                {
                    await context.UserClaims.AddRangeAsync(
                        request.Claims.Select(c => new UserClaim
                        {
                            UserId = userId,
                            ClaimType = c.Type,
                            ClaimValue = c.Value
                        }),
                        cancellationToken);
                }
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
