using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SceneryChanger.Services
{
    public static class AbxCacheDecryptor
    {
        public static string CacheDir =>
            Path.Combine(Application.persistentDataPath, "AbxCache"); // writable per-user
        public static async Task<byte[]> DecryptAbxToBytesAsync(string abxPath, byte[] contentKey32)
        {
            using (var fs = File.Open(abxPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen: true))
            {
                var magic = br.ReadBytes(4);
                if (magic.Length != 4 || magic[0] != (byte)'A' || magic[1] != (byte)'B' || magic[2] != (byte)'X' || magic[3] != (byte)'1')
                    throw new InvalidDataException("ABX bad magic");
                int ver = fs.ReadByte(); if (ver != 1) throw new InvalidDataException("ABX unsupported version");
                var iv = br.ReadBytes(16);
                int clen = br.ReadInt32(); if (clen <= 0 || clen > fs.Length) throw new InvalidDataException("ABX length");

                // derive keys
                byte[] encKey32, macKey32;
                using (var sha = SHA256.Create())
                {
                    encKey32 = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("enc")));
                    macKey32 = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("mac")));
                }

                // verify MAC(header||cipher)
                fs.Position = 0;
                byte[] macCalc;
                using (var h = new HMACSHA256(macKey32))
                {
                    var header = new byte[25]; fs.Read(header, 0, header.Length);
                    h.TransformBlock(header, 0, header.Length, null, 0);

                    const int BUF = 1 << 20;
                    var buf = new byte[BUF];
                    int remaining = clen;
                    while (remaining > 0)
                    {
                        int n = await fs.ReadAsync(buf, 0, Math.Min(remaining, BUF));
                        if (n <= 0) throw new EndOfStreamException();
                        h.TransformBlock(buf, 0, n, null, 0);
                        remaining -= n;
                    }
                    h.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                    var macFile = br.ReadBytes(32);
                    macCalc = h.Hash;
                    if (!FixedEq(macCalc, macFile)) throw new InvalidDataException("ABX MAC check failed");
                }

                // decrypt cipher into memory
                fs.Position = 25; // after header
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                    aes.Key = encKey32; aes.IV = iv;

                    using (var dec = aes.CreateDecryptor())
                    using (var cs = new CryptoStream(new LimitStream(fs, clen), dec, CryptoStreamMode.Read))
                    using (var ms = new MemoryStream(capacity: clen))
                    {
                        var buf = new byte[1 << 20];
                        int n;
                        while ((n = cs.Read(buf, 0, buf.Length)) > 0)
                            ms.Write(buf, 0, n);
                        return ms.ToArray();
                    }
                }
            }

            // local helpers already exist in your code:
            // Concat(..), FixedEq(..)
        }
        public static async Task<string> EnsureDecryptedBundleAsync(string abxPath, byte[] contentKey32, string logicalBundleName = null)
        {
            Directory.CreateDirectory(CacheDir);

            // 1) Hashes we’ll use for naming + integrity buckets
            string abxSha = await SHA256FileAsync(abxPath).ConfigureAwait(false); // full hex
            string keySha = Hex(SHA256Bytes(contentKey32));

            string abx8 = abxSha.Substring(0, 8);
            string key8 = keySha.Substring(0, 8);

            // logical bundle name may be the "bundleName" you pass to BundleLoader
            string basis = string.IsNullOrEmpty(logicalBundleName)
                            ? Path.GetFileNameWithoutExtension(abxPath).ToLowerInvariant()
                            : logicalBundleName.ToLowerInvariant();

            string bundleId8 = Hex(SHA256Bytes(Encoding.UTF8.GetBytes(basis))).Substring(0, 8);

            // Optional: if exact match already exists, reuse it
            // (same abx bits + same key => same decrypted result)
            // We purposely do NOT use the plaintext bundle name anywhere.
            string existing = Directory.EnumerateFiles(CacheDir, $"{bundleId8}-{abx8}-{key8}-*.u3d")
                                       .FirstOrDefault();
            if (existing != null && File.Exists(existing))
                return existing;

            // 2) Decrypt streaming -> tmp file
            string rand8 = RandomToken8();
            string final = Path.Combine(CacheDir, $"{bundleId8}-{abx8}-{key8}-{rand8}.u3d");
            string tmpPath = final + ".tmp";

            try
            {
                using (var fs = File.OpenRead(abxPath))
                using (var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen: true))
                {
                    // --- header ---
                    var magic = br.ReadBytes(4);
                    if (magic.Length != 4 || magic[0] != (byte)'A' || magic[1] != (byte)'B' || magic[2] != (byte)'X' || magic[3] != (byte)'1')
                        throw new InvalidDataException("ABX bad magic");
                    int version = fs.ReadByte(); if (version != 1) throw new InvalidDataException("ABX unsupported version");
                    var iv = br.ReadBytes(16);
                    int clen = br.ReadInt32();
                    if (clen <= 0 || clen > fs.Length) throw new InvalidDataException("ABX cipher length invalid");
                    long cipherStart = fs.Position;

                    // --- derive keys ---
                    byte[] encKey32, macKey32;
                    using (var sha = SHA256.Create())
                    {
                        encKey32 = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("enc")));
                        macKey32 = sha.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("mac")));
                    }

                    // --- HMAC(header||cipher) ---
                    fs.Position = 0;
                    using (var h = new HMACSHA256(macKey32))
                    {
                        var header = new byte[25];
                        int headRead = fs.Read(header, 0, header.Length);
                        if (headRead != header.Length) throw new EndOfStreamException("ABX header truncated");
                        h.TransformBlock(header, 0, header.Length, null, 0);

                        const int BUF = 1 << 20;
                        var buf = new byte[BUF];
                        int remaining = clen;
                        while (remaining > 0)
                        {
                            int toRead = Math.Min(remaining, BUF);
                            int chunkRead = await fs.ReadAsync(buf, 0, toRead).ConfigureAwait(false);
                            if (chunkRead <= 0) throw new EndOfStreamException("ABX cipher truncated");
                            h.TransformBlock(buf, 0, chunkRead, null, 0);
                            remaining -= chunkRead;
                        }
                        h.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                        var macFile = br.ReadBytes(32);
                        if (!FixedEq(h.Hash, macFile)) throw new InvalidDataException("ABX MAC check failed");
                    }

                    // --- Decrypt -> tmp ---
                    fs.Position = cipherStart;
                    using (var aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                        aes.Key = encKey32; aes.IV = iv;

                        using (var dec = aes.CreateDecryptor())
                        using (var outFs = File.Create(tmpPath))
                        using (var cs = new CryptoStream(new LimitStream(fs, clen), dec, CryptoStreamMode.Read))
                        {
                            const int BUF = 1 << 20;
                            var buf = new byte[BUF];
                            int read;
                            while ((read = await cs.ReadAsync(buf, 0, buf.Length).ConfigureAwait(false)) > 0)
                                await outFs.WriteAsync(buf, 0, read).ConfigureAwait(false);
                        }
                    }
                }

                // 3) Atomic finalize, then prune siblings for same bundle
                File.Move(tmpPath, final);

                // mark hidden to make it less discoverable (optional)
                try { new FileInfo(final).Attributes |= FileAttributes.Hidden; } catch { }

                PruneOldBundleCaches(CacheDir, bundleId8, keepPath: final);
                return final;
            }
            catch
            {
                try { if (File.Exists(tmpPath)) File.Delete(tmpPath); } catch { }
                throw;
            }
        }

        // Delete all other files for this bundleId8
        static void PruneOldBundleCaches(string cacheDir, string bundleId8, string keepPath)
        {
            try
            {
                var keepFull = Path.GetFullPath(keepPath);
                foreach (var file in Directory.EnumerateFiles(cacheDir, bundleId8 + "-*.u3d"))
                {
                    var full = Path.GetFullPath(file);
                    if (!string.Equals(full, keepFull, StringComparison.OrdinalIgnoreCase))
                    {
                        try { File.Delete(full); } catch { /* best effort */ }
                    }
                }
            }
            catch { /* best effort */ }
        }

        // 8 hex chars from CSPRNG
        static string RandomToken8()
        {
            var b = new byte[4];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
            return BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();
        }
        // helper to await Tasks inside coroutines without blocking
        public static IEnumerator WaitTask<T>(Task<T> task) { while (!task.IsCompleted) yield return null; }

        public static Task<byte[]> DecryptAbxToBytesOffThread(string abxPath, byte[] contentKey32)
        {
            return Task.Run(() =>
            {
                using (var fs = new FileStream(abxPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen: true))
                using (var sha = SHA256.Create())
                {
                    var magic = br.ReadBytes(4);
                    if (magic.Length != 4 || magic[0] != (byte)'A' || magic[1] != (byte)'B' || magic[2] != (byte)'X' || magic[3] != (byte)'1')
                        throw new InvalidDataException("ABX bad magic");

                    int ver = fs.ReadByte(); if (ver != 1) throw new InvalidDataException("ABX unsupported version");
                    var iv = br.ReadBytes(16);
                    int clen = br.ReadInt32(); if (clen <= 0 || clen > fs.Length) throw new InvalidDataException("ABX length");

                    // derive keys
                    byte[] encKey32, macKey32;
                    using (var sha2 = SHA256.Create())
                    {
                        encKey32 = sha2.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("enc")));
                        macKey32 = sha2.ComputeHash(Concat(contentKey32, Encoding.ASCII.GetBytes("mac")));
                    }

                    // MAC(header||cipher)
                    fs.Position = 0;
                    byte[] macCalc;
                    using (var h = new HMACSHA256(macKey32))
                    {
                        var header = new byte[25]; fs.Read(header, 0, header.Length);
                        h.TransformBlock(header, 0, header.Length, null, 0);

                        var buf = new byte[1 << 20];
                        int remaining = clen;
                        while (remaining > 0)
                        {
                            int n = fs.Read(buf, 0, Math.Min(remaining, buf.Length));
                            if (n <= 0) throw new EndOfStreamException();
                            h.TransformBlock(buf, 0, n, null, 0);
                            remaining -= n;
                        }
                        h.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                        var macFile = br.ReadBytes(32);
                        macCalc = h.Hash;
                        if (!FixedEq(macCalc, macFile)) throw new InvalidDataException("ABX MAC check failed");
                    }

                    // decrypt to memory
                    fs.Position = 25;
                    using (var aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC; aes.Padding = PaddingMode.PKCS7;
                        aes.Key = encKey32; aes.IV = iv;
                        using (var dec = aes.CreateDecryptor())
                        using (var cs = new CryptoStream(new LimitStream(fs, clen), dec, CryptoStreamMode.Read))
                        using (var ms = new MemoryStream(clen))
                        {
                            var buf = new byte[1 << 20];
                            int n;
                            while ((n = cs.Read(buf, 0, buf.Length)) > 0)
                                ms.Write(buf, 0, n);
                            return ms.ToArray();
                        }
                    }
                }
            });
        }

        // ---- helpers ----
        static async Task<string> SHA256FileAsync(string path)
        {
            using (var sha = SHA256.Create())
            using (var fs = File.OpenRead(path))
            {
                byte[] hash = await Task.Run(() => sha.ComputeHash(fs));
                return Hex(hash);
            }
        }
        static byte[] SHA256Bytes(byte[] data) { using (var sha = SHA256.Create()) return sha.ComputeHash(data); }
        static string Hex(byte[] b) { var sb = new StringBuilder(b.Length * 2); foreach (var x in b) sb.Append(x.ToString("x2")); return sb.ToString(); }
        static bool FixedEq(byte[] a, byte[] b) { if (a == null || b == null || a.Length != b.Length) return false; int d = 0; for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i]; return d == 0; }
        static byte[] Concat(params byte[][] arrs) { int len = 0; foreach (var a in arrs) len += a.Length; var buf = new byte[len]; int o = 0; foreach (var a in arrs) { Buffer.BlockCopy(a, 0, buf, o, a.Length); o += a.Length; } return buf; }

        // Wrap a base stream to expose only 'len' bytes (so CryptoStream stops at cipher end)
        sealed class LimitStream : Stream
        {
            readonly Stream _s; long _remaining;
            public LimitStream(Stream s, long len) { _s = s; _remaining = len; }
            public override bool CanRead => true; public override bool CanSeek => false; public override bool CanWrite => false;
            public override long Length => _remaining; public override long Position { get => 0; set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) { if (_remaining <= 0) return 0; count = (int)Math.Min(count, _remaining); int n = _s.Read(buffer, offset, count); _remaining -= n; return n; }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken ct) { if (_remaining <= 0) return 0; count = (int)Math.Min(count, _remaining); int n = await _s.ReadAsync(buffer, offset, count, ct); _remaining -= n; return n; }
        }
    }
}
