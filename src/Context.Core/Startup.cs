using System;
using System.Globalization;
using Context.Interfaces.Common;
using Context.Interfaces.Configuration;
using Context.Interfaces.Schema;
using Context.Interfaces.Services;
using Context.Interfaces.UI;
using Context.Interfaces.UI.CommandBars;
using Context.Core.Configuration;
using System.ComponentModel.Design;
using System.Text;

namespace Context.Core
{
    public class Startup : IStartupObject
    {
        private const string ProductSection = "Product";

        private const string PreLoadOption = "PreLoad";
        private const string SetupUIOption = "SetupUI";

        private readonly object syncLock;
        private readonly ModuleManager modules;
        private readonly IProductInfo productInfo;
        private readonly ISystemInfo systemInfo;
        private ICommandService commandService;
        private IResourceManager resourceManager;
        private IPersistentFileManager fileManager;
        private IConfigurationManager configuration;
        private IContextService contextService;
        private ICustomerService customerService;
        private ISchemaService schemaService;
        private IServiceContainer container;

        public Startup()
        {
            this.syncLock = new object();
            this.modules = new ModuleManager(this);
            this.systemInfo = RetrieveSystemInfo();
            this.productInfo = ReadProductInfo();
            this.container = new ServiceContainer();
        }

        public Startup(IProductInfo productInfo)
        {
            this.syncLock = new object();
            this.modules = new ModuleManager(this);
            this.productInfo = productInfo;
            this.systemInfo = RetrieveSystemInfo();
            this.container = new ServiceContainer();
        }

        private ICommandService CommandService
        {
            get
            {
                if (commandService == null)
                {
                    lock (syncLock)
                    {
                        if (commandService == null)
                        {
                            commandService = (ICommandService)LoadService(typeof(ICommandService));
                            if (commandService == null)
                            {
                                commandService = new CommandService(this);
                            }
                        }
                    }
                }
                return commandService;
            }
        }

        private IResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    lock (syncLock)
                    {
                        if (resourceManager == null)
                        {
                            resourceManager = (IResourceManager)LoadService(typeof(IResourceManager));
                            if (resourceManager == null)
                            {
                                resourceManager = new ResourceManager(this);
                            }
                        }
                    }
                }

