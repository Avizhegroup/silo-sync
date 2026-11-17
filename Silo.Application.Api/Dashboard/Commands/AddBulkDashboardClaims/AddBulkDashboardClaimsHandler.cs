namespace Silo.Application.Api.Features;

public class AddBulkDashboardClaimsHandler(WmsApiContext context)
    : IRequestHandler<AddBulkDashboardClaimsCommand, AddBulkUserClaimsVm>
{
    public async Task<AddBulkUserClaimsVm> Handle(
        AddBulkDashboardClaimsCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.UserIds.Any())
            return new AddBulkUserClaimsVm { Succeeded = false };

        var userIds = request.UserIds.Distinct().ToList();

        var itemCodes = request.ItemCodes
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await using var transaction = await context.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var userId in userIds)
            {
                var existing = await context.UserClaims
                    .Where(uc =>
                        uc.UserId == userId &&
                        uc.ClaimType == "dashboard")
                    .ToListAsync(cancellationToken);

                context.UserClaims.RemoveRange(existing);

                if (itemCodes.Any())
                {
                    await context.UserClaims.AddRangeAsync(
                        itemCodes.Select(code => new UserClaim
                        {
                            UserId = userId,
                            ClaimType = "dashboard",
                            ClaimValue = code
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
