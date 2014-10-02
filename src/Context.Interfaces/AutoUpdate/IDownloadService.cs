using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace Context.Interfaces.AutoUpdate
{
    [Guid("2DE3A841-C2D7-4f00-BD7F-E7577F1FF2FC")]
    public interface IDownloadService
    {
        void Download(string sourcePath, FileInfo targetFile, long sourceSize, byte[] md5Hash, DownloadStatusCallback statusCallback);

        CookieContainer CookieContainer { get; }

        void Start();

        void Stop();
    }
}
