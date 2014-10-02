using System;
using System.IO;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class PersistentFile : IPersistentFile
    {
        private readonly PersistentFileManager manager;
        private readonly string filePath;
        private bool isDirty;
        private int size;
        private DateTime created;
        private DateTime modified;
        private FileAttributes fileAttributes;

        public PersistentFile(PersistentFileManager manager, string filePath, bool isDirty, int size, DateTime created, DateTime modified, FileAttributes fileAttributes)
        {
            this.manager = manager;
            this.filePath = filePath;
            this.size = size;
            this.created = created;
            this.modified = modified;
            this.fileAttributes = fileAttributes;
            this.isDirty = isDirty;
            // TODO: start FileSystemWatcher
        }

        #region IPersistentFile Members

        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (isDirty == value)
                {
                    return;
                }

                if (value)
                {
                    isDirty = true;
                    // TODO: stop FileSystemWatcher
                }
                else
                {
                    if (File.Exists(filePath) && FileUtils.GetFileLength(filePath) == size)
                    {
                        File.SetCreationTime(filePath, created);
                        File.SetLastWriteTime(filePath, modified);
                        File.SetAttributes(filePath, fileAttributes);
                        // TODO: start FileSystemWatcher
                        isDirty = false;
                    }
                }
            }
        }

        #endregion
    }
}
