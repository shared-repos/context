using System;
using System.Threading;
using Context.Interfaces.Logging;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ServiceModule : IModule, IExternalModule
    {
        private readonly string serviceName;
        private readonly RunOptions runOptions;
        private ILogService logger;
        private bool keep;

        public ServiceModule(string serviceName, RunOptions runOptions)
        {
            this.serviceName = serviceName;
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

            new Thread(delegate()
            {
                try
                {
                    ServiceUtils.StartService(serviceName, TimeSpan.FromSeconds(30));
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Error start up service '{0}': {1}:{2}, StackTrace: {3}", serviceName, ex.GetType().Name, ex.Message, ex.StackTrace);
                }
            }).Start();
        }

        public void Stop()
        {
            keep = false;
            try
            {
                ServiceUtils.StopService(serviceName, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Error stop service '{0}': {1}:{2}, StackTrace: {3}", serviceName, ex.GetType().Name, ex.Message, ex.StackTrace);
            }
        }

        public void Keep()
        {
            keep = true;
        }

        #endregion
    }
}
