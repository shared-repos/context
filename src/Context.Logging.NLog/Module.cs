using System;
using System.Runtime.InteropServices;
using Context.Communication.Configuration;
using Context.Interfaces.Common;
using Context.Interfaces.Configuration;
using Context.Interfaces.Logging;
using Context.Interfaces.Services;

namespace Context.Logging.NLog
{
    [Guid("2F3E3DDF-D31A-4dd1-A437-CD2DF0FAC706")]
    public class Module : IModule, IConfigurationProvider
    {
        private static Guid LogServiceGuid = typeof(ILogService).GUID;

        private ILogService logService;

        #region IModule Members

        public void Attach(IServiceManager manager)
        {
            IContextService context = (IContextService)manager.GetService(typeof(IContextService));
            logService = new LogService(context);
            IProductInfo product = (IProductInfo)manager.GetService(typeof(IProductInfo));
            using (context.CreateScope(LogService.GlobalLogName))
            {
                logService.Log(LogLevel.Info, "{0} Version {1}. Product ID: ({2})", product.ApplicationName, product.ProductVersion, product.ProductId);
            }
        }

        public void Detach()
        {
            logService.Flush();
        }

        public object GetService(Guid serviceId)
        {
            if (serviceId == LogServiceGuid)
            {
                return logService;
            }

            return null;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (logService != null && serviceType == logService.GetType())
            {
                return logService;
            }

            return null;
        }

        #endregion

        #region IConfigurationProvider Members

        public IConfigurationSection[] GetConfigurationSections()
        {
            return new IConfigurationSection[] { new NLogSection() };
        }

        public IConfigurationPage[] GetConfigurationPages()
        {
            return new IConfigurationPage[0];
        }

        #endregion
    }
}
