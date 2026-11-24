using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Revokes serials the same way the legacy revoke (SRevokeTags) does: the tag and its movements are deleted
/// and the print record is soft deleted. The inquiry then reports the serial as Revoked.
/// </summary>
public class RevokeTagForTeamYarHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , ILogger<RevokeTagForTeamYarHandler> logger)
    : IRequestHandler<RevokeTagForTeamYarCommand, RevokeTagForTeamYarVm>, ITeamYarSecHandler
{
    public async Task<RevokeTagForTeamYarVm> Handle(RevokeTagForTeamYarCommand request, CancellationToken cancellationToken)
    {
        var serials = TeamYarHelper.NormalizeSerials(request.Serials);

        if (serials.Count == 0)
        {
            throw TeamYarHelper.Validation("Serials must contain at least one serial.");
        }

        var tags = await context.Tags.AsNoTracking()
                                .Where(t => serials.Contains(t.ProductSerial))
                                .Select(t => new { t.ProductSerial, t.TagEpc })
                                .ToListAsync(cancellationToken);

        var known = await TeamYarHelper.GetPrintSerialsAsync(context, serials, cancellationToken);
        known.UnionWith(tags.Select(t => t.ProductSerial));

        var unknown = serials.Where(s => !known.Contains(s)).ToList();

        if (unknown.Count > 0)
        {
            throw TeamYarHelper.Validation($"Serial(s) not found: {TeamYarHelper.Quote(unknown)}.");
        }

        var epcs = tags.Select(t => t.TagEpc).Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);
        var revokeDate = DateTimeTools.GetPersianDate();

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.TagsMovements
                         .Where(m => serials.Contains(m.ProductSerial!) || epcs.Contains(m.TagEpc!))
                         .ExecuteDeleteAsync(cancellationToken);

            foreach (var epc in epcs)
            {
                await context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM tbl_ProductPlacementMissions WHERE fld_PPMProductEPC = {epc}", cancellationToken);
            }

            await context.Tags
                         .Where(t => serials.Contains(t.ProductSerial))
                         .ExecuteDeleteAsync(cancellationToken);

            await context.Prints
                         .Where(p => (serials.Contains(p.ProductSerial!) || epcs.Contains(p.TagEpc!))
                                     && (p.SoftDelete == 0 || p.SoftDelete == null))
                         .ExecuteUpdateAsync(setters => setters
                             .SetProperty(p => p.SoftDelete, (int?)1)
                             .SetProperty(p => p.SoftDeleteUserId, userId)
                             .SetProperty(p => p.SoftDeleteDate, revokeDate), cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar RevokeTags failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }
}
