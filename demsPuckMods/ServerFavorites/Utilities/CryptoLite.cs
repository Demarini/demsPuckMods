using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

internal static class CryptoLite
{
    // Change these to your own random bytes/pepper.
    private static readonly byte[] Salt = new byte[] {
        0x31,0xA7,0x5B,0x22,0x9C,0x4D,0xE1,0x08,0xC3,0x5E,0x90,0x2B,0x77,0x19,0xAF,0xD2
    };
    private const int Iterations = 10000;
    private const int KeyBytes = 32;   // AES-256
    private const int IvBytes = 16;   // AES block size
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("SBF1"); // file header

    private static byte[] DeriveKey()
    {
        // “Key” tied to current machine/user but still easy to bypass with dnSpy – that’s fine.
        var device = SystemInfo.deviceUniqueIdentifier;
        if (string.IsNullOrEmpty(device)) device = Environment.UserName ?? "user";
        var password = device + "|YourModPepper|v1";

        using (var kdf = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256))
            return kdf.GetBytes(KeyBytes);
    }

    public static string EncryptToBase64(string plainText)
    {
        var key = DeriveKey();
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.GenerateIV();

            var ms = new MemoryStream();
            // write header + IV first
            ms.Write(Magic, 0, Magic.Length);
            ms.Write(aes.IV, 0, aes.IV.Length);

            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
                sw.Write(plainText);

            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public static string DecryptAuto(string text)
    {
        // Legacy plaintext (starts with '{' or '[') -> return as-is
        var trimmed = text?.TrimStart();
        if (!string.IsNullOrEmpty(trimmed) && (trimmed.StartsWith("{") || trimmed.StartsWith("[")))
            return text;

        // Otherwise expect Base64(MAGIC + IV + CIPHERTEXT)
        byte[] buf = Convert.FromBase64String(text);
        if (buf.Length < Magic.Length + IvBytes) throw new Exception("Encrypted payload too small");

        for (int i = 0; i < Magic.Length; i++)
            if (buf[i] != Magic[i]) throw new Exception("Invalid header");

        var iv = new byte[IvBytes];
        Buffer.BlockCopy(buf, Magic.Length, iv, 0, IvBytes);

        var cipher = new byte[buf.Length - Magic.Length - IvBytes];
        Buffer.BlockCopy(buf, Magic.Length + IvBytes, cipher, 0, cipher.Length);

        var key = DeriveKey();
        using (var aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            var ms = new MemoryStream(cipher);
            var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }
    }
}