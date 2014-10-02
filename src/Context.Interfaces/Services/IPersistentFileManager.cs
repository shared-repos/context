using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Context.Interfaces.Services
{
    [Guid("D85BDD50-932F-4ea6-B126-55A3E04AF9F3")]
    public interface IPersistentFileManager
    {
        void ClearAll();
        IPersistentFile CreateFile(string moniker, int size, DateTime created, DateTime modified, FileAttributes fileAttributes);

        event PersistentFileEventHandler Attached;
        event PersistentFileEventHandler Changed;
        event PersistentFileEventHandler Detached;
    }
}
