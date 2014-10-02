using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Context.Interfaces.Services;
using Context.Interfaces.UI;
using Context.Interfaces.UI.CommandBars;

namespace Context.WinForms.UI.CommandBars
{
    internal class CommandBarService : ICommandBarService, IStatusBarService
    {
        private readonly IServiceManager manager;
        private readonly ICommandService commands;
        private readonly Dictionary<string, CommandBar> commandBars;
        private readonly List<CommandBarControl> controls;
        private ICommandTarget primaryCommandTarget;

        public CommandBarService(IServiceManager manager)
        {
            this.manager = manager;
            this.commandBars = new Dictionary<string, CommandBar>();
            this.controls = new List<CommandBarControl>();
            this.commands = (ICommandService)manager.GetService(typeof(ICommandService));
            this.primaryCommandTarget = commands;
        }

        internal void OnControlCreated(CommandBarControl control)
        {
            controls.Add(control);
        }

        internal void OnControlRemoved(CommandBarControl control)
        {
            controls.Remove(control);
        }

        internal void SetCommandTarget(ICommandTarget primaryCommandTarget)
        {
            if (primaryCommandTarget != null)
            {
                this.primaryCommandTarget = primaryCommandTarget;
            }
        }

        private Guid GetCategoryGuid(string commandGroup)
        {
            Guid groupId = commands.GetCommandGroupId(commandGroup);
            if (groupId == Guid.Empty)
            {
                return Guid.Empty;
            }

            return groupId;
        }

        public ToolStripItem EnsureBarItem(ICommand command)
        {
            string barItemName = StringHelpers.CreateCompoundIdentifier(command.Group, command.Name);
            // TODO: check already exists
            ToolStripItem item = CreateBarItem(command.ControlType);
            if (item != null)
            {
                item.Name = barItemName;
                WriteCommandToItem(command, item);
            }

            return item;
        }

        internal ToolStripItem CreateBarItem(CommandBarControlType controlType)
        {
            switch (controlType)
            {
                case CommandBarControlType.Check:
                    return null; // TODO: implement checkbox
                case CommandBarControlType.Button:
                    ToolStripButton button = new ToolStripButton();
                    return button;
                case CommandBarControlType.Menu:
                    ToolStripMenuItem menu = new ToolStripMenuItem();
                    return menu;
                // TODO: implement other types
            }

            return null;
        }

        private void WriteCommandToItem(ICommand command, ToolStripItem item)
        {
            item.Enabled = command.Enabled;
            item.Visible = command.Visible;
            item.Text = command.Text;
            item.Image = command.Icon;
            item.Click += ItemClick(command);
        }

        private EventHandler ItemClick(ICommand command)
        {
            return delegate(object sender, EventArgs args)
            {
                primaryCommandTarget.InvokeCommand(this, command, null);
            };
        }

        private ICommand GetCommandByFullName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                string[] parts = StringHelpers.GetCompoundIdentifierParts(name);
                if (parts.Length == 2)
                {
                    string commandGroup = parts[0];
                    string commandName = parts[1];
                    return commands.FindCommand(commandName, commandGroup);
                }
            }
            return null;
        }

        internal Image GetImage(int imageId)
        {
            IResourceManager resources = (IResourceManager)manager.GetService(typeof(IResourceManager));
            return resources.GetImage(imageId);
        }

        #region ICommandBarService Members

        public ICommandBar AddCommandBar(CommandBarType type, string name, string displayName, ICommandBarControl attachTo, int position, bool defaultVisible, Guid id)
        {
            CommandBar commandBar;
            if (commandBars.TryGetValue(name, out commandBar))
            {
                return commandBar;
            }

            CommandBarControl parentControl = attachTo as CommandBarControl;

            switch (type)
            {
                case CommandBarType.ToolBar:
                    ToolStrip toolbar = null;
                    commandBar = new CommandBarToolbar(this, parentControl, toolbar, displayName, position);
                    break;
                case CommandBarType.Menu:
                    MenuStrip menu = null;
                    commandBar = new CommandBarMenu(this, parentControl, menu, displayName, position);
                    break;
                case CommandBarType.Popup:
                    ContextMenuStrip popup = new ContextMenuStrip();
                    popup.RenderMode = ToolStripRenderMode.System;
                    commandBar = new CommandBarPopup(this, parentControl, popup, displayName, position);
                    break;
            }

            parentControl.Attach(commandBar);
            if (commandBar != null)
            {
                commandBars.Add(name, commandBar);
            }

            return commandBar;
        }

        private void PopupCloseUp(object sender, EventArgs e)
        {
            this.primaryCommandTarget = commands;
        }

        public ICommandBarControl AddBarControl(string commandName, string commandGroup, string commandBarName, int position)
        {
            CommandBar commandBar = commandBars[commandBarName];
            ICommand command = commands.FindCommand(commandName, commandGroup);
            if (command == null)
            {
                throw new InvalidOperationException(string.Format("Command not found: {0}.{1}", commandGroup, commandName));
            }
            return commandBar.AddControl(command, position);
        }

        public ICommandBarControl AddBarComboBox(string commandName, string commandGroup, string commandBarName, int position, int width, bool showCaption)
        {
            throw new NotImplementedException();
        }

        public ICommandBarControl AddNotifyIconControl(string commandName, string commandGroup)
        {
            ICommand command = commands.FindCommand(commandName, commandGroup);
            if (command == null)
            {
                throw new InvalidOperationException(string.Format("Command not found: {0}.{1}", commandGroup, commandName));
            }

            NotifyIconControl control = new NotifyIconControl(this, command, 0);

            control.Enabled = command.Enabled;
            control.Visible = command.Visible;
            control.Caption = command.Text;
            control.Icon = command.Icon;
            control.Click += ItemClick(command);

            OnControlCreated(control);
            return control;
        }

        public void UpdateControls()
        {
        }

        public void UpdateControls(CommandBar commandBar)
        {
        }

        public bool GetUIContextActive(Guid contextId)
        {
            throw new NotImplementedException();
        }

        public void SetUIContextActive(Guid contextId, bool active)
        {
            throw new NotImplementedException();
        }

        public ICommandBar this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICommandBar this[string name]
        {
            get
            {
                CommandBar bar;
                commandBars.TryGetValue(name, out bar);
                return bar;
            }
        }

        public int Count
        {
            get
            {
                return commandBars.Count;
            }
        }

        #endregion

        #region IStatusBarService Members

        public Bitmap Animation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetText(string text)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text, System.Drawing.Color foreColor, System.Drawing.Color backColor)
        {
            throw new NotImplementedException();
        }

        public void SetLineColumnChar(int line, int column, int character)
        {
            throw new NotImplementedException();
        }

        public void Progress(bool inProgress, string label, int complete, int total)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion

        internal void Close()
        {
            foreach (CommandBarControl control in controls)
            {
                control.Dispose();
            }
        }
    }
}
