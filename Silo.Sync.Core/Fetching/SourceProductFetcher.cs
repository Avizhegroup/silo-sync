using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Silo.Application.Features;
using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Fetching;

/// <summary>
/// Represents the SourceProductFetcher class.
/// </summary>
public sealed class SourceProductFetcher(ILogger<SourceProductFetcher> logger) : ISourceProductFetcher
{
    public async Task<IReadOnlyList<ProductRow>> FetchAsync(SyncSourceConfigDto source, DateTime? checkpoint, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source.ConnectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(source.Command);
        ArgumentException.ThrowIfNullOrWhiteSpace(source.FieldKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(source.FieldCheck);
        ArgumentException.ThrowIfNullOrWhiteSpace(source.FieldOrder);

        var commandText = $"""{source.Command} WHERE {source.FieldCheck} > @checkpoint ORDER BY {source.FieldOrder} DESC""";

        await using var connection = new SqlConnection(source.ConnectionString);
        await using var command = new SqlCommand(commandText, connection)
        {
            CommandTimeout = 120
        };
        var safeCheckpoint = checkpoint ?? new DateTime(1753, 1, 1);
        command.Parameters.Add(new SqlParameter("@checkpoint", SqlDbType.DateTime) { Value = safeCheckpoint });

        await connection.OpenAsync(cancellationToken);

        var dataTable = new DataTable();
        using var adapter = new SqlDataAdapter(command);
        adapter.Fill(dataTable);

        var rows = new List<ProductRow>(dataTable.Rows.Count);

        foreach (DataRow row in dataTable.Rows)
        {
            var rowKey = GetValue(row, source.FieldKey);
            var checkValue = GetValue(row, source.FieldCheck);

            if (!DateTime.TryParse(checkValue, out var checkDateTime))
            {
                checkDateTime = DateTime.MinValue;
            }

            var productCommand = MapDataRowToSaveProductCommand(row, rowKey);
            var rawPayload = JsonSerializer.Serialize(productCommand);

            rows.Add(new ProductRow
            {
                RowKey = rowKey,
                CheckValue = checkDateTime,
                RawPayload = rawPayload,
                Command = productCommand
            });
        }

        logger.LogInformation("Fetched {RowCount} rows from source {SourceKey}", rows.Count, source.SourceKey);

        return rows;
    }

    private static string GetValue(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
        {
            return string.Empty;
        }

        var value = row[columnName];
        return value is DBNull or null ? string.Empty : value.ToString() ?? string.Empty;
    }

    private static SaveProductCommand MapDataRowToSaveProductCommand(DataRow row, string productCode)
    {
        var command = new SaveProductCommand
        {
            ProductCode = productCode,
            ProductTitle = GetValue(row, "ProductTitle"),
            ProductENTitle = row.Table.Columns.Contains("ProductENTitle") ? NullIfEmpty(row["ProductENTitle"]) : null,
            ProductType = row.Table.Columns.Contains("ProductType") ? NullIfEmpty(row["ProductType"]) : null,
            ProductTechnicalCode = row.Table.Columns.Contains("ProductTechnicalCode") ? NullIfEmpty(row["ProductTechnicalCode"]) : null,
            ProductSize = row.Table.Columns.Contains("ProductSize") ? NullIfEmpty(row["ProductSize"]) : null,
            ProductStatus = row.Table.Columns.Contains("ProductStatus") ? NullIfEmpty(row["ProductStatus"]) : null,
            ProductUnit = row.Table.Columns.Contains("ProductUnit") ? NullIfEmpty(row["ProductUnit"]) : null,
            ProductBrand = row.Table.Columns.Contains("ProductBrand") ? NullIfEmpty(row["ProductBrand"]) : null,
            ProductGroup = row.Table.Columns.Contains("ProductGroup") ? NullIfEmpty(row["ProductGroup"]) : null,
            ProductSubGroup = row.Table.Columns.Contains("ProductSubGroup") ? NullIfEmpty(row["ProductSubGroup"]) : null,
            ProductClass = row.Table.Columns.Contains("ProductClass") ? NullIfEmpty(row["ProductClass"]) : null,
            IsActive = true
        };

        if (row.Table.Columns.Contains("ProductPackValue") && decimal.TryParse(NullIfEmpty(row["ProductPackValue"]), out var packValue))
        {
            command.ProductPackValue = packValue;
        }

        if (row.Table.Columns.Contains("ProductValue") && decimal.TryParse(NullIfEmpty(row["ProductValue"]), out var productValue))
        {
            command.ProductValue = productValue;
        }

        if (row.Table.Columns.Contains("ProductPackWeight") && decimal.TryParse(NullIfEmpty(row["ProductPackWeight"]), out var packWeight))
        {
            command.ProductPackWeight = packWeight;
        }

        if (row.Table.Columns.Contains("ProductPackVolume") && decimal.TryParse(NullIfEmpty(row["ProductPackVolume"]), out var packVolume))
        {
            command.ProductPackVolume = packVolume;
        }

        if (row.Table.Columns.Contains("ProductCountInPack") && decimal.TryParse(NullIfEmpty(row["ProductCountInPack"]), out var countInPack))
        {
            command.ProductCountInPack = countInPack;
        }

        return command;
    }

    private static string? NullIfEmpty(object value)
    {
        if (value is DBNull or null)
        {
            return null;
        }

        var text = value.ToString();
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
