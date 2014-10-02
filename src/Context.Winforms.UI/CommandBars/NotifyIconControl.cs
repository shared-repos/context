using System;
using System.Drawing;
using Context.Interfaces.UI.CommandBars;
using System.Windows.Forms;

namespace Context.WinForms.UI.CommandBars
{
    internal class NotifyIconControl : CommandBarControl
    {
        private readonly CommandBarService commandBars;
        private NotifyIcon item;
        private CommandBar popup;

        public NotifyIconControl(CommandBarService commandBars, ICommand command, int position) : base(null, CommandBarControlType.Button, command, position)
        {
            this.commandBars = commandBars;
            this.item = new NotifyIcon();
        }

        protected override void WireClickEvent()
        {
            item.Click += ItemClick;
        }

        protected override void UnwireClickEvent()
        {
            item.Click -= ItemClick;
        }

        protected override void ItemClick(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            if (args != null)
            {
                if (args.Button == MouseButtons.Left)
                {
                    base.ItemClick(sender, e);
                }
                else if (args.Button == MouseButtons.Right)
                {
                    popup.UpdateControls();
                }
            }
        }

        public override string Caption
        {
            get
            {
                return item.Text;
            }
            set
            {
                item.Text = value;
            }
        }

        public override object Icon
        {
            get
            {
                return item.Icon;
            }
            set
            {
                if (value == null)
                {
                    item.Icon = null;
                }
                else
                {
                    Icon icon = value as Icon;
                    if (icon != null)
                    {
                        item.Icon = icon;
                    }
                    else
                    {
                        Bitmap bmp = value as Bitmap;
                        if (bmp != null)
                        {
                            item.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
                        }
                    }
                }
            }
        }

        public override bool Visible
        {
            get
            {
                return item.Visible;
            }
            set
            {
                item.Visible = value;
            }
        }

        public override void Delete()
        {
            commandBars.OnControlRemoved(this);
        }

        internal override void Attach(CommandBar popup)
        {
            this.popup = popup;
            item.ContextMenuStrip = popup.ToolStrip as ContextMenuStrip;
        }

        internal override void Dispose()
        {
            item.Dispose();
        }
    }
}
