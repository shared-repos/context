using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Context.AutoUpdate.AutoUpdateServiceReference;
using Context.Interfaces.AutoUpdate;
using Context.Interfaces.Common;
using Context.Interfaces.Configuration;
using Context.Interfaces.Data;
using Context.Interfaces.Logging;
using Context.Interfaces.Schema;
using Context.Interfaces.Services;

namespace Context.AutoUpdate
{
    internal class AutoUpdateService : IAutoUpdateService
    {
        private const string UpdateArgumentsSubKey = @"Update\Arguments";

        private const string UpdateSignalSettings = "UpdateSignal";
        private const string UIExecutablePathSettings = "UIExecutablePath";
        private const string CheckIntervalSettings = "CheckInterval";
        private const string CheckOnStartSettings = "CheckOnStart";
        private const string StartIntervalSettings = "StartInterval";
        private const string ErrorIntervalSettings = "ErrorInterval";
        private const string InstallIntervalSettings = "InstallInterval";
        private const string ServiceUrlSettings = "ServiceUrl";
        private const string CreatedSettings = "CustomerService.Created";
        private const string CustomerTicket = "CustomerService.CustomerTicket";
        private const string UpdateServicePath = "UpdateServicePath";
        private const string DataSourceSettings = "DataSource";
        private const string UpdateHistory = "UpdateHistory";
        private const string StateInfoString = "State";
        private const string SchemaResourceName = "Context.AutoUpdate.Resources.UpdateHistory.xsd";
        private const string InvalidFile = "Invalid downloaded file: {0}";

        private const string ServiceNameKey = "ServiceName";
        private const string TempFileKey = "TempFile";
        private const string UIExecutablePathKey = "UIExecutablePath";
        private const string IsRestartRequiredKey = "IsRestartRequired";
        private const string IsRestartUIRequiredKey = "IsRestartUIRequired";

        private const int PackageIdFieldIndex = 0;
        private const int PackageFieldIndex = 1;
        private const int InstalledOnFieldIndex = 2;
        private const int StatusFieldIndex = 3;
        private const int ErrorMessageFieldIndex = 4;

        private const int DefaultTimeout = 300000;

        private readonly IContext context;
        private readonly ILogService logger;
        private readonly IDownloadService downloader;
        private readonly IDataSource dataSource;
        private readonly IDataTable updateHistory;
        private readonly int checkInterval;
        private readonly int startInterval;
        private readonly int errorInterval;
        private readonly bool checkOnStart;
        private readonly string updateSignal;
        private readonly string uiExecutablePath;
        private readonly string executablePath;
        private readonly string serviceUrl;
        private readonly string updateServicePath;
        private readonly ICustomerService customerService;
        private readonly DateTime installedAt;
        private readonly double installInterval;
        private readonly object timerLock;
        private AutoUpdateServiceReference.AutoUpdate service;
        private SystemInfo system;
        private Timer timer;

