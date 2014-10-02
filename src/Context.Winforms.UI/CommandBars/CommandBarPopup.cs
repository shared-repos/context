using System;
using System.Drawing;
using System.Windows.Forms;
using Context.Interfaces.UI.CommandBars;

namespace Context.WinForms.UI.CommandBars
{
    internal class CommandBarPopup : CommandBar
    {
        private readonly ContextMenuStrip popup;

        public CommandBarPopup(CommandBarService commandBars, CommandBarControl parentControl, ContextMenuStrip popup, string displayName, int position)
            : base(commandBars, parentControl, popup, displayName, position)
        {
            this.popup = popup;
        }

        public override void ShowPopup(Point position, ICommandTarget primaryCommandTarget)
        {
            UpdateControls();
            popup.Show(position);
            CommandBars.SetCommandTarget(primaryCommandTarget);
        }
    }
}
