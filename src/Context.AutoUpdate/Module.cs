using System;
using System.Runtime.InteropServices;
using Context.Interfaces.Services;
using Context.Interfaces.AutoUpdate;

namespace Context.AutoUpdate
{
    [Guid("28E6B5FB-B864-46c0-A2B8-250D579389B9")]
    public class Module : IModule
    {
        private const string EnabledSettings = "Enabled";

        private static Guid AutoUpdateServiceGuid = typeof(IAutoUpdateService).GUID;

        private IAutoUpdateService autoUpdateService;

        #region IModule Members

        public void Attach(IServiceManager manager)
        {
            IContextService contextService = (IContextService)manager.GetService(typeof(IContextService));

            using (contextService.CreateScope("AutoUpdate"))
            {
                autoUpdateService = new AutoUpdateService(contextService.Current);
                object obj = contextService.Current[EnabledSettings];
                bool enabled = obj == null ? true : Convert.ToBoolean(obj);
                if (enabled)
                {
                    autoUpdateService.Start();
                }
            }
        }

        public void Detach()
        {
            autoUpdateService.Stop();
        }

        public object GetService(Guid serviceId)
        {
            if (serviceId == AutoUpdateServiceGuid)
            {
                return autoUpdateService;
            }

            return null;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (autoUpdateService != null && serviceType == autoUpdateService.GetType())
            {
                return autoUpdateService;
            }

            return null;
        }

        #endregion
    }
}
