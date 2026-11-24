using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Saves, updates or revokes general documents (purchase receipt, transfer, consumption ...) sent by TeamYar.
/// The document type comes from the request.
/// </summary>
public class SaveDocumentForGeneralHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , IConfiguration configuration
    , ILogger<SaveDocumentForGeneralHandler> logger)
    : IRequestHandler<SaveDocumentForGeneralCommand, SaveDocumentForGeneralVm>, ITeamYarSecHandler
{
    public async Task<SaveDocumentForGeneralVm> Handle(SaveDocumentForGeneralCommand request, CancellationToken cancellationToken)
    {
        var headers = request.List;

        if (headers is null || headers.Count == 0 || headers.Any(h => h is null))
        {
            throw TeamYarHelper.Validation("List must contain at least one document and no empty document.");
        }

        var comparer = StringComparer.OrdinalIgnoreCase;
        var errors = new List<string>();
        var documents = new List<TeamYarDocument>();

        var warehouseCodes = headers
            .Where(h => h.OpType != SaveDocumentForSaleOpType.Revoke)
            .SelectMany(h => new[] { TeamYarHelper.Clean(h.OriginWarehouseCode), TeamYarHelper.Clean(h.DestinationWarehouseCode) })
            .Distinct(comparer)
            .ToList();

        var knownWarehouses = (await context.Warehouses.AsNoTracking()
                                            .Where(w => warehouseCodes.Contains(w.Code!))
                                            .Select(w => w.Code!)
                                            .ToListAsync(cancellationToken))
                              .ToHashSet(comparer);

        for (var index = 0; index < headers.Count; index++)
        {
            var header = headers[index];
            var prefix = $"List[{index}]";
            var code = TeamYarHelper.Clean(header.DocumentCode);

            if (code.Length == 0) errors.Add($"{prefix}: documentCode is required.");
            if (!Enum.IsDefined(header.OpType)) errors.Add($"{prefix}: opType is not valid.");

            var headerData = new JObject();

            if (header.OpType != SaveDocumentForSaleOpType.Revoke)
            {
                if (TeamYarHelper.TryParsePersianDate(header.Date, out var date))
                {
                    headerData["Date"] = PersianCalendarTools.GregorianToPersian(date);
                }
                else
                {
                    errors.Add($"{prefix}: date '{header.Date}' is not a valid Persian date (yyyy/MM/dd).");
                }

                var origin = TeamYarHelper.Clean(header.OriginWarehouseCode);
                var destination = TeamYarHelper.Clean(header.DestinationWarehouseCode);

                if (!knownWarehouses.Contains(origin)) errors.Add($"{prefix}: origin warehouse code '{header.OriginWarehouseCode}' does not exist.");
                if (!knownWarehouses.Contains(destination)) errors.Add($"{prefix}: destination warehouse code '{header.DestinationWarehouseCode}' does not exist.");

                headerData["OriginWarehouseCode"] = origin;
                headerData["DestinationWarehouseCode"] = destination;
            }

            documents.Add(new TeamYarDocument
            {
                Code = code,
                Type = header.DocumentType.ToString(),
                OpType = header.OpType,
                HeaderData = headerData,
                Items = (header.Items ?? new())
                    .Where(i => i is not null)
                    .Select(i => new TeamYarDocumentItem(TeamYarHelper.Clean(i.ProductCode), i.Count, i.Unit))
                    .ToList()
            });
        }

        if (errors.Count == 0)
        {
            errors.AddRange(await TeamYarDocumentService.ValidateAsync(context, documents, cancellationToken));
        }

        if (errors.Count > 0)
        {
            throw TeamYarHelper.Validation(errors);
        }

        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await TeamYarDocumentService.ApplyAsync(context, documents, userId
                , TeamYarDocumentService.GetDocumentCheckType(configuration), "TeamYar general", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar general document failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }
}
