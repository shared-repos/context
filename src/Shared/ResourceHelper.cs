using System;
using System.IO;
using System.Text;

namespace Context
{
    public static class ResourceHelper
    {
        public static byte[] GetResourceData(string name)
        {
            using (Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(name))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static string GetResourceString(string name)
        {
            using (Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(name))
            {
                TextReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        public static Stream GetResourceStream(string name)
        {
            using (Stream stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream(name))
            {
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return new MemoryStream(data);
            }
        }
    }
}
