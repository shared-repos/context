using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.UI;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ModuleWindowFactory : IWindowFactory
    {
        private IModuleManager modules;
        private Guid moduleId;

        public ModuleWindowFactory(IModuleManager modules, Guid moduleId)
        {
            this.modules = modules;
            this.moduleId = moduleId;
        }

        #region IWindowFactory Members

        public IToolWindowContent CreateToolWindowControl(Guid controlId)
        {
            IModule module = modules.LoadModule(moduleId);
            if (module == null)
            {
                return null;
            }

            IWindowFactory innerFactory = module as IWindowFactory;
            if (innerFactory == null)
            {
                innerFactory = (IWindowFactory)module.GetService(typeof(IWindowFactory));
            }
            if (innerFactory == null)
            {
                return null;
            }

            return innerFactory.CreateToolWindowControl(controlId);
        }

        #endregion
    }
}
