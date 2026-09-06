using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Silo.Application.Features;
using Silo.Sync.Core.Models;

namespace Silo.Sync.Core.Upsert;

/// <summary>
/// Represents the ProductUpsertService class.
/// </summary>
public sealed class ProductUpsertService(ILogger<ProductUpsertService> logger) : IProductUpsertService
{
    /// <summary>
    /// UpsertOneAsync operation.
    /// </summary>
    public async Task<UpsertResult> UpsertOneAsync(SaveProductCommand command, string connectionString, CancellationToken cancellationToken = default)
    {
        var validationError = ValidateLengths(command);
        if (validationError is not null)
        {
            return new UpsertResult
            {
                Success = false,
                ErrorCategory = ErrorCategorizer.Truncation,
                ErrorMessage = validationError
            };
        }

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var exists = await ProductExistsAsync(connection, command.ProductCode, cancellationToken);

            if (exists)
            {
                await UpdateAsync(connection, command, cancellationToken);
            }
            else
            {
                await InsertAsync(connection, command, cancellationToken);
            }

            return new UpsertResult { Success = true };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to upsert product {ProductCode}", command.ProductCode);
            return new UpsertResult
            {
                Success = false,
                ErrorCategory = ErrorCategorizer.Categorize(ex),
                ErrorMessage = ex.Message
            };
        }
    }

    private static string? ValidateLengths(SaveProductCommand command)
    {
        if (!string.IsNullOrEmpty(command.ProductCode) && command.ProductCode.Length > 50)
        {
            return $"ProductCode exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductTitle) && command.ProductTitle.Length > 250)
        {
            return $"ProductTitle exceeds maximum length of 250 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductENTitle) && command.ProductENTitle.Length > 250)
        {
            return $"ProductENTitle exceeds maximum length of 250 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductTechnicalCode) && command.ProductTechnicalCode.Length > 50)
        {
            return $"ProductTechnicalCode exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductType) && command.ProductType.Length > 50)
        {
            return $"ProductType exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductStatus) && command.ProductStatus.Length > 50)
        {
            return $"ProductStatus exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductSize) && command.ProductSize.Length > 50)
        {
            return $"ProductSize exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductUnit) && command.ProductUnit.Length > 50)
        {
            return $"ProductUnit exceeds maximum length of 50 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductBrand) && command.ProductBrand.Length > 128)
        {
            return $"ProductBrand exceeds maximum length of 128 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductGroup) && command.ProductGroup.Length > 128)
        {
            return $"ProductGroup exceeds maximum length of 128 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductSubGroup) && command.ProductSubGroup.Length > 128)
        {
            return $"ProductSubGroup exceeds maximum length of 128 characters.";
        }

        if (!string.IsNullOrEmpty(command.ProductClass) && command.ProductClass.Length > 128)
        {
            return $"ProductClass exceeds maximum length of 128 characters.";
        }

        return null;
    }

    private static async Task<bool> ProductExistsAsync(SqlConnection connection, string productCode, CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand("SELECT COUNT(1) FROM tbl_Products WHERE ProductCode = @productCode", connection);
        command.Parameters.Add(new SqlParameter("@productCode", SqlDbType.NVarChar, 50) { Value = productCode });

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    private static async Task InsertAsync(SqlConnection connection, SaveProductCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO tbl_Products
            (ProductCode, ProductTitle, ProductENTitle, ProductPackValue, ProductPackWeight,
             ProductPackVolume, ProductCountInPack, ProductValue, ProductTechnicalCode, ProductProperties,
             ProductType, ProductStatus, ProductSize, ProductUnit, ProductRegUser, ProductRegDateTime,
             ProductGalleryId, fld_ProductGroup, fld_ProductBrand, fld_ProductIsActive, fld_ProductSubGroup, fld_ProductClass, ProductTechnicalData)
            VALUES
            (@ProductCode, @ProductTitle, @ProductENTitle, @ProductPackValue, @ProductPackWeight,
             @ProductPackVolume, @ProductCountInPack, @ProductValue, @ProductTechnicalCode, @ProductProperties,
             @ProductType, @ProductStatus, @ProductSize, @ProductUnit, @ProductRegUser, GETDATE(),
             @ProductGalleryId, @ProductGroup, @ProductBrand, @IsActive, @ProductSubGroup, @ProductClass, @ProductTechnicalData)
            """;

        await using var cmd = CreateCommand(connection, sql, command);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task UpdateAsync(SqlConnection connection, SaveProductCommand command, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE tbl_Products SET
                ProductTitle = @ProductTitle,
                ProductENTitle = @ProductENTitle,
                ProductPackValue = @ProductPackValue,
                ProductPackWeight = @ProductPackWeight,
                ProductPackVolume = @ProductPackVolume,
                ProductCountInPack = @ProductCountInPack,
                ProductValue = @ProductValue,
                ProductTechnicalCode = @ProductTechnicalCode,
                ProductProperties = @ProductProperties,
                ProductType = @ProductType,
                ProductStatus = @ProductStatus,
                ProductSize = @ProductSize,
                ProductUnit = @ProductUnit,
                ProductGalleryId = @ProductGalleryId,
                fld_ProductGroup = @ProductGroup,
                fld_ProductBrand = @ProductBrand,
                fld_ProductIsActive = @IsActive,
                fld_ProductSubGroup = @ProductSubGroup,
                fld_ProductClass = @ProductClass,
                ProductTechnicalData = @ProductTechnicalData
            WHERE ProductCode = @ProductCode
            """;

        await using var cmd = CreateCommand(connection, sql, command);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string sql, SaveProductCommand command)
    {
        var cmd = new SqlCommand(sql, connection)
        {
            CommandTimeout = 120
        };

        cmd.Parameters.Add(new SqlParameter("@ProductCode", SqlDbType.NVarChar, 50) { Value = command.ProductCode });
        cmd.Parameters.Add(new SqlParameter("@ProductTitle", SqlDbType.NVarChar, 250) { Value = (object?)command.ProductTitle ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductENTitle", SqlDbType.NVarChar, 250) { Value = (object?)command.ProductENTitle ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductPackValue", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = command.ProductPackValue });
        cmd.Parameters.Add(new SqlParameter("@ProductPackWeight", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = command.ProductPackWeight });
        cmd.Parameters.Add(new SqlParameter("@ProductPackVolume", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = command.ProductPackVolume });
        cmd.Parameters.Add(new SqlParameter("@ProductCountInPack", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = command.ProductCountInPack });
        cmd.Parameters.Add(new SqlParameter("@ProductValue", SqlDbType.Decimal) { Precision = 18, Scale = 4, Value = command.ProductValue });
        cmd.Parameters.Add(new SqlParameter("@ProductTechnicalCode", SqlDbType.NVarChar, 50) { Value = (object?)command.ProductTechnicalCode ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductProperties", SqlDbType.NVarChar, -1) { Value = (object?)command.ProductProperties ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductType", SqlDbType.NVarChar, 50) { Value = (object?)command.ProductType ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductStatus", SqlDbType.NVarChar, 50) { Value = (object?)command.ProductStatus ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductSize", SqlDbType.NVarChar, 50) { Value = (object?)command.ProductSize ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductUnit", SqlDbType.NVarChar, 50) { Value = (object?)command.ProductUnit ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductRegUser", SqlDbType.NVarChar, 50) { Value = DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductGalleryId", SqlDbType.Int) { Value = command.ProductGalleryId });
        cmd.Parameters.Add(new SqlParameter("@ProductGroup", SqlDbType.NVarChar, 128) { Value = (object?)command.ProductGroup ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductBrand", SqlDbType.NVarChar, 128) { Value = (object?)command.ProductBrand ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = command.IsActive });
        cmd.Parameters.Add(new SqlParameter("@ProductSubGroup", SqlDbType.NVarChar, 128) { Value = (object?)command.ProductSubGroup ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductClass", SqlDbType.NVarChar, 128) { Value = (object?)command.ProductClass ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ProductTechnicalData", SqlDbType.NVarChar, -1) { Value = (object?)command.ProductTechnicalData ?? DBNull.Value });

        return cmd;
    }
}
