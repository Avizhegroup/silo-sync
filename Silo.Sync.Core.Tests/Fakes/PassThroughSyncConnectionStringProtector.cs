using Silo.Sync.Core.Encryption;

namespace Silo.Sync.Core.Tests.Fakes;

public sealed class PassThroughSyncConnectionStringProtector : ISyncConnectionStringProtector
{
    public string Encrypt(string plainText) => plainText;
    public string Decrypt(string cipherText) => cipherText;
}
