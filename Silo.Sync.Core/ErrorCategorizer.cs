using Microsoft.Data.SqlClient;

namespace Silo.Sync.Core;

/// <summary>
/// Represents the ErrorCategorizer class.
/// </summary>
public static class ErrorCategorizer
{
    /// <summary>
    /// Gets or sets the SyntaxError.
    /// </summary>
    public const string SyntaxError = "SyntaxError";
    /// <summary>
    /// Gets or sets the Truncation.
    /// </summary>
    public const string Truncation = "Truncation";
    /// <summary>
    /// Gets or sets the DuplicateKey.
    /// </summary>
    public const string DuplicateKey = "DuplicateKey";
    /// <summary>
    /// Gets or sets the NotNullViolation.
    /// </summary>
    public const string NotNullViolation = "NotNullViolation";
    /// <summary>
    /// Gets or sets the ConversionError.
    /// </summary>
    public const string ConversionError = "ConversionError";
    /// <summary>
    /// Gets or sets the Timeout.
    /// </summary>
    public const string Timeout = "Timeout";
    /// <summary>
    /// Gets or sets the Deadlock.
    /// </summary>
    public const string Deadlock = "Deadlock";
    /// <summary>
    /// Gets or sets the Other.
    /// </summary>
    public const string Other = "Other";

    public static string Categorize(Exception exception)
    {
        if (exception is SqlException sqlException)
        {
            return CategorizeSqlException(sqlException);
        }

        if (exception.InnerException is SqlException innerSql)
        {
            return CategorizeSqlException(innerSql);
        }

        return Other;
    }

    private static string CategorizeSqlException(SqlException sqlException)
    {
        var message = (sqlException.Message ?? string.Empty).ToLowerInvariant();
        var number = sqlException.Number;

        if (number is -2 or 0 or 1222)
        {
            return number switch
            {
                -2 => Timeout,
                1222 => Deadlock,
                _ => Other
            };
        }

        if (number == 2627 || number == 2601 || message.Contains("unique") || message.Contains("duplicate key"))
        {
            return DuplicateKey;
        }

        if (number == 515 || message.Contains("cannot insert the value null") || message.Contains("not null"))
        {
            return NotNullViolation;
        }

        if (number == 8152 || number == 2628 || message.Contains("truncat"))
        {
            return Truncation;
        }

        if (message.Contains("convert") || message.Contains("conversion"))
        {
            return ConversionError;
        }

        if (message.Contains("syntax") || message.Contains("incorrect syntax"))
        {
            return SyntaxError;
        }

        return Other;
    }
}
