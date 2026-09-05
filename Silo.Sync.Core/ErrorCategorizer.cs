using Microsoft.Data.SqlClient;

namespace Silo.Sync.Core;

public static class ErrorCategorizer
{
    public const string SyntaxError = "SyntaxError";
    public const string Truncation = "Truncation";
    public const string DuplicateKey = "DuplicateKey";
    public const string NotNullViolation = "NotNullViolation";
    public const string ConversionError = "ConversionError";
    public const string Timeout = "Timeout";
    public const string Deadlock = "Deadlock";
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
