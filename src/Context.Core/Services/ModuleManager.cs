using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ModuleManager : IModuleManager
    {
        private readonly IServiceManager manager;
        private readonly Dictionary<Guid, ModuleInfo> modules;
        private readonly Dictionary<Guid, ModuleInfo> services;
        private readonly object loadLockObj;

        public ModuleManager(IServiceManager manager)
        {
            this.manager = manager;
            this.modules = new Dictionary<Guid, ModuleInfo>();
            this.services = new Dictionary<Guid, ModuleInfo>();
            this.loadLockObj = new object();
        }

        private void EnsureModuleInfos()
        {
            if (modules.Count == 0)
            {
                LoadModuleInfos();
            }
        }

        private void LoadModuleInfos()
        {
            DataSet config = ConfigurationManager.GetSection(ConfigurationSectionHandler.SectionName) as DataSet;
            if (config == null)
            {
                return;
            }

            DataTable modulesTable = config.Tables[ConfigurationSectionHandler.ModulesConfigurationSettings];
            if (modulesTable == null)
            {
                return;
            }

            DataColumn idCol = modulesTable.Columns[ConfigurationSectionHandler.IdColumn];
            DataColumn nameCol = modulesTable.Columns[ConfigurationSectionHandler.NameColumn];
            DataColumn descriptionCol = modulesTable.Columns[ConfigurationSectionHandler.DescriptionColumn];
            DataColumn assemblyCol = modulesTable.Columns[ConfigurationSectionHandler.AssemblyColumn];
            DataColumn classCol = modulesTable.Columns[ConfigurationSectionHandler.ClassColumn];
            DataColumn pathCol = modulesTable.Columns[ConfigurationSectionHandler.PathColumn];
            DataColumn serviceNameCol = modulesTable.Columns[ConfigurationSectionHandler.ServiceNameColumn];

            foreach (DataRow row in modulesTable.Rows)
            {
                Guid id = new Guid(Convert.ToString(row[idCol]));
                Dictionary<string, object> props = new Dictionary<string, object>();
                foreach (DataColumn col in modulesTable.Columns)
                {
                    string val = Convert.ToString(row[col]);
                    if (string.IsNullOrEmpty(val))
                    {
                        continue;
                    }

                    props.Add(col.ColumnName, val);
                }

                string moduleName = Convert.ToString(row[nameCol]);
                string description = Convert.ToString(row[descriptionCol]);
                string assemblyName = Convert.ToString(row[assemblyCol]);
                string className = Convert.ToString(row[classCol]);
                string path = Convert.ToString(row[pathCol]);
                string serviceName = Convert.ToString(row[serviceNameCol]);
                modules.Add(id, new ModuleInfo(id, moduleName, description, assemblyName, className, path, serviceName, props));
            }

            DataTable servicesTable = config.Tables[ConfigurationSectionHandler.ServicesConfigurationSettings];
            if (servicesTable == null)
            {
                return;
            }

            idCol = servicesTable.Columns[ConfigurationSectionHandler.IdColumn];
            nameCol = servicesTable.Columns[ConfigurationSectionHandler.NameColumn];
            DataColumn moduleCol = servicesTable.Columns[ConfigurationSectionHandler.ModuleColumn];

            foreach (DataRow row in servicesTable.Rows)
            {
                Guid id = new Guid(Convert.ToString(row[idCol]));
                string name = Convert.ToString(row[nameCol]);
                Guid moduleId = new Guid(Convert.ToString(row[moduleCol]));
                AddService(id, name, moduleId);
            }
        }

        private void AddService(Guid serviceId, string name, Guid moduleId)
        {
            ModuleInfo module;
            if (modules.TryGetValue(moduleId, out module))
            {
                module.AddService(serviceId, name);
                services.Add(serviceId, module);
            }
        }

        internal IModule GetServiceModule(Guid serviceId)
        {
            ModuleInfo module;
            if (services.TryGetValue(serviceId, out module))
            {
                if (!module.IsLoaded)
                {
                    lock (loadLockObj)
                    {
                        LoadModuleCore(module);
                    }
                }

                return module.Module;
            }

            return null;
        }

        #region IModuleManager Members

        public IModuleInfo[] GetModuleList()
        {
            EnsureModuleInfos();
            IModuleInfo[] arr = new IModuleInfo[modules.Count];
            int i = 0;
            foreach (ModuleInfo module in modules.Values)
            {
                arr[i] = module;
                i++;
            }
            return arr;
        }

        public Guid[] ListModuleServices(Guid moduleId)
        {
            ModuleInfo module;
            if (modules.TryGetValue(moduleId, out module))
            {
                return module.ListServices();
            }
            return null;
        }

        public bool IsModuleRegistered(Guid moduleId)
        {
            return modules.ContainsKey(moduleId);
        }

        public bool IsModuleLoaded(Guid moduleId)
        {
            ModuleInfo module;
            if (modules.TryGetValue(moduleId, out module))
            {
                return module.IsLoaded;
            }
            return false;
        }

        public IModule LoadModule(Guid moduleId)
        {
            ModuleInfo module;
            if (modules.TryGetValue(moduleId, out module))
            {
                if (!module.IsLoaded)
                {
                    lock (loadLockObj)
                    {
                        LoadModuleCore(module);
                    }
                }

                return module.Module;
            }
            return null;
        }

        private void LoadModuleCore(ModuleInfo moduleInfo)
        {
            if (moduleInfo.IsLoaded)
            {
                return;
            }

            if (!string.IsNullOrEmpty(moduleInfo.Path))
            {
                string path = moduleInfo.Path;
                string arguments = Convert.ToString(moduleInfo[ConfigurationSectionHandler.ArgumentsColumn]);
                RunOptions runOptions = ParseRunOptions(Convert.ToString(moduleInfo[ConfigurationSectionHandler.RunOptionsColumn]));
                IModule module = new ExternalModule(path, arguments, runOptions);
                moduleInfo.Module = module;
                AttachModule(module);
                return;
            }

            if (!string.IsNullOrEmpty(moduleInfo.ServiceName))
            {
                string serviceName = moduleInfo.ServiceName;
                RunOptions runOptions = ParseRunOptions(Convert.ToString(moduleInfo[ConfigurationSectionHandler.RunOptionsColumn]));
                IModule module = new ServiceModule(serviceName, runOptions);
                moduleInfo.Module = module;
                AttachModule(module);
                return;
            }

            if (!string.IsNullOrEmpty(moduleInfo.AssemblyName) && !string.IsNullOrEmpty(moduleInfo.ClassName))
            {
                try
                {
                    Type moduleType = null;
                    try
                    {
                        string fullTypeName = Assembly.CreateQualifiedName(moduleInfo.AssemblyName, moduleInfo.ClassName);
                        moduleType = Type.GetType(fullTypeName);
                    }
                    catch
                    {
                        try
                        {
                            string fullTypeName = Assembly.CreateQualifiedName(GetAssemblyName(moduleInfo.AssemblyName), moduleInfo.ClassName);
                            moduleType = Type.GetType(fullTypeName);
                        }
                        catch
                        {
                            string fullTypeName = Assembly.CreateQualifiedName(GetAssemblyName2(moduleInfo.AssemblyName), moduleInfo.ClassName);
                            moduleType = Type.GetType(fullTypeName);
                        }
                    }

                    if (moduleType != null)
                    {
                        IModule module = (IModule)Activator.CreateInstance(moduleType);
                        moduleInfo.Module = module;
                        if (module != null)
                        {
                            AttachModule(module);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private RunOptions ParseRunOptions(string str)
        {
            RunOptions options = default(RunOptions);
            if (!string.IsNullOrEmpty(str))
            {
                string[] arr = str.Split('|');
                foreach (string opt in arr)
                {
                    options |= (RunOptions)Enum.Parse(typeof(RunOptions), opt, true);
                }
            }

            return options;
        }

        private void AttachModule(IModule module)
        {
            ConfigurationManagerService.Instance.ConfigureModule(module);
            module.Attach(manager);
        }

        private string GetAssemblyName(string name)
        {
            AssemblyName assemblyName = new AssemblyName(name);
            AssemblyName thisAssembly = GetType().Assembly.GetName();

            if (assemblyName.Version == null)
            {
                assemblyName.Version = thisAssembly.Version;
            }

            if (assemblyName.GetPublicKeyToken() == null)
            {
                assemblyName.SetPublicKeyToken(thisAssembly.GetPublicKeyToken());
            }

            if (assemblyName.CultureInfo == null)
            {
                assemblyName.CultureInfo = thisAssembly.CultureInfo;
            }

            return assemblyName.ToString();
        }

        private string GetAssemblyName2(string name)
        {
            AssemblyName assemblyName = new AssemblyName(name);
            AssemblyName rootAssembly = Assembly.GetEntryAssembly().GetName();

            if (assemblyName.Version == null)
            {
                assemblyName.Version = rootAssembly.Version;
            }

            if (assemblyName.GetPublicKeyToken() == null)
            {
                assemblyName.SetPublicKeyToken(rootAssembly.GetPublicKeyToken());
            }

            if (assemblyName.CultureInfo == null)
            {
                assemblyName.CultureInfo = rootAssembly.CultureInfo;
            }

            return assemblyName.ToString();
        }

        public void UnloadModule(Guid moduleId)
        {
            ModuleInfo module;
            if (modules.TryGetValue(moduleId, out module))
            {
                if (module.IsLoaded)
                {
                    module.Module.Detach();
                    module.Module = null;
                }
            }
        }

        internal void OnClosing(ref bool cancel)
        {
            if (Closing != null)
            {
                Closing(ref cancel);
            }
        }

        public event ClosingEventHandler Closing;

        #endregion
    }
}
