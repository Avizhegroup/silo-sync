using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Records that TeamYar confirmed a placement. The movement rows of the serials are marked with the same
/// "sent to external API" fields the legacy code uses (ApiSendStatus, ApiSendUser, ApiSendDateTime, ApiSendData).
/// </summary>
public class VerifyLocateForTeamYarHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , ILogger<VerifyLocateForTeamYarHandler> logger)
    : IRequestHandler<VerifyLocateForTeamYarCommand, VerifyLocateForTeamYarVm>, ITeamYarSecHandler
{
    private sealed record MovementRow(int Id, string Serial, string? DocumentCode);

    public async Task<VerifyLocateForTeamYarVm> Handle(VerifyLocateForTeamYarCommand request, CancellationToken cancellationToken)
    {
        var serials = TeamYarHelper.NormalizeSerials(request.Serials);
        var documentCode = TeamYarHelper.Clean(request.DocumentCode);
        var warehouseCode = TeamYarHelper.Clean(request.WarehouseCode);
        var operationCode = TeamYarHelper.Clean(request.OperationCode);
        var errors = new List<string>();

        if (serials.Count == 0) errors.Add("Serials must contain at least one serial.");
        if (documentCode.Length == 0) errors.Add("DocumentCode is required.");
        if (warehouseCode.Length == 0) errors.Add("WarehouseCode is required.");
        if (operationCode.Length == 0) errors.Add("OperationCode is required.");

        if (errors.Count > 0)
        {
            throw TeamYarHelper.Validation(errors);
        }

        if (!await context.Warehouses.AsNoTracking().AnyAsync(w => w.Code == warehouseCode, cancellationToken))
        {
            throw TeamYarHelper.Validation($"Warehouse code '{warehouseCode}' does not exist.");
        }

        var found = await TeamYarHelper.GetTagSerialsAsync(context, serials, cancellationToken);
        var missing = serials.Where(s => !found.Contains(s)).ToList();

        if (missing.Count > 0)
        {
            throw TeamYarHelper.Validation($"Serial(s) not found or revoked: {TeamYarHelper.Quote(missing)}.");
        }

        var comparer = StringComparer.OrdinalIgnoreCase;

        var movements = (await context.TagsMovements.AsNoTracking()
                                      .Where(m => serials.Contains(m.ProductSerial!))
                                      .Select(m => new MovementRow(m.TagsMovementId, m.ProductSerial!, m.RMovementActionDocumentId))
                                      .ToListAsync(cancellationToken))
                        .GroupBy(m => m.Serial, comparer)
                        .ToDictionary(g => g.Key, g => g.ToList(), comparer);

        var movementIds = new List<int>();
        var withoutMovement = new List<string>();

        foreach (var serial in serials)
        {
            if (!movements.TryGetValue(serial, out var rows))
            {
                withoutMovement.Add(serial);
                continue;
            }

            // The movement of the given document; otherwise the latest movement of the serial.
            var matched = rows.Where(r => string.Equals(r.DocumentCode, documentCode, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matched.Count == 0)
            {
                matched.Add(rows.OrderByDescending(r => r.Id).First());
            }

            movementIds.AddRange(matched.Select(r => r.Id));
        }

        if (withoutMovement.Count > 0)
        {
            throw TeamYarHelper.Validation($"Serial(s) have no movement to verify: {TeamYarHelper.Quote(withoutMovement)}.");
        }

        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);
        var now = (DateTime?)DateTime.Now;
        var data = JsonSerializer.Serialize(new
        {
            request.CommandUniqueId,
            DocumentCode = documentCode,
            WarehouseCode = warehouseCode,
            OperationCode = operationCode
        });

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await context.TagsMovements
                         .Where(m => movementIds.Contains(m.TagsMovementId))
                         .ExecuteUpdateAsync(setters => setters
                             .SetProperty(m => m.ApiSendStatus, (int?)1)
                             .SetProperty(m => m.ApiSendUser, userId)
                             .SetProperty(m => m.ApiSendDateTime, now)
                             .SetProperty(m => m.ApiSendData, data), cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar VerifyLocate failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }
}
