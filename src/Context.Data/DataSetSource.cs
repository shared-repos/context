using System;
using System.Data;
using System.IO;
using System.Threading;
using Context.Interfaces.Data;
using Context.Interfaces.Logging;
using Context.Interfaces.Schema;
using Context.Interfaces.Services;

namespace Context.Data
{
    public class DataSetSource : IDataSource
    {
        private const string BackupExtension = "~";
        private const string TempExtension = "tmp";
        private const int BackupCount = 3;
        private const int LockTimeout = 20000;

        private readonly IContext context;
        private readonly ISchemaService schemaService;
        private readonly ILogService logger;
        private readonly DataSet dataSet;
        private readonly string fileName;
        private readonly string dataSetName;
        private readonly Mutex mutex;
        private readonly FileMonitor monitor;
        private readonly object syncObj;

        public DataSetSource(IContext context, ISchemaService schemaService, ILogService logger, string path, string dataSetName, string lockName, bool trackChanges)
        {
            this.context = context;
            this.schemaService = schemaService;
            this.logger = logger;
            this.dataSet = new DataSet(dataSetName);
            this.dataSetName = dataSetName;
            this.syncObj = new object();

            string schema = Convert.ToString(schemaService.GetSchema(dataSetName));
            if (!string.IsNullOrEmpty(schema))
            {
                dataSet.ReadXmlSchema(new StringReader(schema));
                dataSet.DataSetName = dataSetName;
            }

            this.fileName = FileUtils.EnsureDirectory(FileUtils.GetAbsolutePath(path));

            if (!string.IsNullOrEmpty(lockName))
            {
                try
                {
                    bool created;
                    mutex = SystemUtils.CreateMutex(lockName, false, out created);
                    logger.Log(LogLevel.Info, "{0} mutex {1}", created ? "Created" : "Open", lockName);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, context, "Can't create mutex '{0}', Exception: {1}:{2}, StackTrace: {3}", lockName, ex.GetType().Name, ex.Message, ex.StackTrace);
                }
            }

            Refresh();

            if (trackChanges)
            {
                monitor = new FileMonitor(fileName, OnChanged);
            }
        }

        public object SyncRoot
        {
            get
            {
                return syncObj;
            }
        }

        public bool IsDirty { get; set; }

        public event DataSourceChangedHandler Changed;

