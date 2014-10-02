using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Context.Interfaces.Services;
using Context.Interfaces.UI.CommandBars;

namespace Context.Core
{
    internal class CommandService : ICommandService
    {
        private IResourceManager resources;
        private Dictionary<string, CommandGroup> commandGroups;
        private Dictionary<Guid, ICommandTarget> commandTargets;
        private Guid defaultContextId;

        public CommandService(IServiceManager manager)
        {
            this.resources = (IResourceManager)manager.GetService(typeof(IResourceManager));
            this.commandGroups = new Dictionary<string, CommandGroup>();
            this.commandTargets = new Dictionary<Guid, ICommandTarget>();
        }

        #region ICommandService Members

        public Guid DefaultContextId
        {
            get
            {
                return defaultContextId;
            }
            set
            {
                defaultContextId = value;
            }
        }

        public void AttachCommandTarget(Guid contextId, ICommandTarget target)
        {
            ICommandTarget priorTarget;
            if (commandTargets.TryGetValue(contextId, out priorTarget))
            {
                target = new CommandTarget(priorTarget, target);
            }

            commandTargets[contextId] = target;
        }

        public void RegisterCommandGroup(string commandGroup, Guid id)
        {
            CommandGroup group = new CommandGroup(commandGroup, id);
            commandGroups.Add(commandGroup, group);
        }

        public ICommand AddCommand(string commandName, string commandGroup, string displayText, string toolTipText, int imageId, CommandBarControlType controlType, CommandBarControlBehavior behavior, Guid contextId)
        {
            if (contextId == Guid.Empty)
            {
                contextId = this.defaultContextId;
            }

            CommandGroup group = EnsureGroup(commandGroup);
            Image image = resources.GetImage(imageId);
            Command command = new Command(commandName, group, displayText, toolTipText, imageId, image, controlType, behavior, contextId);
            group.Commands.Add(commandName, command);
            return command;
        }

        private CommandGroup EnsureGroup(string commandGroup)
        {
            CommandGroup group;
            if (!commandGroups.TryGetValue(commandGroup, out group))
            {
                group = new CommandGroup(commandGroup, Guid.NewGuid());
                commandGroups.Add(commandGroup, group);
            }
            return group;
        }

        public bool RemoveCommand(ICommand command)
        {
            Command cmd = command as Command;
            if (cmd != null)
            {
                return cmd.CommandGroup.Commands.Remove(command.Name);
            }
            return false;
        }

        public ICommand FindCommand(string commandName, string commandGroup)
        {
            CommandGroup group;
            if (commandName != null && commandGroup != null && commandGroups.TryGetValue(commandGroup, out group))
            {
                return group[commandName];
            }
            return null;
        }

        public CommandStatus InvokeCommand(object sender, ICommand command, ICommandBarControl control)
        {
            ICommandTarget target;
            if (commandTargets.TryGetValue(command.ContextId, out target))
            {
                return target.InvokeCommand(sender, command, control);
            }

            return CommandStatus.Incompatible;
        }

        public CommandStatus UpdateCommand(object sender, ICommand command)
        {
            ICommandTarget target;
            if (commandTargets.TryGetValue(command.ContextId, out target))
            {
                return target.UpdateCommand(sender, command);
            }

            return CommandStatus.Incompatible;
        }

        public ICommand[] ListCommands()
        {
            int count = 0;
            foreach (CommandGroup group in commandGroups.Values)
            {
                count += group.Commands.Count;
            }

            ICommand[] arr = new ICommand[count];
            int i = 0;
            foreach (CommandGroup group in commandGroups.Values)
            {
                foreach (Command command in group.Commands.Values)
                {
                    arr[i] = command;
                    i++;
                }
            }

            return arr;
        }

        public Guid GetCommandGroupId(string commandGroup)
        {
            CommandGroup group = EnsureGroup(commandGroup);
            return group.Id;
        }

        #endregion
    }
}
