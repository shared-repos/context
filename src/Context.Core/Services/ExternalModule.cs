using System;
using System.Diagnostics;
using System.IO;
using Context.Interfaces.Logging;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ExternalModule : IModule, IExternalModule
    {
        private readonly string path;
        private readonly string arguments;
        private readonly string fileName;
        private readonly RunOptions runOptions;
        private ILogService logger;
        private bool started;
        private bool keep;

        public ExternalModule(string path, string arguments, RunOptions runOptions)
        {
            this.path = FileUtils.GetAbsolutePath(path);
            this.arguments = arguments;
            this.fileName = Path.GetFileName(path);
            this.runOptions = runOptions;
        }

        #region IModule Members

        public void Attach(IServiceManager manager)
        {
            logger = (ILogService)manager.GetService(typeof(ILogService));

            if ((runOptions & RunOptions.RunOnAttach) != 0)
            {
                Start();
            }
        }

        public void Detach()
        {
            if ((runOptions & RunOptions.RunOnDetach) != 0)
            {
                Start();
            }

            if ((runOptions & RunOptions.StopOnDetach) != 0)
            {
                if (!keep)
                {
                    Stop();
                }
            }
        }

        public object GetService(Guid serviceId)
        {
            return null;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IExternalModule))
            {
                return this;
            }

            return null;
        }

        #endregion

        #region IExternalModule Members

        public void Start()
        {
            keep = false;
            if (started)
            {
                return;
            }

            try
            {
                ProcessUtils.ShellExecute(path, null, arguments);
                started = true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Error start up process '{0}': {1}:{2}, StackTrace: {3}", fileName, ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        public void Stop()
        {
            keep = false;
            try
            {
                ProcessUtils.KillProcesses(path);
                started = false;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Error stop process '{0}': {1}:{2}, StackTrace: {3}", fileName, ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        public void Keep()
        {
            keep = true;
        }

        #endregion
    }
}
