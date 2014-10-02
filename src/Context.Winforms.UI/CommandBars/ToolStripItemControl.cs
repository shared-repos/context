using System;
using System.Collections.Generic;
using System.Text;
using Context.Interfaces.UI.CommandBars;
using System.Windows.Forms;
using System.Drawing;

namespace Context.WinForms.UI.CommandBars
{
    internal class ToolStripItemControl : CommandBarControl
    {
        private readonly ToolStripItem item;

        public ToolStripItemControl(CommandBar parent, CommandBarControlType controlType, ToolStripItem item, ICommand command, int position) : base(parent, controlType, command, position)
        {
            this.item = item;
        }

        public ToolStripItem Item
        {
            get
            {
                return item;
            }
        }

        protected override void WireClickEvent()
        {
            item.Click += ItemClick;
        }

        protected override void UnwireClickEvent()
        {
            item.Click -= ItemClick;
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

        public override bool Enabled
        {
            get
            {
                return item.Enabled;
            }
            set
            {
                item.Enabled = value;
            }
        }

        public override object Icon
        {
            get
            {
                return item.Image;
            }
            set
            {
                item.Image = value as Image;
            }
        }

        internal override void SetDefault(bool isDefault)
        {
            base.SetDefault(isDefault);

            if (isDefault)
            {
                item.Font = new Font(item.Font, FontStyle.Bold);
            }
            else
            {
                item.Font = new Font(item.Font, FontStyle.Regular);
            }
        }
    }
}
