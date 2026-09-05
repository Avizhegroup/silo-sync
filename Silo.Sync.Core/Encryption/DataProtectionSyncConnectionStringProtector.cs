using Microsoft.AspNetCore.DataProtection;

namespace Silo.Sync.Core.Encryption;

public sealed class DataProtectionSyncConnectionStringProtector(IDataProtector dataProtector)
    : ISyncConnectionStringProtector
{
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
        {
            return plainText;
        }

        return Convert.ToBase64String(dataProtector.Protect(System.Text.Encoding.UTF8.GetBytes(plainText)));
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return cipherText;
        }

        var bytes = dataProtector.Unprotect(Convert.FromBase64String(cipherText));
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
