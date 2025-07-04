using System;
using System.IO;
using System.Security.Cryptography;

namespace ModsChecksum.Utils
{
    public static class FingerprintCalculator
    {
        public static string CalculateSHA1(string filePath)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static long CalculateCurseForgeFingerprint(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var ms = new MemoryStream())
            {
                int b;
                while ((b = fs.ReadByte()) != -1)
                {
                    if (b != 9 && b != 10 && b != 13 && b != 32)
                    {
                        ms.WriteByte((byte)b);
                    }
                }
                byte[] data = ms.ToArray();
                return MurmurHash2(data, 1);
            }
        }

        static uint MurmurHash2(byte[] data, uint seed)
        {
            const uint m = 0x5bd1e995;
            const int r = 24;
            int length = data.Length;

            if (length == 0)
                return 0;

            uint h = seed ^ (uint)length;
            int currentIndex = 0;

            while (length >= 4)
            {
                uint k = (uint)(data[currentIndex++] | data[currentIndex++] << 8 | data[currentIndex++] << 16 | data[currentIndex++] << 24);
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (uint)data[currentIndex] << 16;
                    h *= m;
                    break;
                case 2:
                    h ^= (ushort)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;
                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;
                default:
                    break;
            }

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }
    }
} 