using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Security;

namespace Context
{
    internal class FileMonitor
    {
        private readonly FileSystemWatcher fileWatcher;
        private readonly object notificationLock;
        private readonly FileChangedHandler changed;
        private Thread notificationThread;
        private bool isChanged;

        public FileMonitor(string fileName, FileChangedHandler changed)
        {
            if (changed == null)
            {
                return;
            }

            this.changed = changed;
            notificationLock = new object();

            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(fileName);
            fileWatcher.Filter = Path.GetFileName(fileName);
            fileWatcher.Changed += new FileSystemEventHandler(FileSystemChanged);
            fileWatcher.EnableRaisingEvents = true;
        }

        public bool Enabled
        {
            get
            {
                return fileWatcher.EnableRaisingEvents;
            }
            set
            {
                fileWatcher.EnableRaisingEvents = value;
            }
        }

        private void FileSystemChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
            {
                lock (notificationLock)
                {
                    isChanged = true;
                    if (notificationThread != null)
                    {
                        return;
                    }

                    ThreadStart thread = delegate()
                    {
                        while (isChanged)
                        {
                            Thread.Sleep(100);

                            lock (notificationLock)
                            {
                                isChanged = false;
                            }

                            OnChanged();

                            lock (notificationLock)
                            {
                                if (!isChanged)
                                {
                                    notificationThread = null;
                                    break;
                                }
                            }
                        }
                    };

                    notificationThread = new Thread(thread);
                    notificationThread.Start();
                }
            }
        }

        private void OnChanged()
        {
            try
            {
                if (changed != null)
                {
                    changed();
                }
            }
            catch
            {
            }
        }

        public delegate void FileChangedHandler();
    }
}
