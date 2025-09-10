using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Services
{
    public static class AbxLoader
    {
        public static AssetBundle LoadFromAbxPath(string abxPath, byte[] contentKey32)
        {
            var abxBytes = File.ReadAllBytes(abxPath);
            var raw = DecryptAbxToBytes(abxBytes, contentKey32);
            return AssetBundle.LoadFromMemory(raw);
        }

        public static byte[] DecryptAbxToBytes(byte[] abxBytes, byte[] contentKey32)
        {
            using (var ms = new MemoryStream(abxBytes))
            using (var br = new BinaryReader(ms))
            {
                // header
                byte[] magic = br.ReadBytes(4);
                if (magic.Length != 4 || magic[0] != (byte)'A' || magic[1] != (byte)'B' || magic[2] != (byte)'X' || magic[3] != (byte)'1')
                    throw new InvalidDataException("ABX bad magic");
                byte ver = br.ReadByte();
                if (ver != 1) throw new InvalidDataException("ABX unsupported version");
                byte[] iv = br.ReadBytes(16);
                int clen = br.ReadInt32();
                if (clen <= 0 || clen > abxBytes.Length) throw new InvalidDataException("ABX cipher length invalid");
                byte[] cipher = br.ReadBytes(clen);
                byte[] mac = br.ReadBytes(32);

                // derive enc/mac from contentKey32
                byte[] encKey, macKey;
                using (var sha = SHA256.Create())
                {
                    encKey = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("enc")));
                    macKey = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("mac")));
                }

                // verify HMAC over header||cipher
                int headerLen = 4 + 1 + 16 + 4;
                var header = new byte[headerLen];
                Buffer.BlockCopy(abxBytes, 0, header, 0, headerLen);
                byte[] mac2;
                using (var h = new HMACSHA256(macKey))
                {
                    h.TransformBlock(header, 0, header.Length, null, 0);
                    h.TransformFinalBlock(cipher, 0, cipher.Length);
                    mac2 = h.Hash;
                }
                if (!FixedEq(mac, mac2)) throw new InvalidDataException("ABX MAC check failed");

                // decrypt
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                    aes.Key = encKey; aes.IV = iv;
                    using (var dec = aes.CreateDecryptor())
                        return dec.TransformFinalBlock(cipher, 0, cipher.Length);
                }
            }
        }

        static bool FixedEq(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int d = 0; for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i];
            return d == 0;
        }
        static byte[] Concat(params byte[][] arrs)
        {
            int len = 0; for (int i = 0; i < arrs.Length; i++) len += arrs[i].Length;
            var buf = new byte[len]; int o = 0;
            for (int i = 0; i < arrs.Length; i++) { Buffer.BlockCopy(arrs[i], 0, buf, o, arrs[i].Length); o += arrs[i].Length; }
            return buf;
        }
    }
}
