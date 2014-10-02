using System;
using System.Collections.Generic;
using System.IO;

namespace Context.Interfaces.AutoUpdate
{
    public interface IDownloadRequestStatus
    {
        string SourcePath { get; }

        long SourceSize { get; }

        long SourceProgress { get; }

        FileInfo TargetFile { get; }

        Exception Exception { get; }

        DownloadStatus Status { get; }
    }
}
