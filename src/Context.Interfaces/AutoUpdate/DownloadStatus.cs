using System;

namespace Context.Interfaces.AutoUpdate
{
    public enum DownloadStatus
    {
        Initialized,
        Starting,
        Cancelling,
        Cancelled,
        Downloading,
        Invalid,
        Finished,
        Error,
        Reconnecting
    }
}
