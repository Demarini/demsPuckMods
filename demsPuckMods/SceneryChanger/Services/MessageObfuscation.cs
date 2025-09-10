using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SceneryChanger.Services
{
    public class MessageObfuscation
    {
        public static string key = "knewdarklongcorrectlythirdgreatestspokenmotorvotes";
        public static string Encode(string plaintext)
        {
            if (plaintext == null) plaintext = "";
            byte[] data = Encoding.UTF8.GetBytes(plaintext);
            byte[] ks = KeyStream(key, data.Length);
            for (int i = 0; i < data.Length; i++) data[i] ^= ks[i];
            return Convert.ToBase64String(data);
        }

        public static string Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded)) return "";
            byte[] data = Convert.FromBase64String(encoded);
            byte[] ks = KeyStream(key, data.Length);
            for (int i = 0; i < data.Length; i++) data[i] ^= ks[i];
            return Encoding.UTF8.GetString(data);
        }

        static byte[] KeyStream(string key, int length)
        {
            if (key == null) key = string.Empty;
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (var sha = SHA256.Create())
            {
                byte[] seed = sha.ComputeHash(keyBytes);
                byte[] outBuf = new byte[length];
                int off = 0, ctr = 0;

                while (off < length)
                {
                    byte[] ctrBytes = BitConverter.GetBytes(ctr++); // little-endian
                    byte[] blockInput = new byte[seed.Length + ctrBytes.Length];
                    Buffer.BlockCopy(seed, 0, blockInput, 0, seed.Length);
                    Buffer.BlockCopy(ctrBytes, 0, blockInput, seed.Length, ctrBytes.Length);

                    byte[] block = sha.ComputeHash(blockInput);
                    int toCopy = Math.Min(block.Length, length - off);
                    Buffer.BlockCopy(block, 0, outBuf, off, toCopy);
                    off += toCopy;
                }
                return outBuf;
            }
        }
    }
}
