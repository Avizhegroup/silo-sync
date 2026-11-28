using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Silo.Application.Exceptions;
using Silo.Application.Shared.Contracts;

namespace Silo.Application.Api.Features;

/// <summary>
/// Registers (or edits) the batch information of a serial: the print record is prepared here,
/// and the tag itself is created later when the RFID tag is written.
/// </summary>
public class SaveTagForTeamYarHandler(WmsApiContext context
    , IHttpContextAccessor httpContextAccessor
    , ILogger<SaveTagForTeamYarHandler> logger)
    : IRequestHandler<SaveTagForTeamYarCommand, SaveTagForTeamYarVm>, ITeamYarSecHandler
{
    private sealed record PreparedItem(SaveTagForTeamYarDto Dto, string Serial, Product Product, DateTime? Date);

    public async Task<SaveTagForTeamYarVm> Handle(SaveTagForTeamYarCommand request, CancellationToken cancellationToken)
    {
        var validationErrors = new List<ValidationResult>();

        if (!Validator.TryValidateObject(request, new ValidationContext(request), validationErrors, true))
        {
            throw new SiloValidationException(validationErrors);
        }

        var items = request.Data;

        if (items is null || items.Count == 0 || items.Any(i => i is null))
        {
            throw TeamYarHelper.Validation("Data must contain at least one item and no empty item.");
        }

        var comparer = StringComparer.OrdinalIgnoreCase;
        var errors = new List<string>();

        foreach (var repeated in items.GroupBy(i => TeamYarHelper.Clean(i.ProductSerial), comparer).Where(g => g.Count() > 1))
        {
            errors.Add($"Serial '{repeated.Key}' is repeated in the request.");
        }

        var serials = items.Select(i => TeamYarHelper.Clean(i.ProductSerial)).Distinct(comparer).ToList();
        var productCodes = items.Select(i => TeamYarHelper.Clean(i.ProductCode)).Distinct(comparer).ToList();
        var warehouseCodes = items.Select(i => TeamYarHelper.Clean(i.WarehouseCode)).Distinct(comparer).ToList();
        var typeCodes = items.Select(i => TeamYarHelper.Clean(i.ProductTypeCode)).Distinct(comparer).ToList();

        var products = (await context.Products.AsNoTracking()
                                     .Where(p => productCodes.Contains(p.Code))
                                     .ToListAsync(cancellationToken))
                       .GroupBy(p => p.Code, comparer)
                       .ToDictionary(g => g.Key, g => g.First(), comparer);

        var knownWarehouses = (await context.Warehouses.AsNoTracking()
                                            .Where(w => warehouseCodes.Contains(w.Code!))
                                            .Select(w => w.Code!)
                                            .ToListAsync(cancellationToken))
                              .ToHashSet(comparer);

        var knownTypes = (await context.ProductTypes.AsNoTracking()
                                       .Where(t => typeCodes.Contains(t.Code))
                                       .Select(t => t.Code)
                                       .ToListAsync(cancellationToken))
                         .ToHashSet(comparer);

        var existingPrints = (await context.Prints
                                           .Where(p => serials.Contains(p.ProductSerial!)
                                                       && (p.SoftDelete == 0 || p.SoftDelete == null))
                                           .ToListAsync(cancellationToken))
                             .GroupBy(p => p.ProductSerial!, comparer)
                             .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.Id).First(), comparer);

        var existingTags = (await context.Tags
                                         .Where(t => serials.Contains(t.ProductSerial))
                                         .ToListAsync(cancellationToken))
                           .GroupBy(t => t.ProductSerial, comparer)
                           .ToDictionary(g => g.Key, g => g.First(), comparer);

        var prepared = new List<PreparedItem>();

        for (var index = 0; index < items.Count; index++)
        {
            var dto = items[index];
            var prefix = $"Data[{index}]";
            var serial = TeamYarHelper.Clean(dto.ProductSerial);

            products.TryGetValue(TeamYarHelper.Clean(dto.ProductCode), out var product);

            if (serial.Length == 0) errors.Add($"{prefix}: productSerial is required.");
            if (product is null) errors.Add($"{prefix}: product code '{dto.ProductCode}' does not exist.");
            if (!knownWarehouses.Contains(TeamYarHelper.Clean(dto.WarehouseCode))) errors.Add($"{prefix}: warehouse code '{dto.WarehouseCode}' does not exist.");
            if (!knownTypes.Contains(TeamYarHelper.Clean(dto.ProductTypeCode))) errors.Add($"{prefix}: product type code '{dto.ProductTypeCode}' does not exist.");
            if (dto.OriginalCount < 0 || dto.SubCount < 0) errors.Add($"{prefix}: counts must not be negative.");

            DateTime? date = null;

            if (!string.IsNullOrWhiteSpace(dto.Date))
            {
                if (TeamYarHelper.TryParsePersianDate(dto.Date, out var parsed)) date = parsed;
                else errors.Add($"{prefix}: date '{dto.Date}' is not a valid Persian date (yyyy/MM/dd).");
            }

            if (product is not null)
            {
                if (existingTags.TryGetValue(serial, out var tag)
                    && !string.Equals(tag.ProductCode, product.Code, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"{prefix}: serial '{serial}' is already registered for another product.");
                }

                if (existingPrints.TryGetValue(serial, out var print)
                    && print.RegisterFlag == 1
                    && !string.Equals(print.ProductCode, product.Code, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"{prefix}: serial '{serial}' is already registered for another product.");
                }

                prepared.Add(new PreparedItem(dto, serial, product, date));
            }
        }

        if (errors.Count > 0)
        {
            throw TeamYarHelper.Validation(errors);
        }

        var userId = TeamYarHelper.CurrentUserId(httpContextAccessor);
        var now = DateTime.Now;

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in prepared)
            {
                var properties = GetProperties(item.Dto);

                if (existingPrints.TryGetValue(item.Serial, out var print))
                {
                    UpdatePrint(print, item, properties);
                }
                else
                {
                    context.Prints.Add(CreatePrint(item, properties, userId, now));
                }

                if (existingTags.TryGetValue(item.Serial, out var tag))
                {
                    if (item.Dto.OriginalCount.HasValue)
                    {
                        tag.ProductCount = item.Dto.OriginalCount;
                    }

                    tag.ProductType = TeamYarHelper.Clean(item.Dto.ProductTypeCode);
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
            logger.LogError(ex, "TeamYar SaveTag failed. CommandUniqueId: {CommandUniqueId}", request.CommandUniqueId);
            throw;
        }
    }

    private static List<KeyValuePair<string, string>> GetProperties(SaveTagForTeamYarDto dto)
    {
        var properties = new List<KeyValuePair<string, string>>();

        if (dto.PrintData is not null)
        {
            foreach (var field in dto.PrintData.Where(f => f is not null && !string.IsNullOrWhiteSpace(f.Title)))
            {
                properties.Add(new(field.Title.Trim(), field.Value ?? string.Empty));
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.UnitForSubCount))
        {
            properties.Add(new(TeamYarHelper.SubCountUnitPropertyKey, dto.UnitForSubCount.Trim()));
        }

        return properties;
    }

    private static Print CreatePrint(PreparedItem item
        , List<KeyValuePair<string, string>> properties
        , string? userId
        , DateTime now)
    {
        var dto = item.Dto;
        var product = item.Product;

        return new Print
        {
            ProductSerial = item.Serial,
            ProductCode = product.Code,
            ProductRegCode = product.TechnicalCode,
            ProductName = product.Title,
            ProductDescription = product.ENTitle,
            ProductType = TeamYarHelper.Clean(dto.ProductTypeCode),
            ProductSize = product.ProductSize,
            ProductUnit = dto.UnitForOriginalCount ?? product.ProductUnit,
            ProductCount = dto.OriginalCount ?? product.PackValue,
            ProductItemCount = dto.SubCount ?? product.ProductValue,
            ProductCountInPack = product.CountInPack,
            ProductWeight = product.PackWeight,
            ProductVolume = product.PackVolume,
            ProductStatus = product.ProductQc,
            ProductGroup = product.ProductGroup,
            ProductBrand = product.ProductBrand,
            ProductSubGroup = product.ProductSubGroup,
            ProductClass = product.ProductClass,
            ProductProperties = TeamYarHelper.MergeProperties(product.ProductProperties, properties),
            ProductProductionLine = "0",
            ProductProductionShift = "0",
            ProductContractType = dto.DocumentType,
            DocumentId = TeamYarHelper.Clean(dto.DocumentCode),
            DocumentItemId = "0",
            DestinationCode = TeamYarHelper.Clean(dto.WarehouseCode),
            Location = "0",
            PrintActionId = 0,
            PrintActionDateTime = item.Date ?? now,
            PrintUser = userId,
            PrintType = "0",
            PrintFlag = 0,
            RegisterFlag = 0,
            RegisterType = 0,
            InputFileName = string.Empty,
            ErrorTime = string.Empty,
            ErrorDesc = string.Empty,
            SoftDelete = 0,
            ReRegister = false,
            PrintQueue = false,
            OfflineEpc = string.Empty,
            OfflineRegStatus = 0,
            OfflineRegDate = "0",
            ProjectCode = "0",
            TagEpc = string.Empty,
            PackageId = 0,
            Manufacturer = string.Empty
        };
    }

    private static void UpdatePrint(Print print, PreparedItem item, List<KeyValuePair<string, string>> properties)
    {
        var dto = item.Dto;

        print.ProductType = TeamYarHelper.Clean(dto.ProductTypeCode);
        print.DocumentId = TeamYarHelper.Clean(dto.DocumentCode);
        print.ProductContractType = dto.DocumentType;

        if (dto.OriginalCount.HasValue) print.ProductCount = dto.OriginalCount;
        if (dto.SubCount.HasValue) print.ProductItemCount = dto.SubCount;
        if (!string.IsNullOrWhiteSpace(dto.UnitForOriginalCount)) print.ProductUnit = dto.UnitForOriginalCount;

        print.ProductProperties = TeamYarHelper.MergeProperties(print.ProductProperties, properties);

        // After the RFID registration the warehouse belongs to the tag, so it is only editable before it.
        if (print.RegisterFlag != 1)
        {
            print.DestinationCode = TeamYarHelper.Clean(dto.WarehouseCode);

            if (item.Date.HasValue) print.PrintActionDateTime = item.Date;
        }
    }
}
