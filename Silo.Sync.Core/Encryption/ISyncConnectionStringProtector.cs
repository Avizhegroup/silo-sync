namespace Silo.Sync.Core.Encryption;

public interface ISyncConnectionStringProtector
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
