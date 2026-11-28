using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Returns the status of serials.
/// Status rules: Revoked = print record is soft deleted (tag is gone), Used = Tag.Deactivate,
/// Exited = no warehouse and an exit movement exists, Freezed = Tag.Freeze, otherwise Active.
/// The date range filters tags by their registration date, and revoked serials by their revoke date.
/// </summary>
public class GetTagsBySerialsHandler(WmsApiContext context)
    : IRequestHandler<GetTagsBySerialsQuery, GetTagsBySerialsVm> , ITeamYarSecHandler
{
    private sealed record TagRow(string Serial, string Epc, string? WarehouseCode, string? Zone, bool? Freeze, bool? Deactivate);

    private sealed record RevokedRow(int Id, string Serial, string? Epc);

    public async Task<GetTagsBySerialsVm> Handle(GetTagsBySerialsQuery request, CancellationToken cancellationToken)
    {
        var serials = TeamYarHelper.NormalizeSerials(request.Serials);
        var comparer = StringComparer.OrdinalIgnoreCase;

        string? fromText = null;
        string? toText = null;

        if (!string.IsNullOrWhiteSpace(request.FromDate))
        {
            if (!TeamYarHelper.TryParsePersianDate(request.FromDate, out var from))
            {
                throw TeamYarHelper.Validation($"FromDate '{request.FromDate}' is not a valid Persian date (yyyy/MM/dd).");
            }

            fromText = PersianCalendarTools.GregorianToPersian(from);
        }

        if (!string.IsNullOrWhiteSpace(request.ToDate))
        {
            if (!TeamYarHelper.TryParsePersianDate(request.ToDate, out var to))
            {
                throw TeamYarHelper.Validation($"ToDate '{request.ToDate}' is not a valid Persian date (yyyy/MM/dd).");
            }

            toText = PersianCalendarTools.GregorianToPersian(to);
        }

        if (fromText is not null && toText is not null && string.CompareOrdinal(fromText, toText) > 0)
        {
            throw TeamYarHelper.Validation("FromDate must not be after ToDate.");
        }

        if (serials.Count == 0 && fromText is null && toText is null)
        {
            throw TeamYarHelper.Validation("Send at least one serial or a date range.");
        }

        // Tags (registration date is stored as yyyyMMddHHmmss).
        var tagsQuery = context.Tags.AsNoTracking().AsQueryable();

        if (serials.Count > 0)
        {
            tagsQuery = tagsQuery.Where(t => serials.Contains(t.ProductSerial));
        }

        if (fromText is not null)
        {
            var fromDigits = fromText.Replace("/", string.Empty);
            tagsQuery = tagsQuery.Where(t => t.TagRegisterShamsiUnixDate != null
                                             && string.Compare(t.TagRegisterShamsiUnixDate, fromDigits) >= 0);
        }

        if (toText is not null)
        {
            var toDigits = toText.Replace("/", string.Empty) + "999999";
            tagsQuery = tagsQuery.Where(t => t.TagRegisterShamsiUnixDate != null
                                             && string.Compare(t.TagRegisterShamsiUnixDate, toDigits) <= 0);
        }

        var tags = await tagsQuery
            .Select(t => new TagRow(t.ProductSerial, t.TagEpc, t.TagInDestinationId, t.TagZone, t.Freeze, t.Deactivate))
            .ToListAsync(cancellationToken);

        // Tags without a warehouse may have left through an exit movement.
        var candidateEpcs = tags.Where(t => string.IsNullOrEmpty(t.WarehouseCode)).Select(t => t.Epc).ToList();

        var exitedEpcs = candidateEpcs.Count == 0
            ? new HashSet<string>(comparer)
            : (await context.TagsMovements.AsNoTracking()
                            .Where(m => m.HTagsMovementSt == 1 && candidateEpcs.Contains(m.TagEpc!))
                            .Select(m => m.TagEpc!)
                            .Distinct()
                            .ToListAsync(cancellationToken))
              .ToHashSet(comparer);

        var result = tags.Select(t => new GetTagsBySerialsDto
        {
            ProductSerial = t.Serial,
            Epc = t.Epc,
            Status = ResolveStatus(t, exitedEpcs),
            WarehouseCode = t.WarehouseCode ?? string.Empty,
            LocationCode = t.Zone is null or "0" ? string.Empty : t.Zone
        }).ToList();

        // Revoked serials: the tag is deleted and only the soft deleted print record is left.
        var revokedQuery = context.Prints.AsNoTracking().Where(p => p.SoftDelete == 1);

        if (serials.Count > 0)
        {
            revokedQuery = revokedQuery.Where(p => serials.Contains(p.ProductSerial!));
        }

        if (fromText is not null)
        {
            revokedQuery = revokedQuery.Where(p => p.SoftDeleteDate != null && string.Compare(p.SoftDeleteDate, fromText) >= 0);
        }

        if (toText is not null)
        {
            revokedQuery = revokedQuery.Where(p => p.SoftDeleteDate != null && string.Compare(p.SoftDeleteDate, toText) <= 0);
        }

        var activeSerials = result.Select(r => r.ProductSerial).ToHashSet(comparer);

        var revoked = (await revokedQuery
                .Select(p => new RevokedRow(p.Id, p.ProductSerial!, p.TagEpc))
                .ToListAsync(cancellationToken))
            .GroupBy(r => r.Serial, comparer)
            .Select(g => g.OrderByDescending(r => r.Id).First())
            .Where(r => !activeSerials.Contains(r.Serial));

        result.AddRange(revoked.Select(r => new GetTagsBySerialsDto
        {
            ProductSerial = r.Serial,
            Epc = r.Epc ?? string.Empty,
            Status = GetTagsBySerialsStatusEnum.Revoked,
            WarehouseCode = string.Empty,
            LocationCode = string.Empty
        }));

        return new GetTagsBySerialsVm { Result = result.OrderBy(r => r.ProductSerial, comparer).ToList() };
    }

    private static GetTagsBySerialsStatusEnum ResolveStatus(TagRow tag, HashSet<string> exitedEpcs)
    {
        if (tag.Deactivate == true) return GetTagsBySerialsStatusEnum.Used;
        if (exitedEpcs.Contains(tag.Epc)) return GetTagsBySerialsStatusEnum.Exited;
        if (tag.Freeze == true) return GetTagsBySerialsStatusEnum.Freezed;

        return GetTagsBySerialsStatusEnum.Active;
    }
}
