using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.UI.CommandBars;

namespace Context.Core
{
    internal class CommandTarget : ICommandTarget
    {
        private ICommandTarget[] targets;

        public CommandTarget(ICommandTarget source, ICommandTarget target)
        {
            CommandTarget sourceTargets = source as CommandTarget;
            if (sourceTargets != null)
            {
                ICommandTarget[] sourceArray = sourceTargets.targets;
                int count = sourceArray.Length;
                targets = new ICommandTarget[count + 1];
                Array.Copy(sourceArray, targets, count);
            }
            else
            {
                targets = new ICommandTarget[] { source, target };
            }
        }

        #region ICommandTarget Members

        public CommandStatus InvokeCommand(object sender, ICommand command, ICommandBarControl control)
        {
            CommandStatus status = CommandStatus.Incompatible;
            foreach (ICommandTarget target in targets)
            {
                switch (target.InvokeCommand(sender, command, control))
                {
                    case CommandStatus.Handled:
                        return CommandStatus.Handled;
                    case CommandStatus.Unhandled:
                        status = CommandStatus.Unhandled;
                        break;
                }
            }
            return status;
        }

        public CommandStatus UpdateCommand(object sender, ICommand command)
        {
            CommandStatus status = CommandStatus.Incompatible;
            foreach (ICommandTarget target in targets)
            {
                switch (target.UpdateCommand(sender, command))
                {
                    case CommandStatus.Handled:
                        return CommandStatus.Handled;
                    case CommandStatus.Unhandled:
                        status = CommandStatus.Unhandled;
                        break;
                }
            }
            return status;
        }

        #endregion
    }
}
