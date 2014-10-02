using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Context.Interfaces.UI.CommandBars;

namespace Context.WinForms.UI.CommandBars
{
    internal abstract class CommandBar : ICommandBar
    {
        private readonly CommandBarService commandBars;
        private readonly CommandBarControl parentControl;
        private readonly List<ICommandBarControl> controls;
        private readonly ToolStrip toolStrip;
        private readonly string displayName;
        private readonly int position;
        private ICommandBarControl defaultControl;
        private bool modified;

        public CommandBar(CommandBarService commandBars, CommandBarControl parentControl, ToolStrip toolStrip, string displayName, int position)
        {
            this.commandBars = commandBars;
            this.parentControl = parentControl;
            this.controls = new List<ICommandBarControl>();
            this.displayName = displayName;
            this.position = position;
            this.toolStrip = toolStrip;
        }

        public ToolStrip ToolStrip
        {
            get
            {
                return toolStrip;
            }
        }

        public CommandBarService CommandBars
        {
            get
            {
                return commandBars;
            }
        }

        public CommandBarControl ParentControl
        {
            get
            {
                return parentControl;
            }
        }

        protected virtual void SetCurrentStateAsDefault()
        {
        }

        protected virtual void Reset()
        {
        }

        public void SaveState()
        {
            if (modified)
            {
                SetCurrentStateAsDefault();
                modified = false;
            }
        }

        #region ICommandBar Members

        public event EventHandler Popup;

        public void OnPopup()
        {
            if (Popup != null)
            {
                Popup(this, EventArgs.Empty);
            }
        }

        public ICommandBarControl AddControl(CommandBarControlType controlType, bool beginGroup, int imageId, int position)
        {
            ToolStripItem item = commandBars.CreateBarItem(controlType);
            item.Image = commandBars.GetImage(imageId);
            return CreateControl(null, controlType, item, beginGroup, position);
        }

        internal CommandBarControl AddControl(ICommand command, int position)
        {
            ToolStripItem item = commandBars.EnsureBarItem(command);
            bool beginGroup = ((command.Behavior & CommandBarControlBehavior.BeginGroup) != 0);
            return CreateControl(command, command.ControlType, item, beginGroup, position);
        }

        private CommandBarControl CreateControl(ICommand command, CommandBarControlType controlType, ToolStripItem item, bool beginGroup, int position)
        {
            if (!modified)
            {
                Reset();
            }

            int i;
            ToolStripItem beforeItem = null;
            for (i = 0; i < controls.Count; i++)
            {
                if (controls[i].Position > position)
                {
                    beforeItem = ((ToolStripItemControl)controls[i]).Item;
                    break;
                }
            }

            CommandBarControl control = new ToolStripItemControl(this, controlType, item, command, position);
            int j;
            if (beforeItem != null)
            {
                j = toolStrip.Items.IndexOf(beforeItem);
            }
            else
            {
                j = toolStrip.Items.Count;
            }
            toolStrip.Items.Insert(j, item);
            if (beginGroup && j > 0)
            {
                toolStrip.Items.Insert(j, new ToolStripSeparator());
            }
            controls.Insert(i, control);
            commandBars.OnControlCreated(control);
            modified = true;
            return control;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void RemoveControl(ICommandBarControl control)
        {
            CommandBarControl barControl = (CommandBarControl)control;
            int i = controls.IndexOf(control);
            if (i < 0)
            {
                return;
            }

            if (!modified)
            {
                Reset();
            }

            controls.RemoveAt(i);
            commandBars.OnControlRemoved(barControl);
            modified = true;
        }

        public virtual void ShowPopup(Point position, ICommandTarget primaryCommandTarget)
        {
        }

        public void UpdateControls()
        {
            commandBars.UpdateControls(this);
        }

        public IList<ICommandBarControl> Controls
        {
            get
            {
                return controls;
            }
        }

        public ICommandBarControl DefaultControl
        {
            get
            {
                return defaultControl;
            }
            set
            {
                if (defaultControl != value)
                {
                    if (defaultControl != null)
                    {
                        ((CommandBarControl)defaultControl).SetDefault(false);
                    }
                    defaultControl = value;
                    if (defaultControl != null)
                    {
                        ((CommandBarControl)defaultControl).SetDefault(true);
                    }
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return displayName;
            }
        }

        public Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
        }

        ICommandBarControl ICommandBar.ParentControl
        {
            get
            {
                return parentControl;
            }
        }

        public CommandBarType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Visibility Visibility
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

        public bool Visible
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

    }
}
