using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.UI.CommandBars;
using Context.Interfaces.Services;

namespace Context.Core
{
    internal class ModuleCommandTarget : ICommandTarget
    {
        private IModuleManager modules;
        private Guid moduleId;

        public ModuleCommandTarget(IModuleManager modules, Guid moduleId)
        {
            this.modules = modules;
            this.moduleId = moduleId;
        }

        #region ICommandTarget Members

        public CommandStatus InvokeCommand(object sender, ICommand command, ICommandBarControl control)
        {
            IModule module = modules.LoadModule(moduleId);
            if (module == null)
            {
                return CommandStatus.Incompatible;
            }

            ICommandTarget innerTarget = module as ICommandTarget;
            if (innerTarget == null)
            {
                innerTarget = (ICommandTarget)module.GetService(typeof(ICommandTarget));
            }
            if (innerTarget == null)
            {
                return CommandStatus.Incompatible;
            }

            return innerTarget.InvokeCommand(sender, command, control);
        }

        public CommandStatus UpdateCommand(object sender, ICommand command)
        {
            if (!modules.IsModuleLoaded(moduleId))
            {
                return CommandStatus.Unhandled;
            }

            IModule module = modules.LoadModule(moduleId);
            if (module == null)
            {
                return CommandStatus.Incompatible;
            }

            ICommandTarget innerTarget = module as ICommandTarget;
            if (innerTarget == null)
            {
                innerTarget = (ICommandTarget)module.GetService(typeof(ICommandTarget));
            }
            if (innerTarget == null)
            {
                return CommandStatus.Incompatible;
            }

            return innerTarget.UpdateCommand(sender, command);
        }

        #endregion
    }
}