        public AutoUpdateService(IContext context)
        {
            this.timerLock = new object();
            this.context = context;
            this.logger = (ILogService)context.GetService(typeof(ILogService));
            this.downloader = (IDownloadService)context.GetService(typeof(IDownloadService));
            this.customerService = (ICustomerService)context.GetService(typeof(ICustomerService));

            string source = Convert.ToString(context[DataSourceSettings]);
            ISchemaService schemaService = (ISchemaService)context.GetService(typeof(ISchemaService));
            schemaService.RegisterSchema(source, ResourceHelper.GetResourceString(SchemaResourceName));
            IDataService dataService = (IDataService)context.GetService(typeof(IDataService));
            dataSource = dataService.GetDataSource(source);
            updateHistory = dataSource.GetTable(UpdateHistory);

            double checkIntervalInHours = Convert.ToDouble(context[CheckIntervalSettings], CultureInfo.InvariantCulture);
            checkInterval = (int)TimeSpan.FromHours(checkIntervalInHours).TotalMilliseconds;
            double startIntervalInMinutes = Convert.ToDouble(context[StartIntervalSettings], CultureInfo.InvariantCulture);
            startInterval = (int)TimeSpan.FromMinutes(startIntervalInMinutes).TotalMilliseconds;
            double errorIntervalInMinutes = Convert.ToDouble(context[ErrorIntervalSettings], CultureInfo.InvariantCulture);
            errorInterval = (int)TimeSpan.FromMinutes(errorIntervalInMinutes).TotalMilliseconds;
            checkOnStart = Convert.ToBoolean(context[CheckOnStartSettings], CultureInfo.InvariantCulture);
            updateSignal = Convert.ToString(context[UpdateSignalSettings]);
            uiExecutablePath = FileUtils.GetAbsolutePath(Convert.ToString(context[UIExecutablePathSettings]));
            executablePath = FileUtils.EntryFile;
            updateServicePath = FileUtils.GetAbsolutePath(Convert.ToString(context[UpdateServicePath]));
            serviceUrl = Convert.ToString(context[ServiceUrlSettings]);

            IConfigurationManager config = (IConfigurationManager)context.GetService(typeof(IConfigurationManager));
            string installedStr = Convert.ToString(config[CreatedSettings]);
            string ticketStr = Convert.ToString(config[CustomerTicket]);
            if (!DateTime.TryParse(installedStr, out installedAt) && string.IsNullOrEmpty(installedStr) && string.IsNullOrEmpty(ticketStr))
            {
                installedAt = DateTime.Now;
            }

            string installIntervalStr = Convert.ToString(context[InstallIntervalSettings]);
            if (!double.TryParse(installIntervalStr, NumberStyles.Float, CultureInfo.InvariantCulture, out installInterval) && string.IsNullOrEmpty(installIntervalStr))
            {
                installInterval = 1;
            }

            SetSystemInfo();
            CreateServiceReference();
        }

        private void CreateServiceReference()
        {
            service = new AutoUpdateServiceReference.AutoUpdate();
            service.Url = serviceUrl;
            service.CookieContainer = downloader.CookieContainer;
            service.Timeout = DefaultTimeout;
        }

