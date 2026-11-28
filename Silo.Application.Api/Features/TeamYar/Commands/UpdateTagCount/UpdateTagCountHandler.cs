using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Edits counts and units of serials. Only the fields that are sent (not null) are changed.
/// Works for serials that are only prepared for print and for serials that already have a tag.
/// </summary>
public class UpdateTagCountHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , ILogger<UpdateTagCountHandler> logger)
    : IRequestHandler<UpdateTagCountCommand, UpdateTagCountVm>, ITeamYarSecHandler
{
    public async Task<UpdateTagCountVm> Handle(UpdateTagCountCommand request, CancellationToken cancellationToken)
    {
        var list = request.List;

        if (list is null || list.Count == 0 || list.Any(i => i is null))
        {
            throw TeamYarHelper.Validation("List must contain at least one item and no empty item.");
        }

        var comparer = StringComparer.OrdinalIgnoreCase;
        var errors = new List<string>();

        foreach (var repeated in list.GroupBy(i => TeamYarHelper.Clean(i.ProductSerial), comparer).Where(g => g.Count() > 1))
        {
            errors.Add($"Serial '{repeated.Key}' is repeated in the request.");
        }

        for (var index = 0; index < list.Count; index++)
        {
            if (string.IsNullOrWhiteSpace(list[index].ProductSerial)) errors.Add($"List[{index}]: productSerial is required.");
            if (list[index].OriginalCount < 0 || list[index].SubCount < 0) errors.Add($"List[{index}]: counts must not be negative.");
        }

        var serials = TeamYarHelper.NormalizeSerials(list.Select(i => i.ProductSerial));

        var tags = (await context.Tags.Where(t => serials.Contains(t.ProductSerial)).ToListAsync(cancellationToken))
                   .GroupBy(t => t.ProductSerial, comparer)
                   .ToDictionary(g => g.Key, g => g.First(), comparer);

        var prints = (await context.Prints
                                   .Where(p => serials.Contains(p.ProductSerial!) && (p.SoftDelete == 0 || p.SoftDelete == null))
                                   .ToListAsync(cancellationToken))
                     .GroupBy(p => p.ProductSerial!, comparer)
                     .ToDictionary(g => g.Key, g => g.ToList(), comparer);

        foreach (var serial in serials.Where(s => !tags.ContainsKey(s) && !prints.ContainsKey(s)))
        {
            errors.Add($"Serial '{serial}' does not exist or is revoked.");
        }

        if (errors.Count > 0)
        {
            throw TeamYarHelper.Validation(errors);
        }

        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var dto in list)
            {
                var serial = TeamYarHelper.Clean(dto.ProductSerial);
                var properties = new List<KeyValuePair<string, string>>();

                if (!string.IsNullOrWhiteSpace(dto.UnitForSubCount))
                {
                    properties.Add(new(TeamYarHelper.SubCountUnitPropertyKey, dto.UnitForSubCount.Trim()));
                }

                if (prints.TryGetValue(serial, out var serialPrints))
                {
                    foreach (var print in serialPrints)
                    {
                        if (dto.OriginalCount.HasValue) print.ProductCount = dto.OriginalCount;
                        if (dto.SubCount.HasValue) print.ProductItemCount = dto.SubCount;
                        if (!string.IsNullOrWhiteSpace(dto.UnitForOriginalCount)) print.ProductUnit = dto.UnitForOriginalCount.Trim();

                        print.ProductProperties = TeamYarHelper.MergeProperties(print.ProductProperties, properties);
                    }
                }

                if (tags.TryGetValue(serial, out var tag))
                {
                    if (dto.OriginalCount.HasValue) tag.ProductCount = dto.OriginalCount;

                    tag.ProductProperties = TeamYarHelper.MergeProperties(tag.ProductProperties, properties);
                    tag.LastModifierUser = userId;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar UpdateTagCount failed.");
            throw;
        }
    }
}
