using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Context
{
    public class GZipStreamHelper
    {
        public static byte[] CompressString(string data)
        {
            return Compress(Encoding.UTF8.GetBytes(data));
        }

        public static string DecompressString(byte[] data)
        {
            return Encoding.UTF8.GetString(Decompress(data));
        }

        public static byte[] Compress(byte[] data)
        {
            var ms = new MemoryStream();
            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(data, 0, data.Length);
            }

            return ms.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            var ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            var stream = new GZipStream(ms, CompressionMode.Decompress);
            var temp = new MemoryStream();
            var buffer = new byte[512];
            while (true)
            {
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    break;
                }
                else
                {
                    temp.Write(buffer, 0, read);
                }
            }
            stream.Close();
            return temp.ToArray();
        }
    }
}