using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Saves, updates or revokes sale documents sent by TeamYar.
/// The document type is read from the "TeamYar:SaleDocumentType" setting.
/// </summary>
public class SaveDocumentForSaleHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , IConfiguration configuration
    , ILogger<SaveDocumentForSaleHandler> logger)
    : IRequestHandler<SaveDocumentForSaleCommand, SaveDocumentForSaleVm>, ITeamYarSecHandler
{
    private const string SaleDocumentTypeKey = "TeamYar:SaleDocumentType";

    public async Task<SaveDocumentForSaleVm> Handle(SaveDocumentForSaleCommand request, CancellationToken cancellationToken)
    {
        var headers = request.Documents;

        if (headers is null || headers.Count == 0 || headers.Any(h => h is null))
        {
            throw TeamYarHelper.Validation("Documents must contain at least one document and no empty document.");
        }

        var saleType = configuration[SaleDocumentTypeKey];

        if (string.IsNullOrWhiteSpace(saleType))
        {
            throw new InvalidOperationException($"The sale document type is not configured. Set '{SaleDocumentTypeKey}' in the application settings.");
        }

        var errors = new List<string>();
        var documents = new List<TeamYarDocument>();

        for (var index = 0; index < headers.Count; index++)
        {
            var header = headers[index];
            var prefix = $"Documents[{index}]";
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

                headerData["RefCode"] = TeamYarHelper.Clean(header.RefCode);
                headerData["Customer"] = TeamYarHelper.Clean(header.Customer);
            }

            documents.Add(new TeamYarDocument
            {
                Code = code,
                Type = saleType.Trim(),
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
                , TeamYarDocumentService.GetDocumentCheckType(configuration), "TeamYar sale", cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new() { Result = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            logger.LogError(ex, "TeamYar sale document failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }
}
