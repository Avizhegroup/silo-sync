using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Silo.Application.Api.Features;

internal sealed record TeamYarDocumentItem(string ProductCode, decimal Count, string? Unit);

internal sealed class TeamYarDocument
{
    public string Code { get; init; } = string.Empty;

    /// <summary>The document type (action type code) stored in DocumentType.</summary>
    public string Type { get; init; } = string.Empty;

    public SaveDocumentForSaleOpType OpType { get; init; }

    public JObject HeaderData { get; init; } = new();

    public List<TeamYarDocumentItem> Items { get; init; } = new();
}

/// <summary>
/// Shared document logic of the TeamYar sale and general document handlers.
/// Save and Update replace the document (like the legacy import does) when its status allows updates,
/// Revoke removes the document the same way the legacy remove does and writes the document log.
/// </summary>
internal static class TeamYarDocumentService
{
    public static async Task<List<string>> ValidateAsync(WmsApiContext context
        , IReadOnlyList<TeamYarDocument> documents
        , CancellationToken cancellationToken)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var errors = new List<string>();

        foreach (var repeated in documents.GroupBy(d => d.Code, comparer).Where(g => g.Count() > 1))
        {
            errors.Add($"Document code '{repeated.Key}' is repeated in the request.");
        }

        foreach (var document in documents.Where(d => d.OpType != SaveDocumentForSaleOpType.Revoke))
        {
            if (document.Items.Count == 0)
            {
                errors.Add($"Document '{document.Code}': items are required for Save and Update.");
            }

            if (document.Items.Any(i => i.Count < 0))
            {
                errors.Add($"Document '{document.Code}': item counts must not be negative.");
            }
        }

        var productCodes = documents.SelectMany(d => d.Items).Select(i => i.ProductCode).Distinct(comparer).ToList();

        if (productCodes.Count > 0)
        {
            var known = (await context.Products.AsNoTracking()
                                      .Where(p => productCodes.Contains(p.Code))
                                      .Select(p => p.Code)
                                      .ToListAsync(cancellationToken))
                        .ToHashSet(comparer);

            var unknown = productCodes.Where(c => !known.Contains(c)).ToList();

            if (unknown.Count > 0)
            {
                errors.Add($"Product code(s) not found: {TeamYarHelper.Quote(unknown)}.");
            }
        }

        var codes = documents.Select(d => d.Code).Distinct(comparer).ToList();

        var existing = await context.DocumentHeaders.AsNoTracking()
                                    .Where(h => codes.Contains(h.Key))
                                    .Select(h => new { h.Key, h.DocumentType, h.DocumentStatus.IsUpdatePermitted })
                                    .ToListAsync(cancellationToken);

        foreach (var document in documents)
        {
            var header = existing.FirstOrDefault(e => string.Equals(e.Key, document.Code, StringComparison.OrdinalIgnoreCase)
                                                      && string.Equals(e.DocumentType, document.Type, StringComparison.OrdinalIgnoreCase));

            if (header is null && document.OpType == SaveDocumentForSaleOpType.Revoke)
            {
                errors.Add($"Document '{document.Code}' does not exist, so it can not be revoked.");
            }

            if (header is not null && !header.IsUpdatePermitted)
            {
                errors.Add($"Document '{document.Code}' can not be changed or revoked in its current status.");
            }
        }

        return errors;
    }

    /// <summary>
    /// Writes the documents. It must be called inside the handler's transaction.
    /// </summary>
    public static async Task ApplyAsync(WmsApiContext context
        , IReadOnlyList<TeamYarDocument> documents
        , string? userId
        , int documentCheckType
        , string source
        , CancellationToken cancellationToken)
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var now = DateTime.Now;
        var shamsiDate = PersianCalendarTools.GregorianToPersian(now);

        var firstStatusId = await context.DocumentStatuses.AsNoTracking()
                                         .OrderBy(s => s.Id)
                                         .Select(s => (int?)s.Id)
                                         .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new InvalidOperationException("No document status is defined.");

        var productCodes = documents.SelectMany(d => d.Items).Select(i => i.ProductCode).Distinct(comparer).ToList();

        var products = (await context.Products.AsNoTracking()
                                     .Where(p => productCodes.Contains(p.Code))
                                     .Select(p => new { p.Code, p.Title, p.ProductUnit })
                                     .ToListAsync(cancellationToken))
                       .GroupBy(p => p.Code, comparer)
                       .ToDictionary(g => g.Key, g => g.First(), comparer);

        foreach (var document in documents)
        {
            var code = document.Code;
            var type = document.Type;

            // Divided child documents are removed with their parent, like the legacy remove does.
            if (document.OpType == SaveDocumentForSaleOpType.Revoke)
            {
                var childKeys = await context.DocumentHeaders
                                             .Where(h => h.DivideParent == code && h.DocumentType == type)
                                             .Select(h => h.Key)
                                             .ToListAsync(cancellationToken);

                if (childKeys.Count > 0)
                {
                    await context.DocumentItems.Where(i => childKeys.Contains(i.Key!) && i.DocumentType == type).ExecuteDeleteAsync(cancellationToken);
                    await context.DocumentHeaders.Where(h => h.DivideParent == code && h.DocumentType == type).ExecuteDeleteAsync(cancellationToken);
                }
            }

            await context.DocumentItems.Where(i => i.Key == code && i.DocumentType == type).ExecuteDeleteAsync(cancellationToken);
            await context.DocumentHeaders.Where(h => h.Key == code && h.DocumentType == type).ExecuteDeleteAsync(cancellationToken);

            if (document.OpType == SaveDocumentForSaleOpType.Revoke)
            {
                context.DocumentLogs.Add(new DocumentLog
                {
                    Key = code,
                    DocumentType = type,
                    Status = -1,
                    UserId = userId,
                    DateTime = now,
                    ShamsiDate = shamsiDate,
                    EventType = (int)DocumentEventType.RemoveDocument,
                    Description = $"{source} revoke"
                });

                continue;
            }

            var items = document.Items.Select(i =>
            {
                products.TryGetValue(i.ProductCode, out var product);

                return new DocumentItem
                {
                    Key = code,
                    DocumentType = type,
                    DocumentType1 = "0",
                    DocumentType2 = "0",
                    ProductCode = i.ProductCode,
                    ProductTitle = product?.Title,
                    Count = i.Count,
                    ProductUnit = string.IsNullOrWhiteSpace(i.Unit) ? product?.ProductUnit : i.Unit,
                    ItemData = "{}"
                };
            }).ToList();

            context.DocumentHeaders.Add(new DocumentHeader
            {
                Key = code,
                UserId = userId,
                ImportType = ImportType.Api,
                FileName = source,
                DocumentType = type,
                DocumentType1 = "0",
                DocumentType2 = "0",
                ImportDateTime = now,
                Description = string.Empty,
                DocumentStatusId = firstStatusId,
                HeaderData = document.HeaderData.ToString(Newtonsoft.Json.Formatting.None),
                Parent = "0",
                AggStatus = 1,
                DocumentCheckType = documentCheckType,
                DocumentItems = items
            });

            context.DocumentLogs.Add(new DocumentLog
            {
                Key = code,
                DocumentType = type,
                Status = firstStatusId,
                UserId = userId,
                DateTime = now,
                ShamsiDate = shamsiDate,
                EventType = (int)DocumentEventType.InsertDocument,
                Description = $"{source} {document.OpType}"
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public static int GetDocumentCheckType(IConfiguration configuration)
        => int.TryParse(configuration["ProjectConfigs:WmsConfigs:DocumentRemain"], out var value) ? value : 1;
}