        internal void LogError(Exception ex)
        {
            logger.Log(LogLevel.Error, context, "Exception: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
        }

        #region IDataSource Members

        private void OnChanged()
        {
            try
            {
                if (Changed != null)
                {
                    Changed();
                }
            }
            catch
            {
            }
        }

        public IDataTable GetTable(string tableName)
        {
            if (dataSet.Tables.Contains(tableName))
            {
                return new DataSetTable(dataSet.Tables[tableName], this);
            }

            DataTable table = dataSet.Tables.Add(tableName);

            string schema = Convert.ToString(schemaService.GetSchema(dataSetName + "." + tableName));
            if (string.IsNullOrEmpty(schema))
            {
                string anyTableName = dataSetName + ".*";
                schema = Convert.ToString(schemaService.GetSchema(anyTableName));
            }

            if (string.IsNullOrEmpty(schema))
            {
                schema = Convert.ToString(schemaService.GetSchema(tableName));
            }

            if (!string.IsNullOrEmpty(schema))
            {
                table.TableName = "Table";
                table.ReadXmlSchema(new StringReader(schema));
                table.TableName = tableName;
            }

            return new DataSetTable(table, this);
        }

        public IDataTable Query(string queryText)
        {
            throw new NotImplementedException();
        }

        public void Execute(string command)
        {
            throw new NotImplementedException();
        }

        private void SetEnableRaisingEvents(bool enable)
        {
            if (monitor != null)
            {
                monitor.Enabled = enable;
            }
        }

        private void Lock()
        {
            try
            {
                if (mutex != null)
                {
                    mutex.WaitOne(LockTimeout, false);
                }

                Monitor.TryEnter(syncObj, LockTimeout);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in DataSetSource.Lock: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        private void UnLock()
        {
            try
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                }

                Monitor.Exit(syncObj);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error in DataSetSource.UnLock: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        public void Refresh()
        {
            if (File.Exists(fileName) || HasBackupFiles())
            {
                Lock();
                try
                {
                    dataSet.Clear();
                    dataSet.ReadXml(fileName);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, context, "Error reading file '{0}' in DataSetSource: {1}:{2}, StackTrace: {3}", fileName, ex.GetType().Name, ex.Message, ex.StackTrace);
                    for (int i = 0; i < BackupCount; i++)
                    {
                        string ext = i == 0 ? BackupExtension : BackupExtension + i.ToString();
                        string bakFile = Path.ChangeExtension(fileName, ext);
                        try
                        {
                            if (File.Exists(bakFile))
                            {
                                dataSet.Clear();
                                dataSet.ReadXml(bakFile);
                                break;
                            }
                        }
                        catch (Exception ex2)
                        {
                            logger.Log(LogLevel.Error, context, "Error reading backup file '{0}' in DataSetSource: {1}:{2}, StackTrace: {3}", bakFile, ex2.GetType().Name, ex2.Message, ex2.StackTrace);
                        }
                    }
                }
                finally
                {
                    UnLock();
                }
            }

            try
            {
                dataSet.EnforceConstraints = true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, context, "Error set EnforceConstraints in DataSetSource: {0}:{1}, StackTrace: {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }

            IsDirty = true;
        }

        private bool HasBackupFiles()
        {
            for (int i = 0; i < BackupCount; i++)
            {
                string ext = i == 0 ? BackupExtension : BackupExtension + i.ToString();
                string bakFile = Path.ChangeExtension(fileName, ext);
                if (File.Exists(bakFile))
                {
                    return true;
                }
            }

            return false;
        }

        public void Flush()
        {
            if (IsDirty)
            {
                Lock();
                SetEnableRaisingEvents(false);

                try
                {
                    CreateBackupFiles();
                    string tempFile = Path.ChangeExtension(fileName, TempExtension);
                    dataSet.WriteXml(tempFile, XmlWriteMode.WriteSchema);
                    try
                    {
                        File.Move(tempFile, fileName);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, context, "Error moving file '{0}' in Flush: {1}:{2}, StackTrace: {3}", fileName, ex.GetType().Name, ex.Message, ex.StackTrace);
                        File.Copy(tempFile, fileName, true);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, context, "Error creating file '{0}' in Flush: {1}:{2}, StackTrace: {3}", fileName, ex.GetType().Name, ex.Message, ex.StackTrace);
                }
                finally
                {
                    SetEnableRaisingEvents(true);
                    UnLock();
                }
            }

            IsDirty = false;
        }

        private void CreateBackupFiles()
        {
            if (File.Exists(fileName))
            {
                for (int i = BackupCount - 1; i >= 0; i--)
                {
                    string ext = i == 0 ? BackupExtension : BackupExtension + i.ToString();
                    string ext1 = i == 1 ? BackupExtension : BackupExtension + (i - 1).ToString();
                    string fromName = i == 0 ? fileName : Path.ChangeExtension(fileName, ext1);
                    string bakFile = Path.ChangeExtension(fileName, ext);
                    try
                    {
                        if (File.Exists(bakFile))
                        {
                            File.Delete(bakFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, context, "Error deleting previous backup file '{0}': {1}:{2}, StackTrace: {3}", bakFile, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }

                    try
                    {
                        if (File.Exists(fromName))
                        {
                            File.Move(fromName, bakFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(LogLevel.Error, context, "Error creating backup file '{0}': {1}:{2}, StackTrace: {3}", bakFile, ex.GetType().Name, ex.Message, ex.StackTrace);
                    }
                }
            }
        }

        #endregion

    }
}
