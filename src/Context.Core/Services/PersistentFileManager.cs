using System;
using System.IO;
using Context.Interfaces.Services;
using Context.Interfaces.Common;

namespace Context.Core
{
    internal class PersistentFileManager : IPersistentFileManager
    {
        private readonly string rootFolder;

        public PersistentFileManager(IServiceManager manager)
        {
            IProductInfo productInfo = (IProductInfo)manager.GetService(typeof(IProductInfo));
            rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), productInfo.DataFolder);
        }

        #region IPersistentFileManager Members

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        public IPersistentFile CreateFile(string moniker, int size, DateTime created, DateTime modified, FileAttributes fileAttributes)
        {
            moniker = moniker.Replace(":", "--");
            string fullPath = Path.Combine(rootFolder, moniker);
            // TODO: try find the existing file

            bool isDirty;
            if (File.Exists(fullPath))
            {
                int fSize = FileUtils.GetFileLength(fullPath);
                DateTime fCreated = File.GetCreationTime(fullPath);
                DateTime fModified = File.GetLastWriteTime(fullPath);
                FileAttributes fAttributes = File.GetAttributes(fullPath);
                isDirty = fSize != size || fCreated != created || fModified != modified || fAttributes != fileAttributes;
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                isDirty = true;
            }

            return new PersistentFile(this, fullPath, isDirty, size, created, modified, fileAttributes);
        }

        private void OnAttached(IPersistentFile file)
        {
            if (Attached != null)
            {
                Attached(file);
            }
        }

        private void OnChanged(IPersistentFile file)
        {
            if (Changed != null)
            {
                Changed(file);
            }
        }

        private void OnDetached(IPersistentFile file)
        {
            if (Detached != null)
            {
                Detached(file);
            }
        }

        public event PersistentFileEventHandler Attached;

        public event PersistentFileEventHandler Changed;

        public event PersistentFileEventHandler Detached;

        #endregion
    }
}
