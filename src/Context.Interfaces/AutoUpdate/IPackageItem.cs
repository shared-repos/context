using System;
using System.IO;

namespace Context.Interfaces.AutoUpdate
{
    public interface IPackageItem
    {
        string DownloadPath { get; }

        FileInfo TargetFileInfo { get; }

        FileInfo UpdateFileInfo { get; }

        byte[] AssemblyPublicKey { get; }

        byte[] MD5Hash { get; }

        Version FileVersion { get; }

        long FileSize { get; }

        bool IsRestartRequired { get; }
    }
}
