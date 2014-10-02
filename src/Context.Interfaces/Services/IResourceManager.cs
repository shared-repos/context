using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Services
{
    [Guid("A4A1567F-84C2-4540-AFEE-15FF8EC6793B")]
    public interface IResourceManager
    {
        Image GetImage(int imageId);
        string GetString(string name, CultureInfo culture);
        Stream GetStream(string name, CultureInfo culture);
    }
}
