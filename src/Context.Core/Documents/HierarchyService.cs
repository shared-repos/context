using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.UI.Documents;
using Context.Interfaces.UI;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class HierarchyService : IHierarchyService
    {
        private readonly IServiceManager manager;
        private readonly Dictionary<Guid, IHierarchyFactory> registeredFactories;
        private IHierarchy currentHierarchy;

        public HierarchyService(IServiceManager manager)
        {
            this.manager = manager;
            this.registeredFactories = new Dictionary<Guid, IHierarchyFactory>();
        }

        #region IHierarchyService Members

        public IHierarchy CreateHierarchy(string moniker, Guid factoryId, bool createNew, Guid windowId)
        {
            IHierarchyFactory factory;
            if (!registeredFactories.TryGetValue(factoryId, out factory))
            {
                IModuleManager modules = (IModuleManager)manager.GetService(typeof(IModuleManager));
                if (modules == null)
                {
                    return null;
                }

                IModule module = modules.LoadModule(factoryId);
                if (module == null)
                {
                    return null;
                }

                factory = module as IHierarchyFactory;
                if (factory == null)
                {
                    factory = (IHierarchyFactory)module.GetService(typeof(IHierarchyFactory));
                }
                if (factory != null)
                {
                    registeredFactories[factoryId] = factory;
                }
            }

            if (factory != null)
            {
                IHierarchy hierarchy = factory.CreateHierarchy(moniker, createNew);

                IWindowService windows = (IWindowService)manager.GetService(typeof(IWindowService));
                if (windows != null && hierarchy != null)
                {
                    IToolWindow toolWindow = windows.FindToolWindow(windowId);
                    if (toolWindow != null)
                    {
                        IHierarchyView view = toolWindow.Content as IHierarchyView;
                        if (view != null)
                        {
                            view.AddHierarchy(hierarchy);
                        }
                    }
                }

                return hierarchy;
            }
            return null;
        }

        public void RegisterHierarchyFactory(Guid factoryId, IHierarchyFactory hierarchyFactory)
        {
            registeredFactories[factoryId] = hierarchyFactory;
        }

        public IHierarchy CurrentHierarchy
        {
            get
            {
                return currentHierarchy;
            }
            set
            {
                if (currentHierarchy != value)
                {
                    currentHierarchy = value;
                    OnCurrentHierarchyChanged(value);
                }
            }
        }

        public event HierarchyEventHandler CurrentHierarchyChanged;

        #endregion

        private void OnCurrentHierarchyChanged(IHierarchy hierarchy)
        {
            if (CurrentHierarchyChanged != null)
            {
                CurrentHierarchyChanged(hierarchy);
            }
        }
    }
}
