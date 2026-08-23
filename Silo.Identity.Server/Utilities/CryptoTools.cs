using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Silo.Identity.Server.Utilities;
public static class CryptoTools
{
    public static SigningCredentials GetJwtCredential(string key)
    {
        SymmetricSecurityKey symmetricKey = GetSymmetricKey(key);

        return new(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
    }

    public static SymmetricSecurityKey GetSymmetricKey(string passKey)
    {
        var key = Encoding.UTF8.GetBytes(passKey);

        return new(key);
    }

    public static string GetTokenInSHA256(string value)
    {
        var passArray = Encoding.UTF8.GetBytes(value);

        var hash256Array = SHA256.HashData(passArray);
        var sb = new StringBuilder();

        for (int i = 0; i < hash256Array.Length; i++)
        {
            sb.Append(hash256Array[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static string GetHashedStringSha256StringBuilder(string data)
    {
        using (var sha256 = SHA256.Create())
        {
            var byteHash = sha256.
                ComputeHash(Encoding.UTF8.GetBytes(data));
            var sb = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
            {
                sb.Append(byteHash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public static bool ValidatePasswordInSHA256(string passHash, string password)
    {
        var passArray = Encoding.UTF8.GetBytes(password);
        var hash256Array = SHA256.HashData(passArray);
        var sb = new StringBuilder();
        for (int i = 0; i < hash256Array.Length; i++)
        {
            sb.Append(hash256Array[i].ToString("x2"));
        }
        var hash256 = sb.ToString();

        if (hash256.Equals(passHash))
        {
            return true;
        }

        return false;
    }

    public static bool ValidatePasswordInRfc2898Derive(string hashString, string unHashString)
    {
        byte[] buffer4;

        if (hashString == null)
        {
            return false;
        }

        if (unHashString == null)
        {
            return false;
        }

        byte[] src = Convert.FromBase64String(hashString);

        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }

        byte[] dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        byte[] buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);

        using (Rfc2898DeriveBytes bytes = new(unHashString, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }

        if (buffer3.Length != buffer4.Length)
        {
            return false;
        }

        for (int i = 0; i < buffer3.Length; i++)
        {
            if (buffer3[i] != buffer4[i])
            {
                return false;
            }
        }

        return true;
    }
}
