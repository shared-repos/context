using System;
using System.Drawing;
using Context.Interfaces.UI.CommandBars;
using System.Windows.Forms;

namespace Context.WinForms.UI.CommandBars
{
    internal abstract class CommandBarControl : ICommandBarControl
    {
        private readonly CommandBar parent;
        private readonly CommandBarControlType controlType;
        private readonly ICommand command;
        private readonly int position;
        private string name;
        private bool isDefault;
        private EventHandler click;

        public CommandBarControl(CommandBar parent, CommandBarControlType controlType, ICommand command, int position)
        {
            this.parent = parent;
            this.controlType = controlType;
            this.command = command;
            this.position = position;
        }

        internal ICommand GetCommand()
        {
            if (command != null)
            {
                return command;
            }

            if (parent.ParentControl != null)
            {
                return parent.ParentControl.GetCommand();
            }

            return null;
        }

        #region ICommandBarControl Members

        public event EventHandler Click
        {
            add
            {
                if (click == null)
                {
                    WireClickEvent();
                    click = value;
                }
                else
                {
                    click = (EventHandler)Delegate.Combine(click, value);
                }
            }
            remove
            {
                click = (EventHandler)Delegate.Remove(click, value);
                if (click == null)
                {
                    UnwireClickEvent();
                }
            }
        }

        protected abstract void WireClickEvent();

        protected abstract void UnwireClickEvent();

        protected virtual void ItemClick(object sender, EventArgs e)
        {
            if (click != null)
            {
                click(sender, e);
            }
        }

        public virtual string Caption
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public virtual bool Checked
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public virtual bool Enabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public ICommand Command
        {
            get
            {
                return command;
            }
        }

        public virtual object Icon
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public bool IsDefault
        {
            get
            {
                return isDefault;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public ICommandBar ParentBar
        {
            get
            {
                return this.parent;
            }
        }

        public int Position
        {
            get
            {
                return position;
            }
        }

        public virtual ShortcutEntry Shortcut
        {
            get
            {
                return new ShortcutEntry(null, null);
            }
            set
            {
            }
        }

        public virtual string ToolTip
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public CommandBarControlType Type
        {
            get
            {
                return controlType;
            }
        }

        public virtual bool Visible
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public virtual void Delete()
        {
            parent.RemoveControl(this);
        }

        #endregion

        internal virtual void SetDefault(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        internal virtual void Attach(CommandBar popup)
        {
        }

        internal virtual void Dispose()
        {
        }
    }
}
