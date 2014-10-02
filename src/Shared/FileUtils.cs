using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace Context
{
    internal static class FileUtils
    {
        public const int MemoryStreamLimit = 20000000; //(~20M)
        public const int FileBufferSize = 0x1000;
        public const int CopyBufferSize = 0x10000;

        public const int ERROR_SHARING_VIOLATION = 0x20;
        public const int ERROR_LOCK_VIOLATION = 0x21;
        public const int ERROR_HANDLE_DISK_FULL = 0x27;
        public const int ERROR_DISK_FULL = 0x70;

        public static string EntryFile
        {
            get
            {
                return Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName;
            }
        }

        public static string EntryDirectory
        {
            get
            {
                return Path.GetDirectoryName(EntryFile);
            }
        }

        public static FileSystemInfo GetFileSystemInfo(string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            if (fi.Exists)
            {
                return fi;
            }

            DirectoryInfo di = new DirectoryInfo(fullPath);
            if (di.Exists)
            {
                return di;
            }

            return null;
        }

        public static int GetFileLength(string fileName)
        {
            FileInfo info = new FileInfo(fileName);
            return (int)info.Length;
        }

        public static string GetAbsolutePath(string path)
        {
            if (path.StartsWith(".") && path.Length > 2)
            {
                string rootPath = Path.GetDirectoryName(EntryFile);
                path = Path.Combine(rootPath, path.Remove(0, 2));
            }

            return path;
        }

        public static string EnsureDirectory(string path)
        {
            string directory = Path.GetDirectoryName(path);
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
            {
                try
                {
                    di.Create();
                }
                catch (Exception)
                {
                    return path;
                }
            }

            path = Path.Combine(di.FullName, Path.GetFileName(path));
            return path;
        }

        public static void AddFileSecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            FileInfo fInfo = new FileInfo(FileName);
            FileSecurity fSecurity = fInfo.GetAccessControl();
            fSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            fInfo.SetAccessControl(fSecurity);
        }

        public static void RemoveFileSecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            FileInfo fInfo = new FileInfo(FileName);
            FileSecurity fSecurity = fInfo.GetAccessControl();
            fSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            fInfo.SetAccessControl(fSecurity);
        }

        public static void AddDirectorySecurity(string DirectoryName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo diInfo = new DirectoryInfo(DirectoryName);
            DirectorySecurity diSecurity = diInfo.GetAccessControl();
            diSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            diInfo.SetAccessControl(diSecurity);
        }

        public static void RemoveDirectorySecurity(string DirectoryName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo diInfo = new DirectoryInfo(DirectoryName);
            DirectorySecurity diSecurity = diInfo.GetAccessControl();
            diSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            diInfo.SetAccessControl(diSecurity);
        }

        public static byte[] ReadAllBytes(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                return data;
            }
        }

        public static void MapDrive(string driveName, string path)
        {
            if (!NativeMethods.DefineDosDevice(0, driveName, path))
            {
                throw new Win32Exception();
            }
        }

        public static void UnmapDrive(string driveName)
        {
            if (!NativeMethods.DefineDosDevice(2, driveName, null))
            {
                throw new Win32Exception();
            }
        }

        public static string GetDriveMapping(string driveName)
        {
            var sb = new StringBuilder(259);
            if (NativeMethods.QueryDosDevice(driveName, sb, sb.Capacity) == 0)
            {
                // Return empty string if the drive is not mapped
                int err = Marshal.GetLastWin32Error();
                if (err == 2)
                {
                    return string.Empty;
                }

                throw new Win32Exception();
            }

            return sb.ToString(4, sb.Length - 4);
        }

        public static bool CopyFile(string from, string to, int retryCount)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    File.Copy(from, to, true);
                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            return false;
        }

        public static bool DeleteFile(string path, int retryCount)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            return false;
        }

        public static void CopyAndDeleteFile(string from, string to, int retryCount)
        {
            DeleteOrMoveToTempFile(to);
            if (CopyFile(from, to, retryCount))
            {
                DeleteFile(from, retryCount);
            }
        }

        private static bool IsTheSameBase(string path1, string path2)
        {
            string root1 = Path.GetPathRoot(path1);
            string root2 = Path.GetPathRoot(path2);
            if (string.IsNullOrEmpty(root1) || string.IsNullOrEmpty(root2))
            {
                return false;
            }

            if (root1[0] == Path.DirectorySeparatorChar || root2[0] == Path.DirectorySeparatorChar)
            {
                return false;
            }

            return string.Compare(root1, root2, true) == 0;
        }

        public static string GetRecycleBinPath(string pathRoot)
        {
            if (string.IsNullOrEmpty(pathRoot))
            {
                pathRoot = Path.DirectorySeparatorChar.ToString();
            }
            else if (!pathRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                pathRoot = pathRoot + Path.DirectorySeparatorChar.ToString();
            }

            if (Directory.Exists(pathRoot + "$RECYCLE.BIN"))
            {
                return pathRoot + "$RECYCLE.BIN";
            }

            if (Directory.Exists(pathRoot + "RECYCLER"))
            {
                return pathRoot + "RECYCLER";
            }

            if (!Directory.Exists(pathRoot + "Temp"))
            {
                Directory.CreateDirectory(pathRoot + "Temp");
            }

            return pathRoot + "Temp";
        }

        public static string CreateTempDirectoryFor(string path)
        {
            var temp = Guid.NewGuid().ToString("n");
            string tempFolder = Path.Combine(Path.GetTempPath(), temp);
            if (!IsTheSameBase(tempFolder, path))
            {
                tempFolder = Path.Combine(GetRecycleBinPath(Path.GetPathRoot(path)), temp);
            }
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }

        public static void DeleteOrMoveToTempFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            try
            {
                File.Delete(fileName);
            }
            catch
            {
                try
                {
                    string tempFile = Path.Combine(CreateTempDirectoryFor(fileName), Path.GetFileName(fileName));
                    File.Move(fileName, tempFile);
                    return;
                }
                catch
                {
                }

                throw;
            }
        }

        public static void DeleteOrMoveToTempDirectory(string directoryName)
        {
            DeleteOrMoveToTempDirectory(directoryName, false);
        }

        public static void DeleteOrMoveToTempDirectory(string directoryName, bool deleteHiddenFiles)
        {
            directoryName = directoryName.TrimEnd(Path.DirectorySeparatorChar);
            if (!Directory.Exists(directoryName))
            {
                return;
            }

            try
            {
                if (deleteHiddenFiles)
                {
                    DeleteDirectoryWithHiddenFiles(directoryName);
                }
                else
                {
                    Directory.Delete(directoryName);
                }
            }
            catch
            {
                try
                {
                    string tempDir = Path.Combine(CreateTempDirectoryFor(directoryName), Path.GetFileName(directoryName));
                    Directory.Move(directoryName, tempDir);
                    return;
                }
                catch
                {
                }

                throw;
            }
        }

        public static void DeleteDirectoryWithHiddenFiles(string directoryName)
        {
            var di = new DirectoryInfo(directoryName);
            foreach (var fi in di.GetFiles())
            {
                if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    fi.Delete();
                }
            }

            di.Delete();
        }

        public static string GetAlternateFileName(string fullPath)
        {
            string fileNameWithoutExtension = Path.ChangeExtension(fullPath, "").TrimEnd('.');
            string extension = Path.GetExtension(fullPath);

            for (int i = 1; i <= 100; i++)
            {
                string newFileName = Path.ChangeExtension(fileNameWithoutExtension + " (" + i.ToString() + ")", extension);
                if (!File.Exists(newFileName))
                {
                    return newFileName;
                }
            }

            throw new Exception(string.Format("GetAlternateFileName failed for path {0}", fullPath));
        }

        public static Stream CreateTempStream(long limit)
        {
            if (limit < MemoryStreamLimit)
            {
                return new MemoryStream();
            }
            else
            {
                return File.Create(Path.GetTempFileName(), FileBufferSize, FileOptions.DeleteOnClose);
            }
        }

        public static void CopyStream(Stream source, Stream target)
        {
            CopyStream(source, target, null);
        }

        public static void CopyStream(Stream source, Stream target, Action<long> progress)
        {
            byte[] buffer = new byte[CopyBufferSize];
            while (true)
            {
                int readCount = source.Read(buffer, 0, CopyBufferSize);
                if (readCount == 0)
                {
                    target.Flush();
                    return;
                }

                target.Write(buffer, 0, readCount);

                if (progress != null)
                {
                    progress(target.Position);
                }
            }
        }

        private static class NativeMethods
        {
            [SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool DefineDosDevice(int flags, string devname, string path);

            [SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern int QueryDosDevice(string devname, StringBuilder buffer, int bufSize);
        }
    }
}
