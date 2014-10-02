using System;
using System.Runtime.InteropServices;
using Context.Interfaces.Services;
using Context.Interfaces.Data;

namespace Context.Data
{
    [Guid("71FC5EAC-6C36-465c-AF04-97347E4D0A90")]
    public class Module : IModule
    {
        private static Guid IDataServiceGuid = typeof(IDataService).GUID;

        private IDataService dataService;

        #region IModule Members

        public void Attach(IServiceManager manager)
        {
            IContextService contextService = (IContextService)manager.GetService(typeof(IContextService));
            using (contextService.CreateScope("DataSources"))
            {
                dataService = new DataService(contextService);
            }
        }

        public void Detach()
        {
        }

        public object GetService(Guid serviceId)
        {
            if (serviceId == IDataServiceGuid)
            {
                return dataService;
            }

            return null;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataService))
            {
                return dataService;
            }

            return null;
        }

        #endregion
    }
}

