using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class HashUtils
    {
        public static string ToHex(this byte[] bytes, int lenth)
        {
            StringBuilder result = new StringBuilder(lenth);

            var needBytes = (lenth / 2) + (lenth % 2);

            for (int i = 0; i < needBytes; i++)
            {
                var b = GetByte(bytes, i, needBytes);
                result.Append(b.ToString("x2"));
            }

            return result.ToString().Substring(0, lenth);
        }

        private static byte GetByte(byte[] bytes, int index, int needBytes)
        {
            var size = bytes.Length;
            if (size > needBytes)
            {
                var times = (size / needBytes) + 1;
                byte bRes = 0;
                for (int i = 0; i < times; i++)
                {
                    var xindex = index + (i * needBytes);
                    bRes += xindex < size ? bytes[xindex] : (byte)0;
                }
                return bRes;
            }
            else
            {
                var xindex = index % size;
                return bytes[xindex];
            }
        }

        public static string GetMD5Hash(this string text, int lenth = 32)
        {
            string dataHash;
            using (MD5 md5Hasher = MD5.Create())
            {
                var md5Hash = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(text));
                dataHash = md5Hash.ToHex(lenth);
            }
            return dataHash;
        }
    }
}