                return resourceManager;
            }
        }

        private IPersistentFileManager FileManager
        {
            get
            {
                if (fileManager == null)
                {
                    lock (syncLock)
                    {
                        if (fileManager == null)
                        {
                            fileManager = (IPersistentFileManager)LoadService(typeof(IPersistentFileManager));
                            if (fileManager == null)
                            {
                                fileManager = new PersistentFileManager(this);
                            }
                        }
                    }
                }

                return fileManager;
            }
        }

        private IConfigurationManager ConfigurationManager
        {
            get
            {
                if (configuration == null)
                {
                    lock (syncLock)
                    {
                        if (configuration == null)
                        {
                            configuration = (IConfigurationManager)LoadService(typeof(IConfigurationManager));
                            if (configuration == null)
                            {
                                configuration = ConfigurationManagerService.Instance;
                            }
                        }
                    }
                }

                return configuration;
            }
        }

        private IContextService ContextService
        {
            get
            {
                if (contextService == null)
                {
                    lock (syncLock)
                    {
                        if (contextService == null)
                        {
                            contextService = (IContextService)LoadService(typeof(IContextService));
                            if (contextService == null)
                            {
                                contextService = new ContextService(this);
                            }
                        }
                    }
                }

                return contextService;
            }
        }

        private ICustomerService CustomerService
        {
            get
            {
                if (customerService == null)
                {
                    lock (syncLock)
                    {
                        if (customerService == null)
                        {
                            customerService = (ICustomerService)LoadService(typeof(ICustomerService));
                            if (customerService == null)
                            {
                                using (ContextService.CreateScope(Core.CustomerService.CustomerServiceScope))
                                {
                                    customerService = new CustomerService(ContextService);
                                }
                            }
                        }
                    }
                }

                return customerService;
            }
        }

        private ISchemaService SchemaService
        {
            get
            {
                if (schemaService == null)
                {
                    lock (syncLock)
                    {
                        if (schemaService == null)
                        {
                            schemaService = (ISchemaService)LoadService(typeof(ISchemaService));
                            if (schemaService == null)
                            {
                                schemaService = new SchemaService();
                            }
                        }
                    }
                }

                return schemaService;
            }
        }

        private ISystemInfo RetrieveSystemInfo()
        {
            SystemInfo info = new SystemInfo();
            info.ComponentInfos = BuildComponentInfos();
            info.CultureName = CultureInfo.CurrentCulture.Name;
            info.System = GetCurrentSystem();
            info.SystemVersion = Environment.OSVersion.Version;
            return info;
        }

        private string BuildComponentInfos()
        {
            StringBuilder sb = new StringBuilder();
            var netVersion = Environment.Version;
            netVersion = new Version(netVersion.Major, netVersion.Minor, netVersion.Build);
            sb.Append(".NET Framework ");
            sb.Append(netVersion.ToString());
            return sb.ToString();
        }

        private SystemID GetCurrentSystem()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    return SystemID.Windows;
                case PlatformID.MacOSX:
                    return SystemID.OSX;
                case PlatformID.Unix:
                    return SystemID.Linux;
                default:
                    return SystemID.Unknown;
            }
        }

        #region IStartupObject Members

        void IStartupObject.Start()
        {
            foreach (IModuleInfo moduleInfo in modules.GetModuleList())
            {
                if (string.Compare(Convert.ToString(moduleInfo[PreLoadOption]), "true", true) == 0)
                {
                    PreLoadModule(moduleInfo);
                }

                // TODO: don't setup UI if cached
                if (string.Compare(Convert.ToString(moduleInfo[SetupUIOption]), "true", true) == 0)
                {
                    SetupModuleUI(moduleInfo);
                }
            }
        }

        bool IStartupObject.QueryClose()
        {
            bool cancel = false;
            modules.OnClosing(ref cancel);
            if (!cancel)
            {
                Dispose();
            }

            return !cancel;
        }

        private void PreLoadModule(IModuleInfo moduleInfo)
        {
            Guid id = moduleInfo.Id;
            CommandService.DefaultContextId = id;
            IModule module = modules.LoadModule(id);
            if (module == null)
            {
                return;
            }

            IPreLoadService service = module as IPreLoadService;
            if (service == null)
            {
                service = (IPreLoadService)module.GetService(typeof(IPreLoadService));
            }

            if (service != null)
            {
                service.PreLoad();
            }
        }

        private void SetupModuleUI(IModuleInfo moduleInfo)
        {
            Guid id = moduleInfo.Id;
            CommandService.DefaultContextId = id;
            IModule module = modules.LoadModule(id);
            if (module == null)
            {
                return;
            }

            ISetupUI setupUI = module as ISetupUI;
            if (setupUI == null)
            {
                setupUI = (ISetupUI)module.GetService(typeof(ISetupUI));
            }
            if (setupUI != null)
            {
                setupUI.SetupUI();
            }

            ICommandTarget commandTarget = module as ICommandTarget;
            if (commandTarget == null)
            {
                commandTarget = (ICommandTarget)module.GetService(typeof(ICommandTarget));
            }
            if (commandTarget != null)
            {
                CommandService.AttachCommandTarget(id, commandTarget);
            }
        }

        #endregion

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IProductInfo))
            {
                return productInfo;
            }

            if (serviceType == typeof(ISystemInfo))
            {
                return systemInfo;
            }

            if (serviceType == typeof(IModuleManager))
            {
                return modules;
            }

            if (serviceType == typeof(ICommandService))
            {
                return CommandService;
            }

            if (serviceType == typeof(IResourceManager))
            {
                return ResourceManager;
            }

            if (serviceType == typeof(IPersistentFileManager))
            {
                return FileManager;
            }

            if (serviceType == typeof(IConfigurationManager))
            {
                return ConfigurationManager;
            }

            if (serviceType == typeof(IContextService))
            {
                return ContextService;
            }

            if (serviceType == typeof(ICustomerService))
            {
                return CustomerService;
            }

            if (serviceType == typeof(ISchemaService))
            {
                return SchemaService;
            }

            return LoadService(serviceType);
        }

        private object LoadService(Guid serviceId)
        {
            IModule module = modules.GetServiceModule(serviceId);
            if (module != null)
            {
                return module.GetService(serviceId);
            }

            return null;
        }

        private object LoadService(Type type)
        {
            return LoadService(type.GUID);
        }

        private IProductInfo ReadProductInfo()
        {
            IProductInfo product = new ProductInfo(ConfigurationManager.GetSection(ProductSection));
            return product;
        }

        #endregion

        #region IServiceManager Members

        object IServiceManager.GetService(Guid serviceId)
        {
            return LoadService(serviceId);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IModuleInfo moduleInfo in modules.GetModuleList())
            {
                modules.UnloadModule(moduleInfo.Id);
            }
        }

        #endregion

        #region IServiceContainer Members

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            container.AddService(serviceType, callback, promote);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            container.AddService(serviceType, callback);
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            container.AddService(serviceType, serviceInstance, promote);
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            container.AddService(serviceType, serviceInstance);
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            container.RemoveService(serviceType, promote);
        }

        public void RemoveService(Type serviceType)
        {
            container.RemoveService(serviceType);
        }

        #endregion
    }
}