        private void CheckSuccessUpdate()
        {
            try
            {
                foreach (IDataRow row in updateHistory)
                {
                    if (Convert.ToString(row[StatusFieldIndex]) == UpdateStatus.Ready.ToString())
                    {
                        Package package = DeserializePackage(Convert.ToString(row[PackageFieldIndex]));
                        EnsurePackageItemFolders(package);
                        string errors;
                        bool success = CheckItemsSuccess(package, out errors);
                        if (success)
                        {
                            row[StatusFieldIndex] = UpdateStatus.Success.ToString();
                        }
                        else
                        {
                            row[StatusFieldIndex] = UpdateStatus.Error.ToString();
                            row[ErrorMessageFieldIndex] = errors;
                        }

                        updateHistory.Update(row);
                        dataSource.Flush();

                        if (success)
                        {
                            logger.Log(LogLevel.Info, context, "Packege {0} has been installed.", package.Id);
                        }
                        else
                        {
                            logger.Log(LogLevel.Error, context, "Error installing packege {0}: {1}", package.Id, errors);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in CheckSuccessUpdate: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private bool CheckItemsSuccess(Package package, out string errors)
        {
            StringBuilder sb = new StringBuilder();

            bool result = true;
            foreach (PackageItem item in package.Items)
            {
                if (string.IsNullOrEmpty(item.TargetPath))
                {
                    continue;
                }

                if (item.TargetFileInfo.Exists || !CheckFile(item.UpdateFileInfo, item.FileVersion, item.MD5Hash, item.AssemblyPublicKey))
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(item.TargetPath);
                    result = false;
                }
            }

            errors = sb.ToString();
            return result;
        }

        private Cookie CreateCustomerCookie(string customerTicket)
        {
            Cookie cookie = new Cookie(".CustomerTicket", customerTicket);
            cookie.Expires = DateTime.Now.AddDays(1000);
            cookie.Expired = false;
            cookie.Path = "/";
            Uri uri = new Uri(serviceUrl);
            cookie.Domain = uri.Authority;
            return cookie;
        }

        private ProductInfo GetProductInfo()
        {
            ProductInfo product = new ProductInfo();
            IProductInfo info = (IProductInfo)context.GetService(typeof(IProductInfo));

            product.BuildTypeName = info.BuildType.ToString();
            product.CultureName = info.CultureName;
            product.Edition = info.Edition;
            product.ProductName = info.ProductName;
            product.ProductVersionLabel = info.ProductVersion.ToString();
            product.ProductId = info.ProductId;
            product.InstalledAt = installedAt.ToString("O");
            product.State = info.GetInfoString(StateInfoString);
            return product;
        }

        private void SetSystemInfo()
        {
            system = new SystemInfo();
            ISystemInfo info = (ISystemInfo)context.GetService(typeof(ISystemInfo));

            system.ComponentInfos = info.ComponentInfos;
            system.CultureName = info.CultureName;
            system.SystemName = info.System.ToString();
            system.SystemVersionLabel = info.SystemVersion.ToString();
        }

        private void EnsurePackageItemFolders(Package package)
        {
            foreach (PackageItem item in package.Items)
            {
                item.TargetPath = FileUtils.EnsureDirectory(FileUtils.GetAbsolutePath(item.TargetPath));
                item.UpdatePath = FileUtils.EnsureDirectory(FileUtils.GetAbsolutePath(item.UpdatePath));
            }
        }

        private string SerializePackage(Package package)
        {
            return JsonConvert.SerializeObject(package);
        }

        private Package DeserializePackage(string text)
        {
            if (text.StartsWith("<"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Package));
                return (Package)serializer.Deserialize(new StringReader(text));
            }
            else
            {
                return JsonConvert.DeserializeObject<Package>(text);
            }
        }

        private void CheckPackageState(PackageState packageState)
        {
            lock (packageState.SyncObj)
            {
                Package package = packageState.Package;
                IDataRow historyRow = CreateOrUpdateHistoryRow(package);

                string status = Convert.ToString(historyRow[StatusFieldIndex]);
                if (status == UpdateStatus.Success.ToString() || status == UpdateStatus.Ready.ToString())
                {
                    return;
                }

                bool ready = CheckDownloadsReady(packageState, historyRow);
                if (!ready)
                {
                    return;
                }

                PackageReady(package, historyRow);

                bool isRestartRequired = package.IsRestartRequired || package.IsRestartUIRequired;
                if (!isRestartRequired)
                {
                    PackageUpdate(package, historyRow);
                    return;
                }

                StartUpdateThread(package);
            }
        }

        private void PackageUpdate(Package package, IDataRow historyRow)
        {
            foreach (PackageItem item in package.Items)
            {
                UpdatePackageItem(item);
            }

            historyRow[StatusFieldIndex] = UpdateStatus.Success.ToString();
            updateHistory.Update(historyRow);
            dataSource.Flush();
        }

        private void UpdatePackageItem(PackageItem item)
        {
            File.Copy(item.TargetPath, item.UpdatePath, true);
            File.Delete(item.TargetPath);
        }

        private void PackageReady(Package package, IDataRow historyRow)
        {
            historyRow[StatusFieldIndex] = UpdateStatus.Ready.ToString();
            updateHistory.Update(historyRow);
            dataSource.Flush();

            OnReady(package);
        }

        private void StartUpdateThread(Package package)
        {
            new Thread(delegate()
            {
                try
                {
                    string tempFile = Path.GetTempFileName();
                    using (StreamWriter writer = File.CreateText(tempFile))
                    {
                        foreach (PackageItem item in package.Items)
                        {
                            if (item.IsRestartRequired)
                            {
                                writer.WriteLine(item.TargetPath);
                                writer.WriteLine(item.UpdatePath);
                            }
                            else
                            {
                                UpdatePackageItem(item);
                            }
                        }
                    }

                    IProductInfo productInfo = (IProductInfo)context.GetService(typeof(IProductInfo));
                    bool portable = false;
                    try
                    {
                        SystemUtils.RegistrySetValues(Path.Combine(productInfo.OptionsRoot, UpdateArgumentsSubKey),
                            ServiceNameKey, productInfo.ApplicationName,
                            TempFileKey, tempFile,
                            UIExecutablePathKey, uiExecutablePath,
                            IsRestartRequiredKey, package.IsRestartRequired.ToString(),
                            IsRestartUIRequiredKey, package.IsRestartUIRequired.ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, context, "Error in StartUpdateThread: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                        logger.Log(LogLevel.Info, context, "Trying to update with portable flow");
                        portable = true;
                    }

                    if (package.IsRestartUIRequired)
                    {
                        SystemUtils.SetEventWaitHandle(updateSignal);
                    }

                    if (portable)
                    {
                        StartPortableUpdateProcess(tempFile, package.IsRestartRequired, package.IsRestartUIRequired);
                    }
                    else
                    {
                        StartElevatedUpdateProcess();
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, context, "Error in StartUpdateThread: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
                }
            }).Start();
        }

        private bool CheckDownloadsReady(PackageState packageState, IDataRow historyRow)
        {
            bool ready = true;
            Package package = packageState.Package;
            EnsurePackageItemFolders(package);
            foreach (PackageItem item in package.Items)
            {
                FileInfo file = item.TargetFileInfo;
                if (file.Exists && file.Length == item.FileSize)
                {
                    if (CheckFile(file, item.FileVersion, item.MD5Hash, GetDefaultAssemblyPublicKey()))
                    {
                        continue;
                    }

                    historyRow[StatusFieldIndex] = UpdateStatus.Error.ToString();
                    string err = Convert.ToString(historyRow[ErrorMessageFieldIndex]);
                    historyRow[ErrorMessageFieldIndex] = err + (string.IsNullOrEmpty(err) ? "" : ", ") + string.Format(InvalidFile, file.Name);
                    updateHistory.Update(historyRow);
                    dataSource.Flush();
                }

                downloader.Download(item.DownloadPath, file, item.FileSize, item.MD5Hash, packageState.OnDownloadStatus);
                ready = false;
            }

            return ready;
        }

        private byte[] GetDefaultAssemblyPublicKey()
        {
            return GetType().Assembly.GetName().GetPublicKey();
        }

        private IDataRow CreateOrUpdateHistoryRow(Package package)
        {
            string packageId = package.Id;
            IDataRow historyRow = updateHistory.Find(packageId);
            if (historyRow == null)
            {
                historyRow = updateHistory.Create(packageId);
                historyRow[PackageIdFieldIndex] = packageId;
                historyRow[StatusFieldIndex] = UpdateStatus.New.ToString();
            }
            else
            {
                string status = Convert.ToString(historyRow[StatusFieldIndex]);
                if (status == UpdateStatus.Success.ToString() || status == UpdateStatus.Ready.ToString())
                {
                    return historyRow;
                }
            }
            historyRow[PackageFieldIndex] = SerializePackage(package);
            updateHistory.Update(historyRow);
            dataSource.Flush();
            return historyRow;
        }

        private void StartPortableUpdateProcess(string tempFile, bool isRestartRequired, bool isRestartUIRequired)
        {
            try
            {
                string uiExecutablePathLocal = isRestartUIRequired ? uiExecutablePath : "";
                string executablePathLocal = isRestartRequired ? executablePath : "";
                ProcessUtils.StartProcessNoWindow(updateServicePath, string.Format("Portable \"{0}\" \"{1}\" \"{2}\"", tempFile, uiExecutablePathLocal, executablePathLocal));
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in StartPortableUpdateProcess: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private void StartElevatedUpdateProcess()
        {
            try
            {
                ProcessUtils.StartProcessNoWindow(updateServicePath, "Elevated");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in StartElevatedUpdateProcess: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private bool CheckFile(FileInfo fileInfo, Version version, byte[] md5Hash, byte[] assemblyPublicKey)
        {
            //FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
            //if (versionInfo == null || versionInfo.FileVersion == null || new Version(versionInfo.FileVersion) != version)
            //{
            //    return false;
            //}

            if (md5Hash != null && md5Hash.Length > 0)
            {
                if (!VerifyMd5Hash(fileInfo, md5Hash))
                {
                    return false;
                }
            }

            return true;
        }

        private bool VerifyMd5Hash(FileInfo fi, byte[] md5hash)
        {
            MD5 md5Hasher = MD5.Create();
            using (Stream stream = fi.OpenRead())
            {
                byte[] hash = md5Hasher.ComputeHash(stream);

                if (hash == null || hash.Length != md5hash.Length)
                {
                    return false;
                }

                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != md5hash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void OnTimer(object state)
        {
            lock (timerLock)
            {
                CheckForUpdates();
            }
        }

        private void OnReady(IPackage package)
        {
            if (Ready != null)
            {
                Ready(package);
            }
        }

        #region IAutoUpdateService Members

        public void CheckForUpdates()
        {
            try
            {
                string ticket = customerService.CustomerTicket;
                service.CookieContainer.Add(CreateCustomerCookie(ticket));

                if (string.IsNullOrEmpty(ticket))
                {
                    logger.Log(LogLevel.Error, context, "Customer ticket has not been created before use of auto-update service.");
                    return;
                }

                Package package = service.GetUpdates(system, GetProductInfo());
                if (package == null)
                {
                    logger.Log(LogLevel.Info, context, "There are no updates.");
                    return;
                }

                if (string.IsNullOrEmpty(package.ProductVersionLabel))
                {
                    logger.Log(LogLevel.Info, context, "Package does not have product version label.");
                    return;
                }

                var productVersion = new Version(package.ProductVersionLabel);
                IProductInfo productInfo = (IProductInfo)context.GetService(typeof(IProductInfo));
                if (productVersion < productInfo.ProductVersion)
                {
                    logger.Log(LogLevel.Info, context, "Can't update version {0} to an old version {1}.", productInfo.ProductVersion.ToString(), package.ProductVersionLabel);
                    return;
                }

                PackageState packageState = new PackageState(this, package);
                CheckPackageState(packageState);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in CheckForUpdates: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);

                lock (timerLock)
                {
                    if (timer != null)
                    {
                        timer.Change(errorInterval, checkInterval);
                    }
                }
            }
        }

        public IList<IUpdateHistory> History
        {
            get
            {
                List<IUpdateHistory> history = new List<IUpdateHistory>();
                foreach (IDataRow row in updateHistory)
                {
                    UpdateHistoryItem item = new UpdateHistoryItem();
                    item.Status = (UpdateStatus)Enum.Parse(typeof(UpdateStatus), Convert.ToString(row[StatusFieldIndex]));
                    item.ErrorMessage = Convert.ToString(Convert.ToString(row[ErrorMessageFieldIndex]));
                    string installedOn = Convert.ToString(row[InstalledOnFieldIndex]);
                    if (!string.IsNullOrEmpty(installedOn))
                    {
                        item.InstalledOn = Convert.ToDateTime(installedOn);
                    }
                    item.Package = DeserializePackage(Convert.ToString(row[PackageFieldIndex]));
                    history.Add(item);
                }

                return history;
            }
        }

        public void Start()
        {
            CheckSuccessUpdate();

            lock (timerLock)
            {
                if (timer == null)
                {
                    int dueTime = checkOnStart ? startInterval : checkInterval;
                    timer = new Timer(OnTimer, null, dueTime, checkInterval);
                }
            }
        }

        public void Stop()
        {
            lock (timerLock)
            {
                if (timer != null)
                {
                    timer.Dispose();
                }
            }
        }

        public event UpdateStatusEventHandler Ready;

        #endregion

        private class PackageState
        {
            private readonly AutoUpdateService service;
            private readonly Package package;
            private readonly object syncObj;

            public PackageState(AutoUpdateService service, Package package)
            {
                this.service = service;
                this.package = package;
                syncObj = new object();
            }

            public Package Package
            {
                get
                {
                    return package;
                }
            }

            public object SyncObj
            {
                get
                {
                    return syncObj;
                }
            }

            public void OnDownloadStatus(IDownloadRequestStatus status)
            {
                if (status.Status == DownloadStatus.Finished)
                {
                    service.CheckPackageState(this);
                }
            }
        }

        private class UpdateHistoryItem : IUpdateHistory
        {
            public IPackage Package
            {
                get;
                set;
            }

            public DateTime InstalledOn
            {
                get;
                set;
            }

            public UpdateStatus Status
            {
                get;
                set;
            }

            public string ErrorMessage
            {
                get;
                set;
            }
        }
    }
}
