using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using Context.Interfaces.Common;
using Context.Core;

namespace Context.Service
{
    [RunInstaller(true)]
    public class ProductServiceInstaller : Installer
    {
        public ProductServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;

            IServiceProvider provider = new Startup();
            IProductInfo product = (IProductInfo)provider.GetService(typeof(IProductInfo));

            serviceInstaller.DisplayName = product.ApplicationName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.Description = product.Description;
            serviceInstaller.ServiceName = product.ApplicationName;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
