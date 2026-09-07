namespace Silo.Sync.Core.Encryption;

/// <summary>
/// Defines operations for ISyncConnectionStringProtector.
/// </summary>
public interface ISyncConnectionStringProtector
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
